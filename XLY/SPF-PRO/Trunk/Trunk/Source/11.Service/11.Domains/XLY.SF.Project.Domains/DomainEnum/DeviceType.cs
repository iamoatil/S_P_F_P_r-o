using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{

    /// <summary>
    /// 介质类型
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// 镜像文件
        /// </summary>
        Mirror = 1,

        /// <summary>
        /// SDCard
        /// </summary>
        SDCard = 2,

        /// <summary>
        /// 磁盘
        /// </summary>
        Disk = 3,

        /// <summary>
        /// 默认设备
        /// </summary>
        Default = 4,

        /// <summary>
        /// cellbrite镜像文件
        /// </summary>
        Cellbrite = 5
    }
    
}
