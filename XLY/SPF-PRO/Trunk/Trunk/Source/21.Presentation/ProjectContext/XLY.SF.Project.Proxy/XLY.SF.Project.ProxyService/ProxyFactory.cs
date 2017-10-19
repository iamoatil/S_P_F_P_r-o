using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ProxyService
{
    /// <summary>
    /// 服务代理工厂，创建所有底层服务。【享元模式】,线程安全
    /// </summary>
    public class ProxyFactory
    {
        #region Lock

        private static object _objLock = new object();

        #endregion

        #region 基础服务

        /// <summary>
        /// 基础设备服务【设备连接监听】
        /// </summary>
        private static volatile DeviceMonitorProxy _deviceMonitorProxy;

        public static DeviceMonitorProxy DeviceMonitor
        {
            get
            {
                if (_deviceMonitorProxy == null)
                {
                    lock (_objLock)
                    {
                        if (_deviceMonitorProxy == null)
                            _deviceMonitorProxy = new DeviceMonitorProxy();
                    }
                }
                return _deviceMonitorProxy;
            }
        }

        #endregion

        #region 初始加载



        #endregion
    }
}
