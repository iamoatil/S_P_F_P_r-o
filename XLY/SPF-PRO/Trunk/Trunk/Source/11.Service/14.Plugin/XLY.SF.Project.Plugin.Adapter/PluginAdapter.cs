using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Domains;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Adapter
{
    /// <summary>
    /// 插件控制器
    /// </summary>
    [Export("PluginAdapter", typeof(IPluginAdapter))]
    public class PluginAdapter : IPluginAdapter
    {
        public static PluginAdapter Instance => SingleWrapperHelper<PluginAdapter>.Instance;

        /// <summary>
        /// 插件加载器
        /// </summary>
        [ImportMany("PluginLoader", typeof(IPluginLoader))]
        public List<IPluginLoader> PluginLoaders { get; set; }

        /// <summary>
        /// 插件列表
        /// </summary>
        public Dictionary<AbstractPluginInfo, IPlugin> Plugins { get; set; }

        #region 初始化控制器

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialization(IAsyncProgress asyn)
        {
            Plugins = new Dictionary<AbstractPluginInfo, IPlugin>();
            var pluginLoaders = IocManagerSingle.Instance.GetParts<IPluginLoader>(PluginExportKeys.PluginLoaderKey);
            foreach (var loader in pluginLoaders)
            {
                var pls = loader.Value.Load(asyn);

                foreach (var pl in pls)
                {
                    if (null != pl.PluginInfo)
                    {
                        Plugins.Add(pl.PluginInfo as AbstractPluginInfo, pl);
                    }
                }
            }
        }

        #endregion

        #region 公开方法
        /// <summary>
        /// 通过插件类型和状态获取当前所有的插件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">插件类型</param>
        /// <param name="state">插件状态</param>
        /// <returns>满足条件的所有插件</returns>
        public Dictionary<T, IPlugin> GetPluginsByType<T>(PluginType type = PluginType.SpfDataParse, PluginState state = PluginState.None) where T : AbstractPluginInfo
        {
            var plugin = state == PluginState.None ? Plugins.Where(p => p.Key.PluginType == type && p.Key is T) :
                Plugins.Where(p => p.Key.PluginType == type && p.Key is T && state.HasFlag(p.Key.State));
            return plugin.ToDictionary(x => (T)x.Key, x => x.Value);
        }

        /// <summary>
        /// 获取提取项列表
        /// </summary>
        /// <param name="source">数据泵</param>
        /// <returns></returns>
        public ExtractItemCollection GetAllExtractItems(Pump source)
        {
            ExtractItemCollection extracts = new ExtractItemCollection();

            //插件过滤 根据提取方式和操作系统
            var filtetResult = GetPluginsByType<DataParsePluginInfo>(PluginType.SpfDataParse).Keys.Where(p => p.Pump.HasFlag(source.Type) &&
                                                            p.DeviceOSType.HasFlag(source.OSType));

            //加载提取项列表
            extracts.Load(filtetResult);

            return extracts;
        }

        /// <summary>
        /// 智能匹配插件 按过滤器条件
        /// </summary>
        /// <param name="pluginList"></param>
        /// <param name="pluginFilters"></param>
        /// <returns></returns>
        public List<DataParsePluginInfo> SmartMatchPlugin(List<DataParsePluginInfo> pluginList, List<PluginMatchFilter> filterList)
        {
            List<DataParsePluginInfo> resultList = new List<DataParsePluginInfo>();

            foreach (var item in filterList)
            {
                var result = pluginList.Where(p => p.AppName == item.AppName);
                if (result.Count() == 0)
                    return resultList;
                result = result.Where(p => p.DeviceOSType.HasFlag(item.EnumOSType));
                if (result.Count() == 0)
                    return resultList;
                result = result.Where(p => p.Pump.HasFlag(item.PumpType));
                if (result.Count() == 0)
                    return resultList;

                if (item.AppVersion.Equals(new Version("0.0")))
                {
                    resultList.AddRange(result);
                    continue;
                }

                if (result.Count() == 1)
                {
                    resultList.AddRange(result);
                    continue;
                }

                if (item.AppName.StartsWith("com.apple") && item.EnumOSType == EnumOSType.IOS)
                {
                    var r = VersionSmartMatch(result, item.AppVersion);
                }

                //安卓QQ微信不进行版本匹配
                if (item.AppName == "com.tencent.mobileqq" || item.AppName == "com.tencent.mm")
                {
                    resultList.AddRange(result);
                    continue;
                }


                if (item.Manufacture != null)
                {
                    result = result.Where(p => p.Manufacture.Equals(item.Manufacture, StringComparison.OrdinalIgnoreCase)
                            || (p.Manufacture.Equals(item.Brand, StringComparison.OrdinalIgnoreCase)));
                }

                var res = VersionSmartMatch(pluginList.Where(p => p.AppName == item.AppName), item.AppVersion);
                if (res != null)
                    resultList.Add(res);
            }

            return resultList;
        }

        /// <summary>
        /// 版本匹配（优先级：相等=>高版本=>低版本）
        /// </summary>
        private DataParsePluginInfo VersionSmartMatch(IEnumerable<DataParsePluginInfo> plugins, Version version)
        {
            try
            {
                if (plugins.IsInvalid())
                {
                    return null;
                }
                //if only one
                if (plugins.Count() == 1)
                {
                    return plugins.First();
                }
                // 相等
                var p = plugins.Where(s => s.Version == version);
                if ((p != null) && (p.Count() > 0))
                {
                    return p.First();
                }

                // 低版本 最近一个
                var less = plugins.Where(s => s.Version < version);
                if ((less != null) && (less.Count() > 0))
                {
                    return less.OrderByDescending(s => s.Version).First();
                }

                // 高版本 最近一个
                var greater = plugins.Where(s => s.Version > version);
                if ((greater != null) && (greater.Count() > 0))
                {
                    return greater.OrderBy(s => s.Version).First();
                }

                //return the latest version plugin
                return plugins.OrderByDescending(s => s.Version).First();
            }
            catch
            {

            }
            return null;
        }


        /// <summary>
        /// 匹配插件（根据数据泵，即设备类型、设备操作系统类型、数据提取类型匹配插件）
        /// </summary>
        /// <param name="source">数据泵</param>
        /// <param name="extractItems">选择的提取项</param>
        /// <returns></returns>
        public Dictionary<ExtractItem, List<DataParsePluginInfo>> MatchPluginByPump(Pump source, IEnumerable<ExtractItem> extractItems)
        {
            var result = new Dictionary<ExtractItem, List<DataParsePluginInfo>>();

            //插件过滤 根据提取方式和操作系统
            var filtetResult = GetPluginsByType<DataParsePluginInfo>(PluginType.SpfDataParse).Keys.Where(p => p.Pump.HasFlag(source.Type) &&
                                                            p.DeviceOSType.HasFlag(source.OSType));

            //插件匹配
            foreach (var extract in extractItems)
            {
                result.Add(extract, filtetResult.Where(p => p.Name == extract.Name && p.Group == extract.Parent.Name).ToList());
            }

            return result;
        }

        /// <summary>
        /// 匹配插件（根据特征库和APP版本信息匹配插件）
        /// </summary>
        /// <param name="pump">数据泵类型</param>
        /// <param name="appSourePath">APP本地数据根目录</param>
        /// <param name="appVersion">APP版本信息</param>
        /// <returns></returns>
        public DataParsePluginInfo MatchPluginByApp(IEnumerable<DataParsePluginInfo> pluginList, Pump pump, string appSourePath, Version appVersion)
        {
            return PluginFeatureMathchService.FeatureMathch(pluginList, pump, appSourePath, appVersion);
        }

        /// <summary>
        /// 执行插件
        /// </summary>
        /// <param name="plugin">要执行的插件</param>
        /// <param name="asyn">异步通知</param>
        /// <param name="callback">插件执行完回调</param>
        public void ExecutePlugin(DataParsePluginInfo plugin, IAsyncProgress asyn, Action<IDataSource> callback)
        {
            var pl = Plugins[plugin] as AbstractDataParsePlugin;
            if (null != pl)
            {
                pl.StartTime = DateTime.Now;
                var ds = pl.Execute(null, asyn) as IDataSource;
                pl.EndTime = DateTime.Now;
                callback?.Invoke(ds);
            }
        }

        /// <summary>
        /// 批量执行插件集
        /// </summary>
        /// <param name="plugins">要执行的插件列表</param>
        /// <param name="asyn">异步通知</param>
        /// <param name="callback">插件执行完回调</param>
        public void ExecutePluginList(List<DataParsePluginInfo> plugins, IAsyncProgress asyn, Action<IDataSource> callback)
        {
            if (plugins != null)
            {
                return;
            }

            var pls = plugins.ToList();

            try
            {
                foreach (var p in pls)
                {
                    ExecutePlugin(p, asyn, callback);
                }
            }
            catch (OperationCanceledException oe)
            {
                LoggerManagerSingle.Instance.Warn("Exit analysis task:" + oe.Message);
                return;
            }
        }

        //private static string _ScriptContextRunName = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "XLY.SF.Tools.ScriptContextRun.exe");

        ///// <summary>
        ///// 执行JS插件
        ///// </summary>
        ///// <param name="jsCode">要执行的插件的JS源码</param>
        ///// <returns></returns>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public object ExecuteJs(string jsCode)
        //{
        //    if (!System.IO.File.Exists(_ScriptContextRunName))
        //    {
        //        return new JavaScriptContext().Execute(jsCode);
        //    }
        //    else
        //    {
        //        string res = "";
        //        try
        //        {
        //            string jsFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, String.Format("JS_{0}.jscode", Guid.NewGuid().ToString()));
        //            System.IO.File.WriteAllText(jsFile, jsCode, System.Text.Encoding.UTF8);

        //            Process pro = new Process();
        //            pro.StartInfo.FileName = _ScriptContextRunName;
        //            pro.StartInfo.Arguments = string.Format("\"{0}\"", jsFile);
        //            pro.StartInfo.UseShellExecute = false;
        //            pro.StartInfo.CreateNoWindow = true;

        //            pro.Start();
        //            pro.WaitForExit();

        //            if (System.IO.File.Exists(jsFile))
        //            {
        //                res = System.IO.File.ReadAllText(jsFile, System.Text.Encoding.UTF8);
        //                System.IO.File.Delete(jsFile);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LoggerManagerSingle.Instance.Error("JS execute error:" + ex.Message);
        //        }

        //        return res;
        //    }
        //}

        #endregion

    }
}
