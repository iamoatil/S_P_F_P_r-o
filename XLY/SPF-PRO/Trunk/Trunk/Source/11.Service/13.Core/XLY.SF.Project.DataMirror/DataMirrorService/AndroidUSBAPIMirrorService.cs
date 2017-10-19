using DllClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataMirror
{
    /// <summary>
    /// 安卓镜像服务
    /// </summary>
    internal class AndroidUSBAPIMirrorService : AbstractMirrorService
    {
        /// <summary>
        /// 当前镜像进度
        /// </summary>
        private Int64 CurSchedule { get; set; }

        /// <summary>
        /// 镜像数据总大小
        /// </summary>
        private Int64 TotalSize { get; set; }

        /// <summary>
        /// 是否停止镜像
        /// </summary>
        private bool IsStop { get; set; }

        /// <summary>
        /// 镜像文件流
        /// </summary>
        private FileStream MirrorStream { get; set; }

        /// <summary>
        /// 镜像异步通知
        /// </summary>
        private IAsyncProgress MirrorAsyn { get; set; }

        /// <summary>
        /// 镜像源
        /// </summary>
        private Mirror Source { get; set; }

        public override void Execute(Mirror mirror, IAsyncProgress asyn)
        {
            try
            {
                Source = mirror;
                TotalSize = Source.Block.Size;
                MirrorAsyn = asyn;

                MirrorAsyn.Start(TotalSize);

                var device = Source.Source as Device;

                var service = X86DLLClientSingle.Instance.AndroidMirrorAPIChannel;

                var openHandle = service.AndroidMirror_OpenDevice(device.ID);
                if (0 == openHandle)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("安卓手机镜像出错！OpenDevice失败，设备ID:{0}", device.ID));
                    MirrorAsyn.IsSuccess = false;
                    MirrorAsyn.Stop();
                    return;
                }

                var result = service.AndroidMirror_Initialize(openHandle, 61440, 0);
                if (0 != result)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("安卓手机镜像出错！Initialize失败，设备ID:{0} 错误码:{1}", device.ID, result));
                    MirrorAsyn.IsSuccess = false;
                    MirrorAsyn.Stop();
                    return;
                }

                CurSchedule = 0;
                IsStop = false;
                Source.Local = FileHelper.ConnectPath(Source.Target, Source.TargetFile);
                FileHelper.CreateFileDirectory(Source.Local);
                MirrorStream = new FileStream(Source.Local, FileMode.Create);

                X86DLLClientSingle.Instance.ClientCallback._ImageDataCallBack += ImageDataCallBack;

                result = service.AndroidMirror_ImageDataZone(openHandle, Source.Block.Block, 0, -1);
                if (0 != result)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("安卓手机镜像出错！ImageDataZone失败，设备ID:{0} 错误码:{1}", device.ID, result));
                    MirrorAsyn.IsSuccess = false;
                    MirrorAsyn.Stop();
                    return;
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error("安卓手机镜像出错！", ex);
                MirrorAsyn.IsSuccess = false;
                MirrorAsyn.Stop();
            }
            finally
            {
                X86DLLClientSingle.Instance.ClientCallback._ImageDataCallBack -= ImageDataCallBack;
                MirrorStream?.Flush();
                MirrorStream?.Close();
                MirrorStream = null;
                MirrorAsyn = null;
            }
        }

        public override void Stop(IAsyncProgress asyn)
        {
            IsStop = true;
        }

        public override bool EnableSuspend => true;

        public override void Suspend(IAsyncProgress asyn)
        {
            IsStop = true;
        }

        public override void Continue(IAsyncProgress asyn)
        {
            try
            {
                TotalSize = Source.Block.Size;
                MirrorAsyn = asyn;

                MirrorAsyn.Start(TotalSize);

                var device = Source.Source as Device;

                var service = X86DLLClientSingle.Instance.AndroidMirrorAPIChannel;

                var openHandle = service.AndroidMirror_OpenDevice(device.ID);
                if (0 == openHandle)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("安卓手机镜像出错！OpenDevice失败，设备ID:{0}", device.ID));
                    MirrorAsyn.IsSuccess = false;
                    MirrorAsyn.Stop();
                    return;
                }

                var result = service.AndroidMirror_Initialize(openHandle, 61440, 0);
                if (0 != result)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("安卓手机镜像出错！Initialize失败，设备ID:{0} 错误码:{1}", device.ID, result));
                    MirrorAsyn.IsSuccess = false;
                    MirrorAsyn.Stop();
                    return;
                }

                CurSchedule = 0;
                IsStop = false;
                MirrorStream = new FileStream(Source.Local, FileMode.Append, FileAccess.Write);

                X86DLLClientSingle.Instance.ClientCallback._ImageDataCallBack += ImageDataCallBack;

                result = service.AndroidMirror_ImageDataZone(openHandle, Source.Block.Block, CurSchedule, -1);
                if (0 != result)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("安卓手机镜像出错！ImageDataZone失败，设备ID:{0} 错误码:{1}", device.ID, result));
                    MirrorAsyn.IsSuccess = false;
                    MirrorAsyn.Stop();
                    return;
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error("安卓手机镜像出错！", ex);
                MirrorAsyn.IsSuccess = false;
                MirrorAsyn.Stop();
            }
            finally
            {
                X86DLLClientSingle.Instance.ClientCallback._ImageDataCallBack -= ImageDataCallBack;
                MirrorStream?.Flush();
                MirrorStream?.Close();
                MirrorStream = null;
                MirrorAsyn = null;
            }
        }

        private void ImageDataCallBack(byte[] data, ref int stop)
        {
            try
            {
                MirrorStream.Write(data, 0, data.Length);

                TotalSize += data.Length;

                MirrorAsyn.Advance(data.Length);

                if (IsStop)
                {
                    stop = 1;
                }
            }
            catch (Exception ex)
            {
                stop = 1;

                LoggerManagerSingle.Instance.Error("安卓手机镜像ImageDataCallBack出错！", ex);
            }
        }

    }
}
