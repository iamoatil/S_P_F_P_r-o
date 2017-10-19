using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ViewDomain.Model;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.ProxyService.DeviceMonitorProxy
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/17 15:42:58
* ==============================================================================*/

namespace XLY.SF.Project.ProxyService
{
    /// <summary>
    /// 设备监听服务
    /// </summary>
    public class DeviceMonitorProxy
    {
        /// <summary>
        /// 设备上下线
        /// </summary>
        public event DeviceConnectedDelegate OnDeviceConnected;

        #region 基础服务

        /// <summary>
        /// 设备管理器【设备连接】
        /// </summary>
        private DeviceManagement _devManager;

        #endregion

        internal DeviceMonitorProxy()
        {
            _devManager = new DeviceManagement();
            _devManager.DeviceConnected += this._devManager_DeviceConnected;
            _devManager.DeviceDisconnected += this._devManager_DeviceDisconnected;
        }

        private void _devManager_DeviceConnected(IDevice device)
        {
            OnDeviceConnected?.Invoke(device, true);
        }

        private void _devManager_DeviceDisconnected(IDevice device)
        {
            OnDeviceConnected?.Invoke(device, false);
        }

        #region 设备检测服务

        /// <summary>
        /// 打开设备检测服务
        /// </summary>
        public void OpenDeviceService()
        {
            _devManager.Start();
        }

        /// <summary>
        /// 关闭设备检测服务
        /// </summary>
        public void StopDeviceService()
        {
            _devManager.Stop();
        }

        #endregion

        #region 当前已连接设备

        /// <summary>
        /// 获取当前已连接设备
        /// </summary>
        public IDevice[] GetCurConnectedDevices()
        {
            return _devManager.CurConnectDeves.ToArray();
        }

        #endregion
    }
}
