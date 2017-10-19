using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices
{
    /// <summary>
    /// 设备监控接口
    /// </summary>
    public interface IDeviceMonitor
    {
        /// <summary>
        /// 启动设备监控，设备相关初始化操作
        /// </summary>
        bool Start();

        /// <summary>
        /// 关闭设计监控，及相关清理工作
        /// </summary>
        void Close();

        /// <summary>
        /// 执行自我保护
        /// </summary>
        void DoSelfPretection();

        /// <summary>
        /// 设备连接通知事件
        /// </summary>
        DelegateDeviceConnected DeviceConnected { get; set; }

        /// <summary>
        /// 设备失去连接通知事件
        /// </summary>
        DelegateDeviceConnected DeviceDisconnected { get; set; }

    }

    public delegate void DelegateDeviceConnected(IDevice device);

}
