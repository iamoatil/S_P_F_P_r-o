/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/11 14:44:26 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.DataPump.Android;
using XLY.SF.Project.DataPump.IOS;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 数据泵控制器
    /// </summary>
    [Obsolete("请使用 DataPumpExtensions 扩展类型替代，因此此版本的Context存在不安全的Source属性，在以后的版本中会移除该类型")]
    public class DataPumpControler
    {
        #region Properties

        public DataPumpBase DataPump { get; private set; }

        public DataPumpExecutionContext Context { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 初始化数据泵
        /// </summary>
        /// <param name="pump">数据泵</param>
        /// <param name="ExtractItems">提取项</param>
        /// <param name="rootSavePath">本地储存根目录</param>
        /// <param name="asyn">异步通知</param>
        [Obsolete("在以后的版本中会移除该方法")]
        public void Init(Pump pump, IEnumerable<ExtractItem> extractItems, String rootSavePath, IAsyncProgress asyn)
        {
            DataPump = pump.GetDataPump();
            Context = DataPump.CreateContext(pump, rootSavePath, null, null, asyn);
        }

        /// <summary>
        /// 提取数据路径
        /// </summary>
        /// <param name="item">要提取的项</param>
        /// <param name="asyn">异步通知接口</param>
        /// <returns>是否提取成功</returns>
        [Obsolete("请使用 DataPumpExtensions.Execute 扩展方法替代，在以后的版本中会移除该方法")]
        public void Extract(SourceFileItem item, IAsyncProgress asyn)
        {
            if (Context == null) return;
            Context.UnsafeSource = item;
            DataPumpControllableExecutionContext contextEx = Context as DataPumpControllableExecutionContext;
            if (contextEx != null) contextEx.Reporter = asyn;
            DataPump.Execute(Context);
        }

        /// <summary>
        /// 停止当前正在进行的任务
        /// </summary>
        [Obsolete("请使用 DataPumpExtensions.Cancel 扩展方法替代，在以后的版本中会移除该方法")]
        public void Stop()
        {
            Context?.Cancel();
        }

        #endregion

        #endregion
    }
}
