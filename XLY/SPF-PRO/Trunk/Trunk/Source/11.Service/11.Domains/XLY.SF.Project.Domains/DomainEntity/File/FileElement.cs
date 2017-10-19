using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{

    #region 磁盘信息

    /// <summary>
    /// 磁盘信息
    /// 我们对于底层的Disk_INFO_EX
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DISK_INFO
    {
        /// <summary>
        /// 磁盘编号
        /// </summary>
        public byte disk_num;

        /// <summary>
        /// WORD :序列号,char SN[]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string sSerialNumber;

        /// <summary>
        /// WORD 23-26: 固件版本 len 8
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string sFirmwareRev;

        /// <summary>
        /// WORD 27-46: 内部型号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string sModelNumber; //WORD 27-46: 内部型号	

        /// <summary>
        /// 磁盘总扇区数
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string sectors;

        /// <summary>
        /// 分区个数
        /// </summary>
        public byte part_counts;

        /// <summary>
        /// IDE, USB
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string controller;

        /// <summary>
        /// 分区表信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public DISK_PARTITION_INFO[] pt;

    }

    #endregion

    #region 分区信息

    /// <summary>
    /// 分区信息
    /// DISK_PARTITION_INFO_EX
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DISK_PARTITION_INFO
    {
        /// <summary>
        /// 磁盘编号
        /// </summary>
        public Byte driver_number;

        /// <summary>
        /// 分区盘符
        /// </summary>
        public char driver_letter;

        /// <summary>
        /// 文件系统类型
        /// </summary>
        public Byte file_system;

        /// <summary>
        /// 分区偏移扇区, 相对于整个磁盘
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string start_lba;

        /// <summary>
        /// 分区扇区数
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string sectors;

        /// <summary>
        /// 空闲扇区数
        /// 空闲扇区数 * 512 = 字节数
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string free_sectors;

        /// <summary>
        /// 卷类型
        /// </summary>
        public uint vol_type;

        /// <summary>
        /// 动态卷的对象ID(ObjectId)
        /// </summary>
        public uint vol_object_id;

        /// <summary>
        /// 卷序列号
        /// </summary>
        public UInt32 serial_num;

        /// <summary>
        /// 卷名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string vol_name;

        /// <summary>
        /// 文件系统名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string fs_name;

    }

    #endregion

    #region 分区附加信息接口，“fun_20”获取分区附加信息，与“分区”结构有差异

    /// <summary>
    /// 磁盘分区信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DSK_VOLUME_INFO
    {
        /// <summary>
        /// 盘符
        /// </summary>
        public byte nDriver;

        /// <summary>
        /// 磁盘号
        /// </summary>
        public byte dsk_num;

        /// <summary>
        /// 文件簇中每个扇区的byte数
        /// </summary>
        public uint bytes_per_sec;

        /// <summary>
        /// 文件簇中每个簇的扇区数
        /// </summary>
        public uint secs_per_clust;

        /// <summary>
        /// 分区剩余文件簇数；
        /// </summary>
        public uint frees_clusts;

        /// <summary>
        /// 分区剩余文件簇数
        /// </summary>
        public uint total_clusts;

        /// <summary>
        /// 偏移扇区(相对于整个磁盘512字节/扇区)
        /// </summary>
        public ulong offset_sec;

        /// <summary>
        /// 总扇区数(512字节/扇区)
        /// </summary>
        public ulong total_secs;

        /// <summary>
        /// 卷序列号
        /// </summary>
        public uint serial_num;

        /// <summary>
        /// 卷名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string vol_name;

        /// <summary>
        /// 文件系统名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string fs_name;

    }

    #endregion

    #region 节点数据链表结构

    /// <summary>
    /// 节点数据链表结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LINK_DIR_FILE_NODE_INFO
    {
        /// <summary>
        /// 结点数据结构信息
        /// </summary>
        public DIR_FILE_NODE_INFO NodeDataInfo;

        /// <summary>
        /// 下一个文件节点数据链表;
        /// </summary>
        public IntPtr next;
    }

    #endregion

    #region 文件节点对象

    /// <summary>
    /// 文件节点数据结构信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DIR_FILE_NODE_INFO
    {
        /// <summary>
        /// 偏移扇区
        /// </summary>
        public UInt64 OffsetSec;

        /// <summary>
        /// 内容物理偏移字节，UI显示使用
        /// </summary>
        public UInt64 Physical_Offset_Bytes;

        /// <summary>
        /// 文件id号(对于raw文件存储的则是对应的扇区号) 
        /// </summary>
        public UInt64 FileId;

        /// <summary>
        /// 文件父id号(-1 - raw文件目录, -2 - 丢失文件)
        /// </summary>
        public UInt64 ParentFileId;

        /// <summary>
        /// 偏移字节
        /// 恢复文件使用
        /// </summary>
        public Int16 OffsetBytes;

        /// <summary>
        /// 文件大小
        /// </summary>
        public UInt64 Size;

        /// <summary>
        /// 占用大小，按簇计算
        /// </summary>
        public UInt64 PhysicalSize;

        /// <summary>
        /// 通道号 0xFF - 表示无效的通道号, 即不区分通道号
        /// </summary>
        public byte Channel;

        /// <summary>
        /// 文件属性（Raw=8，lost =10）
        /// </summary>
        public Int16 FileAttribute;

        /// <summary>
        /// 系统级别属性，只读、隐藏、临时等
        /// </summary>
        public Int32 SystemAttributeFlag;

        /// <summary>
        /// 创建时间/视频开始时间
        /// </summary>
        public Int32 CreateTime;

        /// <summary>
        /// 修改时间/视频结束时间
        /// </summary>
        public Int32 ModifyTime;

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public Int32 LastAccessTime;

        /// <summary>
        /// 恢复标记: 1 - 表示要恢复
        /// </summary>
        public byte Checked;

        /// <summary>
        /// 文件名长度
        /// </summary>
        public byte FileLength;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName;
    }

    #endregion

    #region 文件 恢复 结构

    /// <summary>
    /// 底层文件（恢复和打开）使用的特定结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FILE_RECOVERY_INFO
    {
        /// <summary>
        /// 偏移扇区
        /// </summary>
        public UInt64 OffsetSec;

        /// <summary>
        /// 文件id号
        /// </summary>
        public UInt64 FileId;

        /// <summary>
        /// 父id号
        /// </summary>
        public UInt64 ParentFileId;

        /// <summary>
        /// 文件属性
        /// </summary>
        public Int16 Attr;

        /// <summary>
        /// 偏移字节
        /// </summary>
        public Int16 OffSet;

        /// <summary>
        /// 文件大小
        /// </summary>
        public UInt64 Size;
    }

    #endregion

    #region 文件类型信息链表结构

    /// <summary>
    /// 文件类型信息链表结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LIST_FILE_RAW_TYPE_INFO
    {
        /// <summary>
        /// 是否选中
        /// </summary>
        public byte Check;

        /// <summary>
        /// 当前结点编号
        /// </summary>
        public UInt64 Num;

        /// <summary>
        /// raw文件类型信息结构
        /// </summary>
        public FILE_RAW_TYPE_INFO rt;

        /// <summary>
        /// 下一个文件链结构
        /// </summary>
        public IntPtr next;
    }

    #endregion

    #region raw文件类型信息结构

    /// <summary>
    /// raw文件类型信息结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FILE_RAW_TYPE_INFO
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string FileType;
        public Int32 Size;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string ExtensionName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string HeadMagic;

        public UInt16 HeadOffsetBytes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string FootMagic;
        public Int32 FootOffsetBytes;
        public Int32 HeadLen;
        public Int32 FootLen;
    }

    #endregion

    #region 底层扫描回调数据

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ScanCallBackData
    {
        public ulong pos_start; //开始位置
        public ulong pos_end; //结束位置
        public ulong pos_current; //进度当前值

        public uint folder_counts; //目录个数
        public uint normal_file_counts; //正常的视频文件个数
        public uint covering_file_counts; //覆盖的视频文件个数
        public uint lost_file_counts; //丢失的视频文件个数
    }

    #endregion

    #region 底层文件属性

    /// <summary>
    /// 底层 WORD attr 有关;
    /// 当attr 与 下面的值 “&”操作，等于枚举值，则说明是该类型等于 1 后的结论。
    /// 与attr &  Deleted == Deleted，则说明是正常的，否则为失败的
    /// 具体细则请见许工最新文档。
    /// </summary>
    [Flags]
    internal enum BottomAttributeFlag : short
    {
        Normal = 0X0001,
        Directory = 0X0002,
        Lost = 0X0004,
        Raw = 0X0008,
        PartCover = 0X0010,
        AllCover = 0X0020,
        NTFSCompress = 0X0040,
        NTFSEncrypt = 0X0080,
        TrueCrypt = 0X0100,
        Chain = 0X0200,
        ReBuild = 0X0400,
        ADS = 0X0800,
        INDX = 0X1000,
        TXFAndOther = 0X2000
    }

    #endregion

    #region 深度查找分区相关结构

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DSK_PART_TABLE
    {
        /// <summary>
        /// 结点数据结构信息
        /// </summary>
        public DSK_PART_INFO disk_part_info;

        /// <summary>
        /// 下一个文件节点数据链表;
        /// </summary>
        public IntPtr next;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DSK_PART_INFO
    {
        /// <summary>
        /// 磁盘编号
        /// </summary>
        public Byte driver_number;

        /// <summary>
        /// 分区盘符
        /// </summary>
        public char driver_letter;

        /// <summary>
        /// 文件系统类型
        /// </summary>
        public Byte file_system;

        /// <summary>
        /// 分区偏移扇区, 相对于整个磁盘
        /// </summary>
        public UInt64 start_lba;

        /// <summary>
        /// 分区扇区数
        /// </summary>
        public UInt64 sectors;

        /// <summary>
        /// 空闲扇区数
        /// </summary>
        public UInt64 freesectors;

        /// <summary>
        /// 卷类型
        /// </summary>
        public uint vol_type;

        /// <summary>
        /// 动态卷的对象ID(ObjectId)
        /// </summary>
        public uint vol_object_id;

        /// <summary>
        /// 卷序列号
        /// </summary>
        public UInt32 serial_num;

        /// <summary>
        /// 卷名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string vol_name;

        /// <summary>
        /// 文件系统名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string fs_name;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FIND_VOLUME_PROGRESS
    {
        public UInt64 start_sec;
        public UInt64 curr_sec;
        public UInt64 end_sec;

        /// <summary>
        /// 找到的分区个数
        /// </summary>
        public UInt32 vol_cnts;
    }

    #endregion

    #region 底层回调函数签名

    /// <summary>
    /// 扫描文件进度回调函数
    /// </summary>
    /// <returns></returns>
    public delegate void ScanCallBack(ref ScanCallBackData pdi);

    public delegate UInt32 FindVolumeCallBack(ref FIND_VOLUME_PROGRESS pdi);

    public delegate void LpfnRecoveryFileProgress(ref CALL_BACK_RECOVERY_CURRENT_FILE pcf);

    public delegate void LpfnCallbackDskQuickCopy(ref DSK_QUICK_COPY_CALLBACK_INFO pcai);

    #endregion

    #region 底层快照信息

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BLK_FDT_SNAPSHOT_HEAD
    {
        /// <summary>
        /// 0x00 设备类型
        /// </summary>
        public Byte dev_type;

        /// <summary>
        /// 0x01 文件系统类型
        /// </summary>
        public Byte fs_type;

        /// <summary>
        /// 0x03 每扇区字节数
        /// </summary>
        public UInt32 bytes_per_sec;

        /// <summary>
        /// 0x07 每簇块扇区数
        /// </summary>
        public UInt32 secs_per_blk;

        /// <summary>
        /// 0x0B 数据区开始扇区
        /// </summary>
        public UInt32 data_area_offset_sec;

        /// <summary>
        /// 0x0F 卷偏移扇区(SEC_SZ:512)
        /// </summary>
        public UInt64 vol_offset_sec;

        /// <summary>
        /// 0x17 卷总扇区数(SEC_SZ:512)	
        /// </summary>
        public UInt64 vol_total_secs;

        /// <summary>
        /// 0x1F 每簇块字节数
        /// </summary>
        public UInt32 bytes_per_blk;

        /// <summary>
        /// 0x23 卷开始簇块号 	
        /// </summary>
        public UInt64 vol_first_blk;

        /// <summary>
        /// 0x2B 总簇块数
        /// </summary>
        public UInt64 vol_total_blks;

        /// <summary>
        /// 0x33 最大文件目录id号
        /// </summary>
        public UInt64 max_id;

        /// <summary>
        /// 0x02 掩码长度(字节个数)
        /// </summary>
        public Byte mask_len;

        /// <summary>
        /// 0x3B 掩码值
        /// </summary>
        public UInt64 mask_value;

        /// <summary>
        /// 0x43 最后簇块掩码位
        /// </summary>
        public UInt64 mask_last_blk_bit;

        /// <summary>
        /// 0x4B 簇块数据区是否有效: 0 - 无效, 1 - 有效
        /// </summary>
        public Byte fdtFlag;

        /// <summary>
        /// 0x4C
        /// </summary>
        public UInt32 check_sum;

    }

    #endregion

    #region 位图数据结构

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DATA_BLOCK_NODE_INFO
    {
        /// <summary>
        /// 0x00 - 未使用(空闲区), 1 - 使用(有效区), 2 - 文件的有效区
        /// </summary>
        public byte bStatusFlag;

        /// <summary>
        /// 偏移扇区
        /// </summary>
        public UInt64 offset_sec;

        /// <summary>
        /// 扇区数
        /// </summary>
        public uint secs;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LINK_DATA_BLOCK_NODE_INFO
    {
        public DATA_BLOCK_NODE_INFO dni;
        public IntPtr next;

    }

    #endregion

    #region 时间

    //日期格式结构 : 7 byte
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DATE_FORMAT
    {
        public Int16 year; //0x00
        public Int16 month; //0x02
        public Int16 day; //0x03

        public Int16 hour; //0x04
        public Int16 minute; //0x05
        public Int16 second; //0x06
    }

    #endregion

    #region 阵列

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DEVICE_BASE_INFO
    {
        /// <summary>
        /// 设备句柄
        /// </summary>
        public IntPtr hDev;

        /// <summary>
        /// 设备类型
        /// </summary>
        public byte dev_type;

        /// <summary>
        /// 卷内偏移字节
        /// </summary>
        public UInt64 offset_bytes_in_vol;

        /// <summary>
        /// 设备偏移扇区
        /// </summary>
        public UInt64 offset_sec;

        /// <summary>
        /// 设备扇区个数
        /// </summary>
        public UInt64 secs;

        /// <summary>
        /// 设备字节数长度
        /// </summary>
        public UInt64 len;
    }

    /// <summary>
    /// RAID装载接口回调结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RAID_PROGRESS_INFO
    {
        public UInt64 pos_cur;
        public UInt64 pos_end;
    }

    /// <summary>
    /// 矩阵结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RAID_INFO_EX
    {
        /// <summary>
        /// 阵列类型
        /// 0x8000 - jbod
        /// 0x8001 - raid0
        /// 0x8002 - raid1 -不支持
        /// 0x8003 - raid2 -不支持
        /// 0x8004 - raid3 -不支持
        /// 0x8005 - raid4 -不支持
        /// 0x8006 - raid5
        /// 0x8007 - raid5e
        /// 0x8008 - raid5ee
        /// 0x8009 - raid6	-不支持				  
        /// </summary>
        public UInt32 raid_type;

        /// <summary>
        /// 磁盘个数（镜像文件个数）
        /// </summary>
        public UInt32 dsk_counts;

        /// <summary>
        /// 厂家类型
        /// 0x00 - Adaptec
        /// 0x01 - AMI Linux-Standard
        /// 0x02 - HP/Compaq
        /// 0x03 - Parity Dynamic (aka)
        /// </summary>
        public UInt32 manufacturer;

        /// <summary>
        /// 旋转方式
        /// 0x00 - 左同步
        /// 0x01 - 左异步
        /// 0x02 - 右同步
        /// 0x03 - 右异步
        /// </summary>
        public UInt32 rotate_mod;

        /// <summary>
        /// 条带大小
        /// </summary>
        public UInt32 stripe_sec;

        /// <summary>
        /// 用于HP/Compaq）必须是2的次方
        /// </summary>
        public UInt32 delay_size;

        /// <summary>
        /// 阵列总扇区数
        /// </summary>
        public UInt64 rd_total_secs;

        /// <summary>
        /// 阵列总字节数
        /// </summary>
        public UInt64 rd_total_bytes;

        public UInt64 rd_total_blocks; //阵列总数据块数 

        public UInt64 rd_total_stripes; //阵列总的条带个数(行数) 

        /// <summary>
        /// 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public Int32[] dsk_order;

        /// <summary>
        /// 设备基本信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public DEVICE_BASE_INFO[] dvi;

    }

    #endregion

    #region  HD1
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DSK_QUICK_COPY_CALLBACK_INFO
    {
        public char bEndFlag;//0 - 镜像未结束, 1 - 镜像结束
        public Int64 pos_start;
        public Int64 pos_end;
        public Int64 pos_cur;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] verify_value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DSK_IMAGE_PARAMETER_INFO
    {
        public IntPtr hSrc;
        public IntPtr hDes;
        public UInt64 src_offset_sec;
        public UInt64 des_offset_sec;
        public UInt64 sectors;
        public byte verify;//verify: 0 - 不校验，1 - md5校验， 2 - sha-1;
        public byte bClearSecFlag;
        public byte device_type_src;
        public byte device_type_des;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CALL_BACK_RECOVERY_CURRENT_FILE
    {
        public Int32 bInitFlag;
        public UInt64 curren_size;
        public UInt64 file_size;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CHECK_SUM_INFO
    {
        public char bFlag;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] barr_md5;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] barr_sha1;
    }

    #endregion

    #region 恢复文件回调
    /// <summary>
    /// 恢复文件回调
    /// </summary>
    /// <param name="pcf"></param>
    public delegate void LpfnRecoveryFileProgressEx(ref CALL_BACK_RECOVERY_CURRENT_FILE pcf);
    #endregion
}
