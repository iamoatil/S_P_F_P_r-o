// ***********************************************************************
// Assembly:XLY.SF.Project.DataExtract
// Author:Songbing
// Created:2017-03-28 13:24:44
// Description:数据提取控制服务
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.DataPump;
using XLY.SF.Project.Domains;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataExtract
{
    /// <summary>
    /// 数据提取控制器
    /// </summary>
    public class DataExtractControler
    {

        #region 基础属性和构造器

        /// <summary>
        /// 所属任务实例
        /// </summary>
        public SPFTask OwnerTask { get; set; }

        /// <summary>
        /// 提取对象
        /// </summary>
        public Pump SourcePump { get; set; }

        /// <summary>
        /// 提取项集合
        /// </summary>
        public IEnumerable<ExtractItem> ExtractItems { get; set; }

        /// <summary>
        /// 工作模式
        /// </summary>
        public EnumDataExtractWorkMode WorkMode { get; set; }

        /// <summary>
        /// 异步通知
        /// </summary>
        public IAsyncProgress Asyn { get; set; }

        /// <summary>
        /// 数据提取控制器构造器
        /// </summary>
        /// <param name="task">当前任务实例</param>
        /// <param name="source">提取对象</param>
        /// <param name="extractItems">提取项集合</param>
        /// <param name="asyn">异步通知</param>
        /// <param name="workMode">工作模式</param>
        public DataExtractControler(SPFTask task, Pump source, ExtractItemCollection extractItems, IAsyncProgress asyn, EnumDataExtractWorkMode workMode)
        {
            OwnerTask = task;
            SourcePump = source;
            Asyn = asyn;
            WorkMode = workMode;
            ExtractItems = extractItems.GetAllCheckedExtractItem();

            MainWorkThread = new SingleThread();
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 开始数据提取
        /// 异步执行
        /// 内部处理异常
        /// 通过异步通知返回进度和提取结果
        /// </summary>
        public void Start()
        {
            DoStart();
        }

        /// <summary>
        /// 停止数据提取
        /// </summary>
        public void Stop()
        {
            DoStop();
        }

        #endregion

        #region 私有方法和私有属性

        /// <summary>
        /// 后台工作主线程
        /// </summary>
        private SingleThread MainWorkThread { get; set; }

        /// <summary>
        /// 数据泵服务
        /// </summary>
        //private DataPumpControler PumpControler { get; set; }

        /// <summary>
        /// 插件管理器
        /// </summary>
        private IPluginAdapter PluginAdapter { get; set; }

        /// <summary>
        /// 数据文件保存根目录
        /// </summary>
        private string RootSourceDataPath { get; set; }

        /// <summary>
        /// CancelToken
        /// </summary>
        private CancellationTokenSource CancelToken { get; set; }

        /// <summary>
        /// 数据泵和插件工作线程池
        /// </summary>
        private List<SingleThread> WorkThreadPool { get; set; }

        /// <summary>
        /// 工作线程池锁
        /// </summary>
        private object WorkThreadPoolLock { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        private void DoStart()
        {
            MainWorkThread.Start(() =>
                {
                    //初始化
                    Initialization();

                    //开始提取
                    DoDataExtract().Wait();
                });
        }

        /// <summary>
        /// 停止
        /// </summary>
        private void DoStop()
        {
            //取消
            CancelToken.Cancel();

            //停止数据泵服务
            //PumpControler.Stop();

            //停止工作线程池
            lock (WorkThreadPoolLock)
            {
                WorkThreadPool.ForEach(st => st.Stop());
                WorkThreadPool = null;
            }

            //停止后台工作主线程
            if (MainWorkThread.IsAlive)
            {
                MainWorkThread.Stop();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialization()
        {
            /*
             * 初始化数据提取控制器
             * 1.获取插件控制器
             * 2.初始化数据文件保存根目录
             * 3.获取数据泵服务
             * 4.初始化数据泵服务
             * 5.初始化异步取消token
             * 6.初始化工作线程池
             * */

            //1.获取插件控制器
            PluginAdapter = IocManagerSingle.Instance.GetPart<IPluginAdapter>("PluginAdapter");

            //2.初始化数据文件保存根目录
            RootSourceDataPath = OwnerTask.TaskSourcePath;
            FileHelper.CreateExitsDirectory(RootSourceDataPath);

            //3.获取数据泵服务
            //PumpControler = new DataPumpControler();

            //4.初始化数据泵服务
            //PumpControler.Init(SourcePump, ExtractItems, RootSourceDataPath, Asyn);

            //5.初始化异步取消token
            CancelToken = new CancellationTokenSource();

            //6.初始化工作线程池
            WorkThreadPool = new List<SingleThread>();
            WorkThreadPoolLock = new object();
        }

        /// <summary>
        /// 执行数据提取
        /// </summary>
        private async Task DoDataExtract()
        {
            /*
             * APP数据提取流程：
             * 1.根据设备类型和提取方式获取该APP的所有插件
             * 2.通过数据泵服务获取SourceFileItems
             * 3.通过特征匹配和版本匹配确定要执行的插件版本
             * 4.执行插件
             * 5.获取解析结果
             * 
             * */

            var items = PluginAdapter.MatchPluginByPump(SourcePump, ExtractItems);

            switch (WorkMode)
            {
                case EnumDataExtractWorkMode.HalfAsync:
                    await DoHalfAsyncDataExtract(items);
                    break;
                default:
                    throw new NotImplementedException();
            }

        }

        /// <summary>
        /// 半异步提取方式
        /// </summary>
        /// <returns></returns>
        private async Task DoHalfAsyncDataExtract(Dictionary<ExtractItem, List<DataParsePluginInfo>> Items)
        {
            foreach (var item in Items)
            {
                //1.同步执行数据泵服务
                CancelToken.Token.ThrowIfCancellationRequested();
                await DoDataPump(item.Key, item.Value);

                //2.异步执行插件
                CancelToken.Token.ThrowIfCancellationRequested();
                //DoDataPlug(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 执行数据泵服务
        /// </summary>
        /// <param name="extractItem"></param>
        /// <param name="plugs"></param>
        /// <returns></returns>
        private async Task DoDataPump(ExtractItem extractItem, IEnumerable<DataParsePluginInfo> plugs)
        {
            await Task.Run(() =>
                {
                    WaitRunWorkThread(() =>
                        {
                            //执行数据泵服务
                            foreach (var plug in plugs)
                            {
                                //plug.SourcePath.ForEach(s => PumpControler.Extract(s, Asyn));
                            }
                        }, () =>
                        {
                            //TODO:取消执行数据泵服务后处理
                        });
                });
        }

        /// <summary>
        /// 执行插件
        /// </summary>
        /// <param name="extractItem"></param>
        /// <param name="plugs"></param>
        /// <returns></returns>
        private async Task DoDataPlug(ExtractItem extractItem, IEnumerable<DataParsePluginInfo> plugs)
        {
            await Task.Run(() =>
                {
                    WaitRunWorkThread(() =>
                        {
                            //1.匹配插件
                            var plug = PluginAdapter.MatchPluginByApp(plugs, SourcePump, RootSourceDataPath, GetAppVersion(extractItem));

                            //2.执行插件
                            PluginAdapter.ExecutePlugin(plug, Asyn, (ds) =>
                                {//插件执行完处理
                                    FinishExtractItem(extractItem, ds);
                                });

                        }, () =>
                        {
                            //TODO:取消插件执行后处理
                        });
                });
        }

        /// <summary>
        /// 提取项执行完毕
        /// </summary>
        /// <param name="extractItem"></param>
        private void FinishExtractItem(ExtractItem extractItem, IDataSource ds)
        {
            //1.处理IDataSource

            //2.结尾处理
            extractItem.IsFinish = true;

            //3.判断是否全部提取完成
            if (ExtractItems.All(e => e.IsFinish))
            {//全部提取完成

            }
        }

        /// <summary>
        /// 获取提取项APP版本号
        /// </summary>
        /// <param name="extractItem"></param>
        /// <returns></returns>
        private Version GetAppVersion(ExtractItem extractItem)
        {
            //TODO:获取APP版本信息
            return null;
        }

        #region 工作线程辅助方法

        /// <summary>
        /// 在后台工作线程中干活
        /// </summary>
        /// <param name="dowork">工作内容</param>
        /// <param name="abortCallback">工作取消后续处理</param>
        private void WaitRunWorkThread(Action dowork, Action abortCallback = null)
        {
            var st = CreateWorkThread();
            if (null == st)
            {
                return;
            }

            st.Start(() =>
            {
                try
                {
                    dowork();
                }
                catch (ThreadAbortException)
                {
                    abortCallback?.Invoke();
                }
            });

            st.Wait(() =>
            {
                RemoveWorkThread(st);
            });
        }

        /// <summary>
        /// 创建新的工作线程
        /// </summary>
        /// <returns></returns>
        private SingleThread CreateWorkThread()
        {
            lock (WorkThreadPoolLock)
            {
                if (null == WorkThreadPool)
                {
                    return null;
                }

                var st = new SingleThread();
                WorkThreadPool.Add(st);

                return st;
            }
        }

        /// <summary>
        /// 移除工作线程
        /// </summary>
        /// <param name="st"></param>
        private void RemoveWorkThread(SingleThread st)
        {
            if (null != st)
            {
                st.Wait();

                lock (WorkThreadPoolLock)
                {
                    WorkThreadPool?.Remove(st);
                }
            }
        }

        #endregion

        #endregion

    }

}
