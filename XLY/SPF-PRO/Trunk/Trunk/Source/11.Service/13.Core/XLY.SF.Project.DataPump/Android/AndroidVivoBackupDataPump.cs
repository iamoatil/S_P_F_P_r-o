using DllClient;
using DllClient.Callback;
using DllClient.ServiceReference1;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.Android
{
    /// <summary>
    /// Vivo数据泵。
    /// </summary>
    public class AndroidVivoBackupDataPump : ControllableDataPumpBase
    {
        #region Methods

        #region Public

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            SourceFileItem source = context.Source;
            if (source.ItemType == SourceFileItemType.NormalPath)
            {
                string path = source.Config.TrimEnd("#F");
                if (path.StartsWith("/data/data/"))
                {
                    path = ("/data/" + path.TrimStart("/data/data/"));
                }
                path = path.Replace("/", @"\");
                source.Local = FileHelper.ConnectPath(context.TargetDirectory, path);
            }
        }

        /// <summary>
        /// 初始化当前的执行流程。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitExecution(DataPumpControllableExecutionContext context)
        {
            return Init(context);
        }

        #endregion

        #region Private

        private Boolean Init(DataPumpControllableExecutionContext context)
        {
            Device device = context.PumpDescriptor.Source as Device;
            if (device == null) return false;
            var service = X86DLLClientSingle.Instance.VivoBackupAPIChannel;
            Int32 handle = service.VivoBackup_OpenDevice(device.ID);
            if (handle != 0)
            {
                Int32 result = -1;
                for (Int32 i = 0; i < 2; i++)
                {
                    result = service.VivoBackup_Initialize(handle);
                    if (result == 0) break;
                }
                if (result == 0)
                {
                    var response = service.VivoBackup_GetAppIDList(new VivoBackup_GetAppIDListRequest() { imgHandle = handle, listAppId = new string[] { } });
                    if (0 == response.VivoBackup_GetAppIDListResult && !response.listAppId.IsInvalid())
                    {
                        var apps = context.ExtractItems?.Select(s => s.AppName).Intersect(response.listAppId);
                        if (apps == null) return false;
                        VivoBackupCallBackDelegate callback = (Int64 size, String fileName, ref Int32 stop) =>
                        {
                            if (context.Reporter.State == AsyncProgressState.Stopping)
                            {
                                stop = 1;
                            }
                        };
                        X86DLLClientSingle.Instance.ClientCallback._VivoBackupCallBack += callback;
                        try
                        {
                            result = service.VivoBackup_BackupFiles(handle, context.TargetDirectory, apps.ToArray(), apps.Count());
                            if (result == 0)
                            {
                                String path = Path.Combine(context.TargetDirectory, device.ID);
                                if (Directory.Exists(path))
                                {
                                    String dataPath = Path.Combine(context.TargetDirectory, "data");
                                    if (!Directory.Exists(dataPath)) FileHelper.CreateDirectory(dataPath);
                                    foreach (DirectoryInfo dir in new DirectoryInfo(path).GetDirectories())
                                    {
                                        //第三方应用
                                        if (dir.Name == "thirdapp")
                                        {
                                            var zips = dir.GetFiles("*.zip");
                                            foreach (var zipfile in zips)
                                            {
                                                ZipFile.ExtractToDirectory(zipfile.FullName, dataPath);
                                            }
                                        }//基本信息,例如calllog contact sms
                                        else
                                        {
                                            Directory.Move(dir.FullName, Path.Combine(dataPath, dir.Name));
                                        }
                                    }
                                    FileHelper.DeleteDirectorySafe(dataPath);
                                    return true;
                                }
                            }
                        }
                        finally
                        {
                            X86DLLClientSingle.Instance.ClientCallback._VivoBackupCallBack -= callback;
                        }
                    }
                }
            }
            LoggerManagerSingle.Instance.Error("Vivo手机数据备份失败！");
            context.Reporter.IsSuccess = false;
            context.Reporter.Stop();
            return false;
        }

        #endregion

        #endregion
    }
}
