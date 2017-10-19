using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 磁盘信息
    /// </summary>
    public class Disk
    {
        public Disk()
        {
            Partitions = new List<BPartition>();
            Files = new Collection<FileX>();
        }

        /// <summary>
        /// 是否是镜像文件转换成的磁盘（主要判断是否是原始数据本身就是磁盘）
        /// </summary>
        public bool IsMirror { get; set; }

        /// <summary>
        /// 当前磁盘打开的句柄
        /// </summary>
        public IntPtr OpenHandle { get; set; }

        /// <summary>
        /// 磁盘编号
        /// </summary>
        public byte DiskNumber { get; set; }

        /// <summary>
        /// 磁盘内部型号
        /// </summary>
        public string InternalModel { get; set; }

        /// <summary>
        /// 磁盘序列号
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// 磁盘名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 磁盘类型
        /// </summary>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 当前磁盘下所有文件信息(所有分区中的文件信息)
        /// </summary>
        public ICollection<FileX> Files { get; set; }

        /// <summary>
        /// 当前磁盘分区集合
        /// </summary>
        public List<BPartition> Partitions { get; set; }

        /// <summary>
        /// 当前磁盘分区总数
        /// </summary>
        public int PCount
        {
            get { return this.Partitions.Count; }
        }

        /// <summary>
        /// 当前磁盘分区总数[等效于PCount,只在芯片提取系统中使用]
        /// </summary>
        public int PartitionsCount
        {
            get;
            set;
        }

        /// <summary>
        /// 磁盘大小(所有分区大小的总和)
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// 磁盘大小描述
        /// </summary>
        public string SizeDesc
        {
            get
            {
                return Partition.GetFileSize((long)this.TotalSectors * 512);
            }
        }

        /// <summary>
        /// 分区偏移扇区,相对于整个磁盘
        /// </summary>
        public ulong SectorOffset { get; set; }

        /// <summary>
        /// 总扇区
        /// </summary>
        public ulong TotalSectors { get; set; }

        /// <summary>
        /// 是否是本地磁盘(当前电脑的硬盘设备)
        /// </summary>
        public bool IsLocalDisk { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Image
        {
            get { return string.Format("{0}/{1}", BPartition.GetPhysicalPath("icons"), "drive.png"); ; }
        }
    }
}
