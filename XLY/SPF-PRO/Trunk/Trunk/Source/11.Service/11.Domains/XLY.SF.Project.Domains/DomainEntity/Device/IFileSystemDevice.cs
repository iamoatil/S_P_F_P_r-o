using System;
using System.Collections;

namespace XLY.SF.Project.Domains
{
    public interface IFileSystemDevice
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 当前设备对象
        /// </summary>
        object Source { get; set; }

        /// <summary>
        /// 当前设备打开句柄
        /// </summary>
        IntPtr Handle { get; set; }

        /// <summary>
        /// 当前装载设备句柄
        /// </summary>
        IntPtr Handle_Flsh { get; set; }

        /// <summary>
        /// 获取磁盘分区
        /// </summary>
        ArrayList Parts { get; }

        /// <summary>
        /// 扫描模式
        /// </summary>
        byte ScanModel { get; set; }

        /// <summary>
        /// 磁盘编号
        /// </summary>
        byte DiskNumber { get; set; }

        /// <summary>
        /// 设备大小
        /// </summary>
        ulong Size { get; set; }

        /// <summary>
        /// 扇区数
        /// </summary>
        ulong TotalSectors { get; }

        /// <summary>
        /// 是否有分区
        /// </summary>
        bool HasPartition { get; }

        /// <summary>
        /// 芯片类型：
        /// 0xA000 联发科手机芯片(mtk)
        /// 0xA001 展讯手机芯片
        /// 0xA002 诺机亚手机芯片
        /// 0xA003 WindowsPhone手机芯片
        /// 0xA004 晨星芯片(MStar)
        /// </summary>
        FlshType FlshType { get; set; }

        /// <summary>
        /// 设备类型
        /// 0x11 - 普通设备: 如:磁盘、分区、镜像文件
        /// 0x12 - 阵列
        /// 0x13 - 动态卷
        /// 0x14 - 芯片(NAND FLASH)
        /// 0x15 - 虚拟磁盘
        /// 0x16 - E01文件
        /// 0x17 - AFF文件
        /// 0x18 - ssd硬盘	
        /// 0x19 - 光盘
        /// 0x1A - dd镜像文件
        /// 0x1B - 手机芯片( 联发科手机芯片(mtk), 展讯手机芯片,...
        /// 0x80 - 效率源USB3.0设备
        /// </summary>
        DevType DevType { get; set; }
    }
}
