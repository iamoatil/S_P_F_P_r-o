using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataPump.IOS
{
    /// <summary>
    /// IOS镜像数据泵。
    /// </summary>
    public class IOSMirrorDataPump : ControllableDataPumpBase
    {
        #region Methods

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            if (context.Source.ItemType == Domains.SourceFileItemType.NormalPath)
            {
                context.Source.Local = FileHelper.ConnectPath(context.GetContextData<String>("desctPath"), context.Source.Config);
            }
        }

        /// <summary>
        /// 初始化当前的执行流程。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override bool InitExecution(DataPumpControllableExecutionContext context)
        {
            return Init(context);
        }

        #endregion

        #region Private

        private Boolean Init(DataPumpControllableExecutionContext context)
        {
            //1.获取镜像文件路径
            String mirrorFile = context.PumpDescriptor.Source as String;
            if (String.IsNullOrWhiteSpace(mirrorFile)) return false;

            //2.解压
            String destPath = Path.Combine(context.TargetDirectory, $"IosData_{context.GetHashCode()}");
            if (Directory.Exists(destPath))
            {
                Directory.Delete(destPath, true);
            }
            ZipFile.ExtractToDirectory(mirrorFile, destPath);

            SetContextData(context, "destPath", destPath);
            //3.处理app文件夹
            var directories = Directory.GetDirectories(destPath);
            foreach (string path in directories)
            {
                if (path.Contains("AppDomain-"))
                {
                    Directory.Move(path, path.Replace("AppDomain-", String.Empty));
                }
            }
            return true;
        }

        #endregion

        #endregion
    }
}
