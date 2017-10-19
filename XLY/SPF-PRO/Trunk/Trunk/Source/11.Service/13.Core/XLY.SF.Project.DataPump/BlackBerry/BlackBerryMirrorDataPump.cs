using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataPump.BlackBerry
{
    /// <summary>
    /// 黑莓镜像数据泵。
    /// </summary>
    public class BlackBerryMirrorDataPump : ControllableDataPumpBase
    {
        #region Methods

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            String dataSourcePath = context.PumpDescriptor.Source as String;
            String destDirectory = context.Source.Local;
            ZipFile.ExtractToDirectory(dataSourcePath, destDirectory);
        }

        /// <summary>
        /// 初始化当前的执行流程。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitExecution(DataPumpControllableExecutionContext context)
        {
            String dataSourcePath = context.PumpDescriptor.Source as String;
            if (String.IsNullOrWhiteSpace(dataSourcePath)) return false;
            String destDirectory = FileHelper.ConnectPath(context.TargetDirectory, $"BlackBerry_{dataSourcePath.GetHashCode()}");
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
