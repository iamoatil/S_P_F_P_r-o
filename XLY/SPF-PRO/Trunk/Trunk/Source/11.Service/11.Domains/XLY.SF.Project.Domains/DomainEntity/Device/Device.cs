using System;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 设备信息领域对象
    /// </summary>
    [Serializable]
    public partial class Device
    {
        [NonSerialized]
        public IDeviceManager DeviceManager;
        //public HTCUSBINFO HtcNoUsbInfo = null;

        [NonSerialized]
        /// <summary>
        /// 手机断开事件
        /// </summary>
        public Action OnDisconnect = null;

        /// <summary>
        /// 设备信息领域对象-构造函数，必须指定IDeviceManager接口实例
        /// </summary>
        public Device(string number, IDeviceManager deviceManager)
        {
            OSType = EnumOSType.None;
            Status = EnumDeviceStatus.Offline;
            ID = number;
            SerialNumber = number;
            DeviceManager = deviceManager;
        }

        public bool IsHtcPhone()
        {
            if (Status == EnumDeviceStatus.Recovery || RealStatus == EnumDeviceStatus.Recovery)
            {
                return false;
            }

            string[] SerialNumberinfo = SerialNumber.ToLower().Split('&');
            if (SerialNumberinfo.Length < 2) return false;
            if (SerialNumberinfo[0] == "vid_0bb4" || SerialNumberinfo[0] == "vid_18d1") return true;
            return false;
        }
    }
}
