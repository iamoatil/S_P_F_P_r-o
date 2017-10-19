using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.Android
{
    /// <summary>
    /// Android 未ROOT数据泵。
    /// </summary>
    public class AndroidUsbUnrootDataPump : ControllableDataPumpBase
    {
        #region Fields

        public static readonly String[] Commands;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.Android.AndroidUsbUnrootDataPump 。
        /// </summary>
        static AndroidUsbUnrootDataPump()
        {
            Commands = new[]
            {
                "basic_info","app_info","sms_info",
                "contact_info","calllog_info"
            };
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            String destPath = context.GetContextData<String>("destPath");
            Device device = context.GetContextData<Device>("device");
            AndroidHelper.Instance.BackupAndResolve(device, FileHelper.ConnectPath(destPath, $"{device.SerialNumber}.rar"));

            String content = String.Empty;
            foreach (String command in Commands)
            {
                content = AndroidHelper.Instance.ExecuteSPFAppCommand(device, command);
                File.WriteAllText(FileHelper.ConnectPath(destPath, $"{command}.txt"), content);
            }
        }

        /// <summary>
        /// 初始化当前的执行流程。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitExecution(DataPumpControllableExecutionContext context)
        {
            if (context.PumpDescriptor.Source is Device device)
            {
                String path = FileHelper.GetPhysicalPath(@"Toolkits\app\SPFSocket.apk");
                if (AndroidHelper.Instance.InstallPackage(path, device))
                {
                    path = FileHelper.ConnectPath(context.TargetDirectory, $"AndroidData_{context.GetHashCode()}");
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    FileHelper.CreateDirectory(path);
                    SetContextData(context, "destPath", path);
                    SetContextData(context, "device", device);
                }
            }
            return false;
        }

        #endregion

        #region Private

        #endregion

        #endregion
    }
}
