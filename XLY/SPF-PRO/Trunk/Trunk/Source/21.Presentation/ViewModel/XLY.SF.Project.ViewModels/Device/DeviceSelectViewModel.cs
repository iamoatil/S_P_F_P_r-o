using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ProxyService;
using XLY.SF.Project.ViewDomain.MefKeys;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.ViewModels.Device.DeviceSelectViewModel
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/17 14:52:21
* ==============================================================================*/

namespace XLY.SF.Project.ViewModels.Device
{
    /// <summary>
    /// 数据源选择界面VM
    /// </summary>
    [Export(ExportKeys.DeviceSelectViewModel, typeof(ViewModelBase))]
    public class DeviceSelectViewModel : ViewModelBase
    {
        #region 属性

        #region 设备列表
        private ObservableCollection<IDevice> _devices = new ObservableCollection<IDevice>();

        /// <summary>
        /// 设备列表
        /// </summary>	
        public ObservableCollection<IDevice> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #endregion

        #region 事件

        public override void ViewClosed()
        {
            ProxyFactory.DeviceMonitor.OnDeviceConnected -= DeviceMonitor_OnDeviceConnected;
        }

        protected override void LoadCore(object parameters)
        {
            Devices.Clear();
            Devices.AddRange(ProxyFactory.DeviceMonitor.GetCurConnectedDevices());
            ProxyFactory.DeviceMonitor.OnDeviceConnected += DeviceMonitor_OnDeviceConnected;
        }

        /// <summary>
        /// 设备状态改变事件
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="isOnline"></param>
        private void DeviceMonitor_OnDeviceConnected(Domains.IDevice dev, bool isOnline)
        {
            SystemContext.Instance.AsyncOperation.Post(t =>
            {
                if (isOnline)
                {
                    Devices.Add(dev);
                }
                else
                {
                    Devices.Remove(dev);
                }
            }, null);
        }
        #endregion

    }
}
