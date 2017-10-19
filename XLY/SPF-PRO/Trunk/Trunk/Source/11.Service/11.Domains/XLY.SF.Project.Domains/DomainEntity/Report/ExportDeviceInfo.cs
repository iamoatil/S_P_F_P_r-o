using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.DeviceInfo
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/12 19:37:16
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 手机采集的设备信息（导出时）
    /// </summary>
    public class ExportDeviceInfo
    {
        /// <summary>
        /// 手机型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 手机名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 手机唯一ID，如IMEI
        /// </summary>
        public string IMEI { get; set; }
        /// <summary>
        /// Mac地址
        /// </summary>
        public string Mac { get; set; }
        /// <summary>
        /// 蓝牙Mac地址
        /// </summary>
        public string BloothMac { get; set; }
        /// <summary>
        /// 设备的制造商
        /// </summary>
        public string Manufacture { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 特征描述
        /// </summary>
        public string FeatureDescription { get; set; }
    }
}
