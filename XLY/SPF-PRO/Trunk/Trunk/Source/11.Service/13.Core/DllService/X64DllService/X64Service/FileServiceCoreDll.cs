using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace X64Service
{
    public class FileServiceCoreDll
    {
        #region DLL名称配置

        private const string _Hd = @"bin\hd.dll";

        private const string _Hd0 = @"bin\hd0.dll";

        private const string _DskFsMaster = @"bin\dsk_fs_master.dll";

        private const string _FileTypTPL = @"bin\file_type_tpl.dll";

        private const string _SnapShotDLL = @"bin\blk_fdt_snapshot.dll";

        private const string _TimeConverter = @"bin\xtime_convert.dll";

        private const string _RaidEx = @"bin\rdEx.dll";

        private const string _diskDll_hd1 = @"bin\hd1.dll";

        private const string _dev_flsh_krnl = @"bin\dev_flsh_krnl.dll";

        #endregion

        #region hd.dll
        /// <summary>
        /// 获取磁盘的基本信息
        /// </summary>
        /// <param name="pdsk">磁盘信息</param>
        /// <param name="dsk_num">磁盘的编号</param>
        /// <returns>标识：0，成功</returns>
        [DllImport(_Hd, EntryPoint = "fun_0")]
        public static extern UInt32 GetDiskInfo(ref DISK_INFO pdsk, byte dsk_num);

        /// <summary>
        /// 打开磁盘
        /// </summary>
        /// <param name="dskNum">磁盘编号</param>
        /// <returns>磁盘句柄</returns>
        [DllImport(_Hd, EntryPoint = "fun_1")]
        public static extern IntPtr OpenDevice(Byte dskNum);

        /// <summary>
        /// 打开文件
        /// 镜像文件
        /// </summary>
        /// <param name="szName">文件的物理路径 </param>
        /// <returns>文件句柄</returns>
        [DllImport(_Hd, EntryPoint = "fun_2")]
        public static extern IntPtr OpenFile(string szName);

        /// <summary>
        /// 获取镜像文件的分区信息
        /// </summary>
        /// <param name="partTable">分区表</param>
        /// <param name="mirrorFilePath">镜像文件</param>
        /// <returns>返回0，代表解析成功。</returns>
        [DllImport(_Hd, EntryPoint = "fun_15")]
        public static extern int GetMirrorFilePartitions(ref DSK_PART_TABLE partTable, string mirrorFilePath);

        /// <summary>
        /// 卸载（释放）分区链表资源。
        /// </summary>
        /// <param name="partTable">链表对象。</param>
        [DllImport(_Hd, EntryPoint = "fun_16")]
        public static extern void FreePartitionTableHandle(ref DSK_PART_TABLE partTable);

        /// <summary>
        /// 关闭设备句柄
        /// </summary>
        /// <param name="deviceHandle"></param>
        [DllImport(_Hd, EntryPoint = "fun_17")]
        public static extern void CloseDevice(IntPtr deviceHandle);

        /// <summary>
        /// 打开卷(分区)
        /// </summary>
        /// <param name="nDrive">分区盘符</param>
        /// <returns>分区句柄</returns>
        [DllImport(_Hd, EntryPoint = "fun_19")]
        public static extern IntPtr OpenPartition(char nDrive);

        /// <summary>
        /// 获取分区信息
        /// </summary>
        /// <param name="partitionLetter">分区盘符</param>
        /// <param name="diskVolumeInfo">分区信息</param>
        [DllImport(_Hd, EntryPoint = "fun_20")]
        public static extern uint GetPartitionExInfo(char partitionLetter, ref DSK_VOLUME_INFO diskVolumeInfo);

        /// <summary>
        /// 获取设备的扇区数
        /// </summary>
        [DllImport(_Hd, EntryPoint = "fun_21")]
        public static extern UInt32 GetDiskSectors(byte diskNum, ref ulong sectors);
        #endregion

        #region dsk_fs_master.dll
        /// <summary>
        /// 装载分区
        /// </summary>
        /// <param name="hSnapshot">簇块文件目录快照文件句柄</param>
        /// <param name="deviceHaldle">磁盘句柄</param>
        /// <param name="partitionOffset">分区开始扇区(偏移量)</param>
        /// <param name="partitionTotalSectors">分区总扇区数</param>
        /// <param name="fileSystemType">文件系统分区类型（NTFS,Fat32,Ext2...）</param>
        /// <param name="mountPartitionFlag">分区标记: 0 - 正常分区, 1 - raw分区</param>
        /// <param name="deviceType">物理设备类型</param>
        /// <returns>分区句柄</returns>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_A")]
        public static extern IntPtr MountPartition(
            IntPtr hSnapshot,
            IntPtr deviceHaldle,
            UInt64 partitionOffset,
            UInt64 partitionTotalSectors,
            byte fileSystemType,
            byte mountPartitionFlag = 0,
            byte deviceType = 4);

        /// <summary>
        /// 许工底层从磁盘中扫描文件
        /// </summary>
        /// <param name="mountHandle">分区mount成功后返回的句柄</param>
        /// <param name="lpfnprgs">扫描过程中回调函数句柄</param>
        /// <param name="fileLinks">底层可返回的文件链对象</param>
        /// <param name="scanModel">底层扫描模式（快速、Raw..）</param>
        /// <param name="rawList">Raw结构链</param>
        /// <param name="SnapFlag">创建快照标志: 0 - 不保存, 1 - 保存</param>
        /// <returns>返回成功与否的标识，0 -> Success</returns>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_1")]
        public static extern Int32 ScanFiles(IntPtr mountHandle, ScanCallBack lpfnprgs, ref LINK_DIR_FILE_NODE_INFO fileLinks, byte scanModel, ref LIST_FILE_RAW_TYPE_INFO rawList, int SnapFlag);

        /// <summary>
        /// 释放目录文件节点链表空间(不会释放自己定义的顶层节点空间)
        /// </summary>
        /// <param name="mountHandle">分区mount成功后返回的句柄</param>
        /// <param name="fileLinks">底层可返回的文件链对象</param>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_2")]
        public static extern Int32 DisposeLinkTableRoom(IntPtr mountHandle, ref LINK_DIR_FILE_NODE_INFO fileLinks);

        /// <summary>
        /// 停止(停止文件扫描、文件恢复)
        /// </summary>
        /// <param name="mountHandle">装载的分区句柄</param>
        /// <param name="nStatus">nStatus - 状态: 1 - 执行, 2 - 暂停, 3 - 停止</param>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_3")]
        public static extern void Stop(IntPtr mountHandle, Int32 nStatus = 3);

        /// <summary>
        /// 总是创建一个新的文件
        /// </summary>
        /// <param name="szName">文件全路径</param>
        /// <returns>文件句柄</returns>
        [DllImport(_Hd, EntryPoint = "fun_3")]
        public static extern IntPtr CreateNewFile(string szName);

        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="mountHandle">装载的分区句柄</param>
        /// <param name="newFileHandle">新建文件句柄</param>
        /// <param name="callback">文件恢复进度回调函数</param>
        /// <param name="recoveryFileInfo">被恢复文件结构</param>
        /// <param name="cherckSumInfo">恢复文件校验信息结构指针</param>
        /// <returns></returns>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_4")]
        public static extern Int32 RecoveryFile(IntPtr mountHandle, IntPtr newFileHandle, LpfnRecoveryFileProgressEx callback, ref FILE_RECOVERY_INFO recoveryFileInfo, ref CHECK_SUM_INFO cherckSumInfo);

        /// <summary>
        /// 通过底层读取文件内容
        /// </summary>
        /// <param name="mountHandle">分区句柄</param>
        /// <param name="recoveryFileInfo">文件信息结构指针</param>
        /// <param name="offset_bytes">偏移字节</param>
        /// <param name="length">读取长度</param>
        /// <param name="buffer">输出数据缓冲区指针</param>
        /// <returns>0 - 成功，其他数字-错误</returns>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_5")]
        public static extern UInt32 ReadFileByBottom(IntPtr mountHandle, ref FILE_RECOVERY_INFO recoveryFileInfo, UInt64 offset_bytes, UInt32 length, byte[] buffer);

        /// <summary>
        /// 通过底层读取文件内容
        /// </summary>
        /// <param name="mountHandle">分区句柄</param>
        /// <param name="recoveryFileInfo">文件信息结构指针</param>
        /// <param name="offset_bytes">偏移字节</param>
        /// <param name="length">读取长度</param>
        /// <param name="buffer">输出数据缓冲区指针</param>
        /// <returns>0 - 成功，其他数字-错误</returns>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_5")]
        public static extern Int32 ReadFileByBottom(IntPtr mountHandle, ref FILE_RECOVERY_INFO recoveryFileInfo, UInt64 offset_bytes, Int32 length, IntPtr buffer);


        /// <summary>
        /// 通过底层读取文件内容
        /// </summary>
        /// <param name="mountHandle">分区句柄</param>
        /// <param name="recoveryFileInfo">文件信息结构指针</param>
        /// <param name="offset_bytes">偏移字节</param>
        /// <param name="length">读取长度</param>
        /// <param name="buffer">输出数据缓冲区指针</param>
        /// <returns>0 - 成功，其他数字-错误</returns>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_5")]
        public static extern Int32 ReadFileByBottom(IntPtr mountHandle, ref FILE_RECOVERY_INFO recoveryFileInfo, UInt64 offset_bytes, Int32 length, byte[] buffer);

        /// <summary>
        /// 卸载分区
        /// </summary>
        /// <param name="mountHandle">装载分区的句柄</param>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_6")]
        public static extern void UnmountPartitionHandle(ref IntPtr mountHandle);

        /// <summary>
        /// 获取位图信息
        /// </summary>
        /// <param name="mountHandle">装载分区的句柄</param>
        /// <param name="plnk">位图信息链表 </param>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_7")]
        public static extern Int32 GetBitMap(IntPtr mountHandle, ref LINK_DATA_BLOCK_NODE_INFO plnk);

        /// <summary>
        /// 释放位图信息链表
        /// </summary>
        /// <param name="mountHandle">装载分区的句柄</param>
        /// <param name="plnk">位图信息链表 </param>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_8")]
        public static extern void DisposeBitMap(IntPtr mountHandle, ref LINK_DATA_BLOCK_NODE_INFO plnk);

        /// <summary>
        /// 读取簇块数据
        /// </summary>
        /// <param name="hMountVolume">装载分区的句柄</param>
        /// <param name="pBuf">数据缓冲区</param>
        /// <param name="offset_blk">偏移簇块号</param>
        /// <param name="blocks">偏移簇块号</param>
        /// <param name="bytes_per_blk">簇块大小</param>
        /// <param name="offset_blk_addr">偏移簇块地址</param>
        /// <returns></returns>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_11")]
        public static extern UInt32 ReadBlock(
             IntPtr hMountVolume,
              IntPtr pBuf,
             UInt64 offset_blk,
             UInt32 blocks,
             UInt32 bytes_per_blk,
             ref UInt64 offset_blk_addr
            );
        #endregion

        #region Hd0.dll 相关接口

        [DllImport(_Hd0, EntryPoint = "hda_fun_0")]
        public static extern IntPtr MountDisk(IntPtr deviceHandle, int disNum, UInt64 dsksecs, Byte devtype);

        [DllImport(_Hd0, EntryPoint = "hda_fun_1")]
        public static extern int GetPhysicalPartitions(IntPtr devHandle, ref DSK_PART_TABLE partTable);

        [DllImport(_Hd0, EntryPoint = "hda_fun_2")]
        public static extern void FreePartitionHanlder(IntPtr mountDevHandle, ref DSK_PART_TABLE partTable);

        /// <summary>
        /// 深度查找分区
        /// </summary>
        [DllImport(_Hd0, EntryPoint = "hda_fun_3")]
        public static extern int GetPhysicalPartitionsByScall(IntPtr mountDevHandle, FindVolumeCallBack lp, UInt64 offset, UInt32 intervalSec, ref DSK_PART_TABLE partTable);

        /// <summary>
        ///  设置操作状态
        /// </summary>
        /// <param name="mountDevHandle">设备句柄</param>
        /// <param name="stauts">nStatus - 状态: 1 - 执行, 2 - 暂停, 3 - 停止</param>
        [DllImport(_Hd0, EntryPoint = "hda_fun_4")]
        public static extern void SetGetPartitionStatus(IntPtr mountDevHandle, Int32 stauts);

        /// <summary>
        /// 释放设备句柄
        /// </summary>
        /// <param name="mountDevHandle">设备句柄</param>
        [DllImport(_Hd0, EntryPoint = "hda_fun_5")]
        public static extern void UnloadDeviceHandle(ref IntPtr mountDevHandle);

        #endregion

        #region 创建磁盘位图接口
        /// <summary>
        /// 装载簇块快照文件
        /// </summary>
        /// <param name="szFileName">簇块快照文件名</param>
        /// <param name="nMode">装载文件模式: 0 - 装载一个存在的文件, 1 - 装载一个新文件</param>
        /// <returns>簇块快照句柄</returns>
        [DllImport(_SnapShotDLL, EntryPoint = "fdt_snapshot_fun_0")]
        public static extern IntPtr MountSnapShot(StringBuilder szFileName, UInt32 nMode);

        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="hSnapshot"></param>
        /// <returns></returns>
        [DllImport(_SnapShotDLL, EntryPoint = "fdt_snapshot_fun_1")]
        public static extern IntPtr UninstallSnapShot(ref IntPtr hSnapshot);

        /// <summary>
        /// 获取簇块快照信息
        /// </summary>
        /// <param name="hSnapshot">簇块快照句柄</param>
        /// <param name="pSnapshotInfo">簇块快照头结构信息(输出)</param>
        /// <returns></returns>
        [DllImport(_SnapShotDLL, EntryPoint = "fdt_snapshot_fun_3")]
        public static extern UInt32 GetSnapShotInfo(IntPtr hSnapshot, ref IntPtr pSnapshotInfo);

        /// <summary>
        /// 读取簇块快照信息
        /// </summary>
        /// <param name="hSnapshot">簇块快照句柄</param>
        /// <param name="offset_block">偏移块号</param>
        /// <param name="ino">输出文件目录id号</param>
        /// <param name="blk_status">0 - 未使用簇块, 1 - 正常使用簇块, 2 - 文件目录最后簇块</param>
        /// <returns></returns>
        [DllImport(_SnapShotDLL, EntryPoint = "fdt_snapshot_fun_5")]
        public static extern UInt32 ReadSnapShotInfo(IntPtr hSnapshot, UInt64 offset_block, ref UInt64 ino, ref Byte blk_status);
        #endregion

        #region 读写扇区数据
        /// <summary>
        /// 读/写扇区数据
        /// </summary>
        /// <param name="hMountVolume">分区句柄</param>
        /// <param name="offset_sec">偏移扇区</param>
        /// <param name="secs">扇区个数</param>
        /// <param name="pBuf">数据缓冲区</param>
        /// <param name="cmd">操作命令: 0x11 - 读扇区, 0x12 - 写扇区</param>
        /// <returns>成功与否(0 - 成功,other - 错误码 )</returns>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_14")]
        public static extern UInt32 ReadOrWriteSectorsData(IntPtr hMountVolume, UInt64 offset_sec, UInt32 secs, IntPtr pBuf, int cmd);
        #endregion

        #region 时间转换

        [DllImport(_TimeConverter, EntryPoint = "convert_std_time_to_date_formats")]
        public static extern void ConvertStdTimeToDateFormat(Int32 stdTm, ref DATE_FORMAT dft, bool znFlag = false);
        #endregion

        #region 阵列
        /// <summary>
        /// 装载镜像矩阵接口
        /// </summary>
        /// <param name="pRaidInfoEx">矩阵结构信息</param>
        /// <returns>阵列句柄</returns>
        [DllImport(_RaidEx, EntryPoint = "rdx_fun_A")]
        public static extern IntPtr MountRaid(ref RAID_INFO_EX pRaidInfoEx);

        /// <summary>
        /// 卸载装载镜像矩阵接口
        /// </summary>
        /// <param name="hMntRaid">阵列句柄</param>
        [DllImport(_RaidEx, EntryPoint = "rdx_fun_B")]
        public static extern void UnMountRaid(ref IntPtr hMntRaid);

        /// <summary>
        /// 获取-阵列信息
        /// </summary>
        /// <param name="hMntRaid">阵列句柄</param>
        /// <param name="pRaidInfoEx">输出阵列信息结构指针</param>
        [DllImport(_RaidEx, EntryPoint = "rdx_fun_C")]
        public static extern UInt32 GetRaidInfo(IntPtr hMntRaid, ref RAID_INFO_EX pRaidInfoEx);


        /// <summary>
        /// 读/写-阵列数据
        /// </summary>
        /// <param name="hMntRaid">阵列装载句柄</param>
        /// <param name="offset_sec">偏移扇区</param>
        /// <param name="secs">扇区数</param>
        /// <param name="pBuf">数据缓冲区</param>
        /// <param name="cmd">
        /// 0x11-读扇区 
        /// 0x12 - 写扇区
        /// </param>
        /// <returns></returns>
        [DllImport(_RaidEx, EntryPoint = "rdx_fun_D")]
        public static extern UInt32 ReadOrWriteRaidData(IntPtr hMntRaid, UInt64 offset_sec, Int32 secs, byte[] pBuf, int cmd);

        /// <summary>
        /// 分析-阵列信息
        /// </summary>
        /// <param name="hMntRaid">阵列装载句柄</param>
        /// <param name="lpfn">进度回调</param>
        /// <param name="pRaidInfoEx">输出阵列信息</param>
        /// <returns></returns>
        [DllImport(_RaidEx, EntryPoint = "rdx_fun_E")]
        public static extern UInt32 AnalyzeRaidInfo(IntPtr hMntRaid, lpfn_raid_update_progress lpfn, ref RAID_INFO_EX pRaidInfoEx);


        /// <summary>
        /// 操作状态
        /// </summary>
        /// <param name="hMntRaid">阵列句柄</param>
        /// <param name="nStatus">
        /// 状态控制 1 - 执行，2 - 暂停，3 - 停止
        /// </param>
        [DllImport(_RaidEx, EntryPoint = "rdx_fun_F")]
        public static extern void RaidStatusControls(IntPtr hMntRaid, int nStatus);


        /// <summary>
        /// RAID装载镜像文件矩阵的接口回调函数
        /// </summary>
        /// <param name="prri"></param>
        public delegate void lpfn_raid_update_progress(ref RAID_PROGRESS_INFO prri);
        #endregion

        #region HD1 Interface

        /// <summary>
        /// 装载磁盘克隆 
        /// </summary>
        /// <param name="pDirPath">存储克隆状态的文件夹</param>
        /// <returns></returns>
        [DllImport(_diskDll_hd1, EntryPoint = "cln_fun_0")]
        public static extern IntPtr HD1MountDeviceClone(string pDirPath);

        /// <summary>
        /// 卸载磁盘克隆
        /// </summary>
        /// <param name="devLocation"></param>
        [DllImport(_diskDll_hd1, EntryPoint = "cln_fun_1")]
        public static extern void HD1UnMountDeviceClone(ref IntPtr devLocation);

        /// <summary>
        /// 磁盘快速镜像
        /// </summary>
        /// <param name="hDskClone">克隆句柄</param>
        /// <param name="lpfn">克隆回调进度指针函数</param>
        /// <param name="pimgInfo">克隆参数结构指针</param>
        /// <param name="nCloneFlag">克隆标记：1 - 新的克隆，2 - 断点续克</param>
        [DllImport(_diskDll_hd1, EntryPoint = "cln_fun_2")]
        public static extern uint HD1QuickImage(IntPtr hDskClone, LpfnCallbackDskQuickCopy lpfn,
            ref DSK_IMAGE_PARAMETER_INFO pimgInfo, int nCloneFlag);

        /// <summary>
        /// 设置操作状态
        /// </summary>
        /// <param name="dev">装载句柄</param>
        /// <param name="nStatus">状态 1 - 执行，2 - 暂停， 3 - 停止 </param>
        [DllImport(_diskDll_hd1, EntryPoint = "cln_fun_3")]
        public static extern void HD1SetStatus(IntPtr dev, int nStatus);
        #endregion

        #region dev_flsh_krnl
        /// <summary>
        /// 装载设备句柄
        /// </summary>
        /// <param name="deviceHandle">设备句柄</param>
        /// <param name="deviceSize">设备大小（字节数）</param>
        /// <param name="deviceType">设备类型</param>
        /// <param name="flshType">
        /// 芯片类型：
        /// 0xA000 联发科手机芯片(mtk)
        /// 0xA001 展讯手机芯片
        /// 0xA002 诺机亚手机芯片
        /// 0xA003 WindowsPhone手机芯片
        /// 0xA004 晨星芯片(MStar)
        ///</param>
        /// <returns>分区句柄，null则失败</returns>
        [DllImport(_dev_flsh_krnl, EntryPoint = "dev_flsh_fun_A")]
        public static extern IntPtr MountDeviceHandle(IntPtr deviceHandle, UInt64 deviceSize, byte deviceType, uint flshType);

        /// <summary>
        /// 获取设备总扇区数
        /// </summary>
        /// <param name="PartitionHandle">分区句柄</param>
        /// <param name="deviceHandle">0成功，反之亦然</param>
        [DllImport(_dev_flsh_krnl, EntryPoint = "dev_flsh_fun_D")]
        public static extern uint DeviceTotalSectors(IntPtr deviceHandle, ref UInt64 totalSectors);

        /// <summary>
        /// 卸载设备句柄
        /// </summary>
        /// <param name="PartitionHandle">分区句柄</param>
        [DllImport(_dev_flsh_krnl, EntryPoint = "dev_flsh_fun_B")]
        public static extern void UnmountDeviceHandle(ref IntPtr PartitionHandle);
        #endregion


    }
}
