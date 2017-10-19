using System.Runtime.CompilerServices;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices
{
    /// <summary>
    /// 设备监控抽象类
    /// </summary>
    public abstract class AbstractDeviceMonitor : IDeviceMonitor
    {
        /// <summary>
        /// 设备连接通知事件
        /// </summary>
        public DelegateDeviceConnected DeviceConnected { get; set; }

        /// <summary>
        /// 设备连接通知事件
        /// </summary>
        public DelegateDeviceConnected DeviceDisconnected { get; set; }

        /// <summary>
        /// 启动设备监控，设备相关初始化操作
        /// </summary>
        public abstract bool Start();

        /// <summary>
        /// 关闭设备监控，及相关清理工作
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 执行自我保护
        /// </summary>
        public virtual void DoSelfPretection()
        {
            //do nothing;
        }

        /// <summary>
        /// 设备连接处理方法。
        /// </summary>
        /// <param name="device">设备对象。</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void OnConnected(IDevice device)
        {
            DeviceConnected?.Invoke(device);
        }

        /// <summary>
        /// 设备断开通知处理方法。
        /// </summary>
        /// <param name="device">设备对象。</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void OnDisconnected(IDevice device)
        {
            DeviceDisconnected?.Invoke(device);
        }
    }
}
