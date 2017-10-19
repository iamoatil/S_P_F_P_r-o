using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/16 17:50:41
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.Domains
{
    public enum EnumDevice
    {
        AndroidDevice,
        AndroidMirror,
        IOSDevice,
        SDCard,
        SIM,
    }

    /// <summary>
    /// 介质类型
    /// </summary>
    public enum MediumType
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
    /// <summary>
    /// 镜像标记
    /// </summary>
    public enum MirrorFlag
    {
        [Description("NewMirror")]
        NewMirror = 1,

        [Description("ContinueMirror")]
        ContinueMirror = 2
    }

}
