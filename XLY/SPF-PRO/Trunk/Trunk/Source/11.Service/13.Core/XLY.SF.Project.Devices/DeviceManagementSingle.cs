using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/31 11:36:43
 * 类功能说明：
 * 1. 统一设备管理器
 *
 *************************************************/

namespace XLY.SF.Project.Devices
{
    /// <summary>
    /// 设备管理器
    /// </summary>
    public class DeviceManagement
    {
        #region 构造

        //[Obsolete("请使用DeviceManagementSingle.Instance获取实例！")]
        public DeviceManagement()
        {
            CurConnectDeves = new List<IDevice>();

            DeviceMonitors = new List<IDeviceMonitor>();
            DeviceMonitors.Add(new AndroidDeviceMonitor());
            DeviceMonitors.Add(new IOSDeviceMonitor());
        }

        ///// <summary>
        ///// 设备连接断开管理器实例
        ///// </summary>
        //public static DeviceManagementSingle Instance => SingleWrapperHelper<DeviceManagementSingle>.Instance;

        #endregion

        #region 属性

        /// <summary>
        /// 当前连接的设备
        /// </summary>
        public List<IDevice> CurConnectDeves { get; set; }

        /// <summary>
        /// 设备连接通知事件
        /// </summary>
        public event DelegateDeviceConnected DeviceConnected;

        /// <summary>
        /// 设备失去连接通知事件
        /// </summary>
        public event DelegateDeviceConnected DeviceDisconnected;

        private List<IDeviceMonitor> DeviceMonitors { get; set; }

        #endregion

        public void Start()
        {
            foreach (var dm in DeviceMonitors)
            {
                try
                {
                    dm.DeviceConnected = OnDeviceConnected;
                    dm.DeviceDisconnected = OnDeviceDisconnected;

                    if (!dm.Start())
                    {
                        LoggerManagerSingle.Instance.Warn("设备监控服务启动失败:" + dm.GetType().ToString());
                    }
                }
                catch (Exception e)
                {
                    LoggerManagerSingle.Instance.Error(e);
                }
            }
        }

        public void Stop()
        {
            foreach (var dm in DeviceMonitors)
            {
                try
                {
                    dm.Close();
                }
                catch (Exception e)
                {
                    LoggerManagerSingle.Instance.Error(e);
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnDeviceConnected(IDevice device)
        {
            CurConnectDeves.Add(device);
            DeviceConnected?.Invoke(device);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnDeviceDisconnected(IDevice device)
        {
            CurConnectDeves.Remove(device);
            DeviceDisconnected?.Invoke(device);
        }

    }
}
