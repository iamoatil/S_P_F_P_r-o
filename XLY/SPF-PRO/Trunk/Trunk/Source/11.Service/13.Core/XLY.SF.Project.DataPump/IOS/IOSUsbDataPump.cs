using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.IOS
{
    /// <summary>
    /// IOS USB数据泵。
    /// </summary>
    public class IOSUsbDataPump : ControllableDataPumpBase
    {
        #region Methods

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            String savePath = context.GetContextData<String>("savePath");
            if (context.Source.ItemType == SourceFileItemType.NormalPath)
            {
                context.Source.Local = FileHelper.ConnectPath(savePath, context.Source.Config);
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
            Device device = context.PumpDescriptor.Source as Device;
            if (device == null) return false;
            IOSDeviceManager dm = device.DeviceManager as IOSDeviceManager;
            if (dm == null) return false;
            String savePath = dm.CopyUserData(device, context.TargetDirectory, context.Reporter);
            if (FileHelper.IsValidDictory(savePath))
            {
                RenameUnofficialApp(savePath);
            }
            SetContextData(context, "savePath", savePath);
            return true;
        }

        /// <summary>
        /// 修改第三方应用的文件夹名字
        /// </summary>
        private void RenameUnofficialApp(String iphonePath)
        {
            if (iphonePath != null && Directory.Exists(iphonePath))
            {
                String[] files = Directory.GetDirectories(iphonePath);
                foreach (String file in files)
                {
                    var fileName = Path.GetFileName(file);
                    //第三方应用
                    if (!fileName.Contains("com.apple.") && fileName.Contains("AppDomain-"))
                    {
                        var newFileName = fileName.Replace("AppDomain-", "");
                        Directory.Move(Path.Combine(iphonePath, fileName), Path.Combine(iphonePath, newFileName));
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
