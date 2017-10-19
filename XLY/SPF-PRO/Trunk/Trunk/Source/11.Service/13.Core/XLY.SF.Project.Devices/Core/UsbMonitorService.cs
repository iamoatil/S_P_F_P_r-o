using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using X64Service;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices
{
    /// <summary>
    /// USB设备监听服务
    /// </summary>
    public sealed class UsbMonitorService
    {
        [Obsolete("请使用UsbMonitorService.Instance获取唯一实例")]
        public UsbMonitorService()
        {

        }

        private static readonly string UsbExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "USBMonitorService.exe");

        /// <summary>
        /// 唯一实例
        /// </summary>
        public static UsbMonitorService Instance => SingleWrapperHelper<UsbMonitorService>.Instance;

        /// <summary>
        /// USB设备状态改变事件
        /// </summary>
        public event Action<bool, UsbDeviceInfo> OnUsbDeviceStateChanged;

        /// <summary>
        /// USB设备连接
        /// </summary>
        public event Action OnUsbDeviceConnected;

        /// <summary>
        /// USB设备断开连接
        /// </summary>
        public event Action OnUsbDeviceDisConnected;

        private bool _serviceState = false;

        private DelegateUsbDeviceStateChange _callback;     //将委托定义为全局

        /// <summary>
        /// 启动服务
        /// </summary>
        public bool Start()
        {
            if (_serviceState)
                return true;
            int rt = USBMonitorCoreDll.Initialize(Encoding.Unicode.GetBytes(UsbExePath));
            if (rt != 0)
            {
                LoggerManagerSingle.Instance.Error($"初始化USB监听服务失败! 错误码：{rt}");
                return false;
            }

            _callback = new DelegateUsbDeviceStateChange(OnDeviceStateChanged); //将委托定义为全局，防止垃圾回收
            rt = USBMonitorCoreDll.Start(_callback);
            if (rt != 0)
            {
                LoggerManagerSingle.Instance.Error($"启动USB监听服务失败! 错误码：{rt}");
                return false;
            }
            _serviceState = true;
            return true;
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop()
        {
            if (_serviceState)
            {
                int rt = USBMonitorCoreDll.Stop();
                if (rt != 0)
                {
                    LoggerManagerSingle.Instance.Error($"停止USB监听服务失败! 错误码：{rt}");
                }
            }
        }

        private int OnDeviceStateChanged(int state, IntPtr devicePtr)
        {
            ThreadPool.QueueUserWorkItem(s =>
            {
                DealCallbackInThread((Tuple<int, IntPtr>)s);
            }, new Tuple<int, IntPtr>(state, devicePtr));
            return 0;
        }

        private void DealCallbackInThread(Tuple<int, IntPtr> arg)
        {
            int state = arg.Item1;
            IntPtr devicePtr = arg.Item2;

            if (state == 1)     //设备连接
            {
                var devInfo = (UsbDeviceInfo)Marshal.PtrToStructure(devicePtr, typeof(UsbDeviceInfo));

                OnUsbDeviceStateChanged?.Invoke(true, devInfo);
                OnUsbDeviceConnected?.Invoke();
                LoggerManagerSingle.Instance.Info($"设备连接：vid={devInfo.VID:X}, pid={devInfo.PID:X}, isPhone={devInfo.IsPhone}, desc={devInfo.GetDeviceDesc()}, type={devInfo.GetDeviceType()}");
            }
            else   //设备断开
            {
                var devInfoStr = Marshal.PtrToStringAnsi(devicePtr);
                var infoStr = devInfoStr.Split('#');
                UsbDeviceInfo devInfo = new UsbDeviceInfo();
                if (infoStr.Length > 2)
                {
                    string[] vpID;
                    if (GetVIDPID(infoStr[1], out vpID))
                    {
                        devInfo.VID = Convert.ToUInt32(vpID[0], 16);
                        devInfo.PID = Convert.ToUInt32(vpID[1], 16);
                    }

                    OnUsbDeviceStateChanged?.Invoke(false, devInfo);
                    OnUsbDeviceDisConnected?.Invoke();
                    LoggerManagerSingle.Instance.Info($"断开设备：vid={devInfo.VID:X}, pid={devInfo.PID:X}, str={devInfoStr}");
                }
            }
        }

        /// <summary>
        /// 获取VID,PID
        /// </summary>
        /// <param name="devInfoStr"></param>
        /// <param name="vpID">VID,PID</param>
        /// <returns></returns>
        private bool GetVIDPID(string devInfoStr, out string[] vpID)
        {
            vpID = new string[2];
            var a = devInfoStr.Split('&');
            if (a.Length == 2 && a[0].Contains("VID_") && a[1].Contains("PID_"))
            {
                vpID[0] = string.Concat(a[0].Skip(4));
                vpID[1] = string.Concat(a[1].Skip(4));

                return true;
            }
            return false;
        }

    }
}
