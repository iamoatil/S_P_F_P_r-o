using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DllClient.ServiceReference1;
using System.ServiceModel;

namespace DllClient.Callback
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class ServerCallback : ICoreServiceCallback
    {
        /// <summary>
        /// 安卓设备手机镜像回调
        /// </summary>
        public ImageDataCallBackDelegate _ImageDataCallBack;

        /// <summary>
        /// Vivo备份回调
        /// </summary>
        public VivoBackupCallBackDelegate _VivoBackupCallBack;

        /// <summary>
        /// 黑莓备份回调
        /// </summary>
        public BlackBerryBackupCallBackDelegate _BlackBerryBackupCallBack;

        public void BlackBerryImageDataCallBack(long imagedALLSize, string filename, ref int stop)
        {
            _BlackBerryBackupCallBack?.Invoke(imagedALLSize, filename, ref stop);
        }

        public void ImageDataCallBack(byte[] data, ref int stop)
        {
            _ImageDataCallBack?.Invoke(data, ref stop);
        }

        public void VivoBackupCallBack(long imagedALLSize, string filename, ref int stop)
        {
            _VivoBackupCallBack?.Invoke(imagedALLSize, filename, ref stop);
        }
    }
}
