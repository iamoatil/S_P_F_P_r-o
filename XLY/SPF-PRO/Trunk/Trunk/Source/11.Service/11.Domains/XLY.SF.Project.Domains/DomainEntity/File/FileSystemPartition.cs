using System;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    public class FileSystemPartition
    {
        public FileSystemPartition()
        {
            this.DevType = 0x11;
            this.PartType = 0;
        }

        /// <summary>
        /// 分区名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 当前分区装载的句柄
        /// </summary>
        public IntPtr Mount { get; set; }

        /// <summary>
        /// 分区快照句柄
        /// </summary>
        public IntPtr SnapShotHandle { get; set; }

        /// <summary>
        /// 文件系统类型
        /// </summary>
        public byte FileSystem { get; set; }

        /// <summary>
        /// 分区卷名
        /// </summary>
        public string VolName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public byte DevType { get; set; }

        /// <summary>
        /// 分区类型(0:正常分区,1:raw分区)
        /// </summary>
        public byte PartType { get; set; }

        /// <summary>
        /// 快照文件
        /// </summary>
        public string SnapshotFile { get; set; }

        /// <summary>
        /// 分区描述
        /// </summary>
        public string Discription { get; set; }

        /// <summary>
        /// 分区偏移扇区,相对于整个磁盘
        /// </summary>
        public ulong SectorOffset { get; set; }

        /// <summary>
        /// 总扇区
        /// </summary>
        public ulong TotalSectors { get; set; }

        /// <summary>
        /// 序列号标识
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// 判断当前分区是否有Mount句柄
        /// </summary>
        public bool HasMount
        {
            get { return this.Mount != IntPtr.Zero; }
        }

        /// <summary>
        /// 分区大小
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// 扫描出来的节点链表
        /// </summary>
        public LINK_DIR_FILE_NODE_INFO NodeLinkList { get; set; }
    }
}
