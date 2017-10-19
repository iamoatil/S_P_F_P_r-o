/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/8 9:59:29 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 设备接口
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        EnumDeviceType DeviceType { get; set; }
    }
}
