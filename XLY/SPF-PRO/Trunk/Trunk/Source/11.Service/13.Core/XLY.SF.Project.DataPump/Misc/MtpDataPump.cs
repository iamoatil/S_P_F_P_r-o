using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Devices.DeviceManager.Mtp;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.Misc
{
    /// <summary>
    /// MTP数据泵。
    /// </summary>
    public class MtpDataPump : ControllableDataPumpBase
    {
        #region Fields

        private static readonly DateTime InvalidDateTime = new DateTime(1970, 1, 1, 0, 0, 0);

        #endregion

        #region Methods

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            SourceFileItem source = context.Source;
            if (source.ItemType != SourceFileItemType.FileExtension)
            {
                throw new InvalidOperationException("Only support FileExtension");
            }
            MTPDevice mtpDevice = GetMTPDevice(context);
            if (mtpDevice == null) return;
            //2.解析文件类型和文件后缀名列表
            var filetype = Regex.Match(source.Config, @"^\$(\S+),").Groups[1].Value;
            var extensions = source.Config.Substring(filetype.Length).Split(';').Select(ex => string.Format(".{0}", ex));
            //3.获取文件列表
            var fileNodes = mtpDevice.GetFiles(extensions);
            //4.拷贝文件
            String sourcePath;
            String destPath;
            foreach (var fileNode in fileNodes)
            {
                sourcePath = fileNode.GetFullPath();
                destPath = Path.Combine(context.TargetDirectory, filetype, sourcePath);
                FileHelper.CreateDirectory(destPath);
                if (MtpDeviceManager.Instance.CopyFileFromDevice(mtpDevice, fileNode, destPath))
                {
                    var copyfile = new FileInfo(Path.Combine(destPath, fileNode.Name));
                    if (copyfile.Exists)
                    {
                        //修改文件的 创建时间、最后修改时间、最后访问时间
                        MtpDeviceManager.Instance.GetDate(mtpDevice, fileNode);

                        File.SetCreationTime(copyfile.FullName, CovertMTPDateTime(fileNode.DateCreated));
                        File.SetLastWriteTime(copyfile.FullName, CovertMTPDateTime(fileNode.DateModified));
                        File.SetLastAccessTime(copyfile.FullName, CovertMTPDateTime(fileNode.DateAuthored));
                    }
                }
            }
        }

        #endregion

        #region Private

        private MTPDevice GetMTPDevice(DataPumpControllableExecutionContext context)
        {
            Device device = context.PumpDescriptor.Source as Device;
            if (device == null) return null;
            MTPDevice mtpDevice = MtpDeviceManager.Instance.GetMTPDevice(device);
            if (mtpDevice != null)
            {
                mtpDevice.RootFileNode = MtpDeviceManager.Instance.GetRootFileNode(mtpDevice, context.Reporter);
            }
            return mtpDevice;
        }

        private static DateTime CovertMTPDateTime(String mtpDateTime)
        {
            //MTP读取出的时间格式为 
            //WIN10 yyyy/MM/DD:HH:MM:ss.fff
            //WIN7  yyyy/MM/DD HH:MM:ss
            if (!mtpDateTime.IsValid())
            {
                return InvalidDateTime;
            }
            String[] arr = mtpDateTime.Split(new Char[] { '/', ':', '.', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (!arr.IsValid() || arr.Length < 6)
            {
                return InvalidDateTime;
            }

            DateTime dt;
            if (DateTime.TryParse(String.Format("{0}-{1}-{2} {3}:{4}:{5}", arr[0], arr[1], arr[2], arr[3], arr[4], arr[5]), out dt))
            {
                if (dt.Year == 1980 && dt.Month == 1 && dt.Day == 1 && dt.Hour == 0 && dt.Minute == 0 && dt.Second == 0)
                {
                    return InvalidDateTime;
                }
                return dt;
            }
            else
            {
                return InvalidDateTime;
            }

        }

        #endregion

        #endregion
    }
}
