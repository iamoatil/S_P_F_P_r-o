/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/14 16:10:38 
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
    /// SIM卡
    /// </summary>
    public class SIMCardDevice : IDevice, IEquatable<SIMCardDevice>
    {
        public SIMCardDevice()
        {
            DeviceType = EnumDeviceType.SIM;
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        public EnumDeviceType DeviceType { get; set; }

        /// <summary>
        /// COM口编号
        /// </summary>
        public string ComNumStr { get; set; }

        public override int GetHashCode()
        {
            return ComNumStr.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SIMCardDevice);
        }

        public bool Equals(SIMCardDevice other)
        {
            if (null == other)
            {
                return false;
            }
            else
            {
                return ComNumStr == other.ComNumStr;
            }
        }

    }
}
