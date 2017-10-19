using System;
using System.Runtime.InteropServices;
using System.Text;
using XLY.SF.Project.Domains;

namespace X64Service
{
    /// <summary>
    /// USB监控服务
    /// </summary>
    public static class USBMonitorCoreDll
    {
        private const string _USBDeviceMonitor = @"bin\UsbMonitor.dll";

        #region USB端口检测DLL封装

        /// <summary>
        /// 初始化监听程序
        /// </summary>
        /// <param name="path">监听可执行程序路径</param>
        /// <returns></returns>
        [DllImport(_USBDeviceMonitor, EntryPoint = "fun_0_0")]
        public extern static int Initialize(byte[] path);

        /// <summary> 
        /// 开始监听
        /// </summary>
        /// <param name="callback">设备状态回调</param>
        /// <returns></returns>
        [DllImport(_USBDeviceMonitor, EntryPoint = "fun_0_2")]
        public extern static int Start(DelegateUsbDeviceStateChange callback);

        /// <summary>
        /// 停止监听
        /// </summary>
        /// <returns></returns>
        [DllImport(_USBDeviceMonitor, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fun_0_3")]
        public extern static int Stop();

        #endregion
    }

}
