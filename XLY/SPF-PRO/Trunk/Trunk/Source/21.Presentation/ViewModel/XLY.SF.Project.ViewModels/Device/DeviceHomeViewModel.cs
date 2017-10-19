using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.ViewModels.Device.DeviceHomeViewModel
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/17 19:47:00
* ==============================================================================*/

namespace XLY.SF.Project.ViewModels.Device
{
    /// <summary>
    /// 设备提取首页VM
    /// </summary>
    [Export(ExportKeys.DeviceHomeViewModel, typeof(ViewModelBase))]
    public class DeviceHomeViewModel : ViewModelBase
    {
        #region 属性

        #region 当前设备标题
        private string _deviceTitle = "设备名称：XXX-XX（已ROOT）（未连接）";

        /// <summary>
        /// 当前设备标题
        /// </summary>	
        public string DeviceTitle
        {
            get { return _deviceTitle; }
            set
            {
                _deviceTitle = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion
        public override void ViewClosed()
        {
            
        }
    }
}
