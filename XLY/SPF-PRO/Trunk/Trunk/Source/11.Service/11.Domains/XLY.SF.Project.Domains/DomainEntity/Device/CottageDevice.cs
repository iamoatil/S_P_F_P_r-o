using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 山寨机 - 镜像设备类
    /// 手机芯片类型：MTK、WP、MStar、WindowsMobile、WebOS、Bada、Brew、Infineon、CoolSand、ADI等
    /// @Author 罗超
    /// @Date 20160909
    /// @Copy XLY
    /// </summary>
    public class CottageDevice : IFileSystemDevice
    {
        #region Properties
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 当前设备对象
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// 当前设备打开句柄
        /// </summary>
        public IntPtr Handle { get; set; }

        private ArrayList _parts;
        /// <summary>
        /// 磁盘分区
        /// </summary>
        public ArrayList Parts
        {
            get { return _parts ?? (_parts = new ArrayList()); }
        }

        /// <summary>
        /// 扫描模式
        /// </summary>
        public byte ScanModel { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public byte DiskNumber { get; set; }

        /// <summary>
        /// 设备大小
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// 扇区数
        /// </summary>
        public ulong TotalSectors { get { return this.Size > 0 ? this.Size / 512 : 0; } }

        /// <summary>
        /// 是否存在分区
        /// </summary>
        public bool HasPartition
        {
            get { return this.Parts.Count != 0; }
        }

        /// <summary>
        /// 芯片类型：
        /// 0xA000 联发科手机芯片(mtk)
        /// 0xA001 展讯手机芯片
        /// 0xA002 诺机亚手机芯片
        /// 0xA003 WindowsPhone手机芯片
        /// 0xA004 晨星芯片(MStar)
        /// </summary>
        public FlshType FlshType { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DevType DevType { get; set; }

        /// <summary>
        /// 当前装载设备句柄
        /// </summary>
        public IntPtr Handle_Flsh { get; set; }

        #endregion
    }
}
