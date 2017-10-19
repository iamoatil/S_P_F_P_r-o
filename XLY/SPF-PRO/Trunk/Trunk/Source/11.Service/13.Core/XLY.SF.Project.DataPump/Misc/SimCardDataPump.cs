using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.DataPump.Misc
{
    /// <summary>
    /// SIM卡数据泵（这是一个打酱油的数据泵）。
    /// </summary>
    public class SimCardDataPump : DataPumpBase
    {
        #region Methods

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpExecutionContext context)
        {
        }

        #endregion

        #endregion
    }
}
