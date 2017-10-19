using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;
using System.IO.Compression;
using XLY.SF.Project.Devices;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataMirror
{
    internal class IOSDeviceMirrorService : AbstractMirrorService
    {
        private IOSDeviceManager DeviceManager { get; set; }

        public override void Execute(Mirror mirror, IAsyncProgress asyn)
        {
            var device = mirror.Source as Device;

            DeviceManager = device.DeviceManager as IOSDeviceManager;

            //数据缓存路径
            var tempSavePath = FileHelper.ConnectPath(mirror.Target, "temp");
            FileHelper.CreateExitsDirectorySafe(tempSavePath);

            //数据备份
            var resPath = DeviceManager.CopyUserData(device, tempSavePath, asyn);

            if (!FileHelper.IsValidDictory(resPath))
            {//数据拷贝失败！

            }

            //打包
            mirror.Local = FileHelper.ConnectPath(mirror.Target, mirror.TargetFile);
            ZipFile.CreateFromDirectory(resPath, mirror.Local);

            if (!FileHelper.IsValid(mirror.Local))
            {//打包失败！

            }

            //删除缓存文件
            FileHelper.DeleteDirectorySafe(tempSavePath);
        }

        public override void Stop(IAsyncProgress asyn)
        {
            DeviceManager?.StopCopyUserData();
            DeviceManager = null;
        }
    }
}
