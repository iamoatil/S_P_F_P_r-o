using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.BlackBerry
{
    /// <summary>
    /// 黑莓USB数据泵。
    /// </summary>
    public class BlackBerryUsbDataPump : ControllableDataPumpBase
    {
        #region Methods

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            String destDirectory = context.Source.Local;
            //BlackBerryDeviceManager deviceManager = BlackBerryDeviceManager.Instance;
            //deviceManager.DataPumpType = true;
            //deviceManager.CopyFile((Device)context.Metadata.Source, "", destDirectory, context.Reporter);
        }

        /// <summary>
        /// 初始化当前的执行流程。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitExecution(DataPumpControllableExecutionContext context)
        {
            if (!(context.PumpDescriptor.Source is Device)) return false;
            if (context.Source == null) return false;
            String destDirectory = FileHelper.ConnectPath(context.TargetDirectory, $"BlackBerry_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}");
            if (Directory.Exists(destDirectory))
            {
                Directory.Delete(destDirectory, true);
            }
            Directory.CreateDirectory(destDirectory);
            context.Source.Local = destDirectory;
            return true;
        }

        #endregion

        #endregion
    }
}
