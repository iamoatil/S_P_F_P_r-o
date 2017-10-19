using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 蓝牙信息
    /// </summary>
    [Serializable]
    public class Bluetooth : AbstractDataItem
    {
        /// <summary>
        /// 本机地址
        /// </summary>
        [Display]
        public string LocalAddress { get; set; }

        /// <summary>
        /// 目标地址
        /// </summary>
        [Display]
        public string TargetAddress { get; set; }

        /// <summary>
        /// 目标名称
        /// </summary>
        [Display]
        public string TargetName { get; set; }

        /// <summary>
        /// 最后连接时间
        /// </summary>
        [Display(Alignment=EnumAlignment.Center)]
        public string LastConnectDateTime { get; set; }

    }
}
