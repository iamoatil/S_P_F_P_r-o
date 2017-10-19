using System;
using System.Runtime.InteropServices;
using System.Text;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 设备连接回调委托
    /// </summary>
    /// <param name="devConnectState">设备连接状态：1为连接。2为断开</param>
    /// <param name="devInfo">设备信息</param>
    /// <returns>目前返回值没意义全返回0</returns>
    public delegate int DelegateUsbDeviceStateChange(int devConnectState, IntPtr devInfo);

    /// <summary>
    /// USB监听设备信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct UsbDeviceInfo
    {
        /// <summary>
        /// 是否为手机
        /// </summary>
        public bool IsPhone;

        public uint VID;

        public uint PID;

        /// <summary>
        /// 设备类型"Android", "iPad", "iPhone"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public byte[] BusReportedDeviceDesc;

        public string GetDeviceDesc()
        {
            if (BusReportedDeviceDesc != null)
            {
                return Encoding.Unicode.GetString(BusReportedDeviceDesc).Trim();
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取设备连接类型
        /// </summary>
        /// <returns></returns>
        public UsbDeviceType GetDeviceType()
        {
            string device = GetDeviceDesc().ToLower();
            if (device != string.Empty)
            {
                if (IsPhone)
                {
                    if (device.Contains("android"))
                    {
                        return UsbDeviceType.Android;
                    }
                    else if (device.Contains("iphone") || device.Contains("ipad"))
                    {
                        return UsbDeviceType.Ios;
                    }
                    else if (device.Contains("mtp"))
                    {
                        return UsbDeviceType.Mtp;
                    }
                }
                else
                {
                    if (device.Contains("flash disk"))
                    {
                        return UsbDeviceType.FlashDisk;
                    }
                }
            }

            return UsbDeviceType.Unknown;
        }
    }

    /// <summary>
    /// USB设备检测的设备类型
    /// </summary>
    public enum UsbDeviceType
    {
        Unknown,
        /// <summary>
        /// Android手机
        /// </summary>
        Android,
        /// <summary>
        /// IOS手机
        /// </summary>
        Ios,
        /// <summary>
        /// MTP设备
        /// </summary>
        Mtp,
        /// <summary>
        /// U盘
        /// </summary>
        FlashDisk,
    }
}
