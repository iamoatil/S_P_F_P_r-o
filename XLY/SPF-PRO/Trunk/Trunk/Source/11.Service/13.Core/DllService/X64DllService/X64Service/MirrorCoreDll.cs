using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace X64Service
{
    public class MirrorCoreDll
    {
        private const string _Hd = @"bin\hd.dll";

        private const string _Hd0 = @"bin\hd0.dll";

        private const string _DskFsMaster = @"bin\dsk_fs_master.dll";

        private const string _FileTypTPL = @"bin\file_type_tpl.dll";

        private const string _SnapShotDLL = @"bin\blk_fdt_snapshot.dll";

        private const string _TimeConverter = @"bin\xtime_convert.dll";

        private const string _RaidEx = @"bin\rdEx.dll";


        /// <summary>
        /// 获取磁盘的基本信息
        /// </summary>
        /// <param name="pdsk">磁盘信息</param>
        /// <param name="dsk_num">磁盘的编号</param>
        /// <returns>标识：0，成功</returns>
        public uint GetDiskInfoEx(ref DISK_INFO pdsk, byte dsk_num)
        {
            return MirrorCoreDll.GetDiskInfo(ref pdsk, dsk_num);
            
        }

        /// <summary>
        /// 打开磁盘
        /// </summary>
        /// <param name="dskNum">磁盘编号</param>
        /// <returns>磁盘句柄</returns>
        public IntPtr OpenDeviceEx(Byte dskNum)
        {
            return OpenDevice(dskNum);
        }

        /// <summary>
        /// 打开文件
        /// 镜像文件
        /// </summary>
        /// <param name="szName">文件的物理路径 </param>
        /// <returns>文件句柄</returns>
        public IntPtr OpenFileEx(string szName)
        {
            return OpenFile(szName);
        }

        /// <summary>
        /// 总是创建一个新的文件
        /// </summary>
        /// <param name="szName">文件全路径</param>
        /// <returns>文件句柄</returns>
        public IntPtr CreateNewFileEx(string szName)
        {
            return CreateNewFile(szName);
        }

        /// <summary>
        /// 获取镜像文件的分区信息
        /// </summary>
        /// <param name="partTable">分区表</param>
        /// <param name="mirrorFilePath">镜像文件</param>
        /// <returns>返回0，代表解析成功。</returns>
        public int GetMirrorFilePartitionsEx(ref DSK_PART_TABLE partTable, string mirrorFilePath)
        {
            return GetMirrorFilePartitionsEx(ref partTable, mirrorFilePath);
        }

        /// <summary>
        /// 卸载（释放）分区链表资源。
        /// </summary>
        /// <param name="partTable">链表对象。</param>
        public void FreePartitionTableHandleEx(ref DSK_PART_TABLE partTable)
        {
            FreePartitionTableHandle(ref partTable);
        }

        /// <summary>
        /// 关闭设备句柄
        /// </summary>
        /// <param name="deviceHandle"></param>
        public void CloseDeviceEx(IntPtr deviceHandle)
        {
            CloseDevice(deviceHandle);
        }

        /// <summary>
        /// 打开卷(分区)
        /// </summary>
        /// <param name="nDrive">分区盘符</param>
        /// <returns>分区句柄</returns>
        public IntPtr OpenPartitionEx(char nDrive)
        {
            return OpenPartition(nDrive);
        }

        /// <summary>
        /// 获取分区信息
        /// </summary>
        /// <param name="partitionLetter">分区盘符</param>
        /// <param name="diskVolumeInfo">分区信息</param>
        public uint GetPartitionExInfoEx(char partitionLetter, ref DSK_VOLUME_INFO diskVolumeInfo)
        {
            return GetPartitionExInfo(partitionLetter, ref diskVolumeInfo);
        }

        /// <summary>
        /// 获取设备的扇区数
        /// </summary>
        public UInt32 GetDiskSectorsEx(byte diskNum, ref ulong sectors)
        {
            return GetDiskSectors(diskNum, ref sectors);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="hSnapshot">簇块文件目录快照文件句柄</param>
        /// <param name="deviceHaldle">磁盘句柄</param>
        /// <param name="partitionOffset">分区开始扇区(偏移量)</param>
        /// <param name="partitionTotalSectors">分区总扇区数</param>
        /// <param name="fileSystemType">文件系统分区类型（NTFS,Fat32,Ext2...）</param>
        /// <param name="mountPartitionFlag">分区标记: 0 - 正常分区, 1 - raw分区</param>
        /// <param name="deviceType">物理设备类型</param>
        /// <returns>分区句柄</returns>
        public IntPtr MountPartitionEx(IntPtr hSnapshot, IntPtr deviceHaldle, UInt64 partitionOffset, UInt64 partitionTotalSectors, byte fileSystemType, byte mountPartitionFlag, byte deviceType)
        {
            return MountPartition(hSnapshot, deviceHaldle, partitionOffset, partitionTotalSectors, fileSystemType, mountPartitionFlag, deviceType);
        }

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
        public Int32 ScanFilesEx(IntPtr mountHandle, ScanCallBack lpfnprgs, ref LINK_DIR_FILE_NODE_INFO fileLinks, byte scanModel, ref LIST_FILE_RAW_TYPE_INFO rawList, bool SnapFlag)
        {
            return ScanFiles(mountHandle, lpfnprgs, ref fileLinks, scanModel, ref rawList, SnapFlag);
        }

        /// <summary>
        /// 释放目录文件节点链表空间(不会释放自己定义的顶层节点空间)
        /// </summary>
        /// <param name="mountHandle">分区mount成功后返回的句柄</param>
        /// <param name="fileLinks">底层可返回的文件链对象</param>
        public Int32 DisposeLinkTableRoomEx(IntPtr mountHandle, ref LINK_DIR_FILE_NODE_INFO fileLinks)
        {
            return DisposeLinkTableRoom(mountHandle, ref fileLinks);
        }

        /// <summary>
        /// 停止(停止文件扫描、文件恢复)
        /// </summary>
        /// <param name="mountHandle">装载的分区句柄</param>
        /// <param name="nStatus">nStatus - 状态: 1 - 执行, 2 - 暂停, 3 - 停止</param>
        public void StopEx(IntPtr mountHandle, Int32 nStatus)
        {
            Stop(mountHandle, nStatus);
        }

        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="mountHandle">装载的分区句柄</param>
        /// <param name="newFileHandle">新建文件句柄</param>
        /// <param name="callback">文件恢复进度回调函数</param>
        /// <param name="recoveryFileInfo">被恢复文件结构</param>
        /// <returns></returns>
        public Int32 RecoveryFileEx(IntPtr mountHandle, IntPtr newFileHandle, LpfnRecoveryFileProgress callback, ref FILE_RECOVERY_INFO recoveryFileInfo)
        {
            return RecoveryFile(mountHandle, newFileHandle, callback, ref recoveryFileInfo);
        }

        /// <summary>
        /// 通过底层读取文件内容
        /// </summary>
        /// <param name="mountHandle">分区句柄</param>
        /// <param name="recoveryFileInfo">文件信息结构指针</param>
        /// <param name="offset_bytes">偏移字节</param>
        /// <param name="length">读取长度</param>
        /// <param name="buffer">输出数据缓冲区指针</param>
        /// <returns>0 - 成功，其他数字-错误</returns>
        public UInt32 ReadFileByBottomEx(IntPtr mountHandle, ref FILE_RECOVERY_INFO recoveryFileInfo, UInt64 offset_bytes, UInt32 length, byte[] buffer)
        {
            return ReadFileByBottom(mountHandle, ref recoveryFileInfo, offset_bytes, length, buffer);
        }

        /// <summary>
        /// 通过底层读取文件内容
        /// </summary>
        /// <param name="mountHandle">分区句柄</param>
        /// <param name="recoveryFileInfo">文件信息结构指针</param>
        /// <param name="offset_bytes">偏移字节</param>
        /// <param name="length">读取长度</param>
        /// <param name="buffer">输出数据缓冲区指针</param>
        /// <returns>0 - 成功，其他数字-错误</returns>
        public Int32 ReadFileByBottomEx(IntPtr mountHandle, ref FILE_RECOVERY_INFO recoveryFileInfo, UInt64 offset_bytes, Int32 length, IntPtr buffer)
        {
            return ReadFileByBottom(mountHandle, ref recoveryFileInfo, offset_bytes, length, buffer);
        }

        /// <summary>
        /// 通过底层读取文件内容
        /// </summary>
        /// <param name="mountHandle">分区句柄</param>
        /// <param name="recoveryFileInfo">文件信息结构指针</param>
        /// <param name="offset_bytes">偏移字节</param>
        /// <param name="length">读取长度</param>
        /// <param name="buffer">输出数据缓冲区指针</param>
        /// <returns>0 - 成功，其他数字-错误</returns>
        public Int32 ReadFileByBottomEx(IntPtr mountHandle, ref FILE_RECOVERY_INFO recoveryFileInfo, UInt64 offset_bytes, Int32 length, byte[] buffer)
        {
            return ReadFileByBottom(mountHandle, ref recoveryFileInfo, offset_bytes, length, buffer);
        }

        /// <summary>
        /// 卸载分区
        /// </summary>
        /// <param name="mountHandle">装载分区的句柄</param>
        public void UnmountPartitionHandleEx(ref IntPtr mountHandle)
        {
            UnmountPartitionHandle(ref mountHandle);
        }

        /// <summary>
        /// 获取位图信息
        /// </summary>
        /// <param name="mountHandle">装载分区的句柄</param>
        /// <param name="plnk">位图信息链表 </param>
        public Int32 GetBitMapEx(IntPtr mountHandle, ref LINK_DATA_BLOCK_NODE_INFO plnk)
        {
            return GetBitMap(mountHandle, ref plnk);
        }

        /// <summary>
        /// 释放位图信息链表
        /// </summary>
        /// <param name="mountHandle">装载分区的句柄</param>
        /// <param name="plnk">位图信息链表 </param>
        public void DisposeBitMapEx(IntPtr mountHandle, ref LINK_DATA_BLOCK_NODE_INFO plnk)
        {
            DisposeBitMap(mountHandle, ref plnk);
        }

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
        public UInt32 ReadBlockEx(IntPtr hMountVolume, IntPtr pBuf, UInt64 offset_blk, UInt32 blocks, UInt32 bytes_per_blk, ref UInt64 offset_blk_addr)
        {
            return ReadBlock(hMountVolume, pBuf, offset_blk, blocks, bytes_per_blk, ref offset_blk_addr);
        }

        public IntPtr MountDiskEx(IntPtr deviceHandle, int disNum, UInt64 dsksecs, Byte devtype)
        {
            return MountDisk(deviceHandle, disNum, dsksecs, devtype);
        }

        public int GetPhysicalPartitionsEx(IntPtr devHandle, ref DSK_PART_TABLE partTable)
        {
            return GetPhysicalPartitions(devHandle, ref partTable);
        }

        public void FreePartitionHanlderEx(IntPtr mountDevHandle, ref DSK_PART_TABLE partTable)
        {
            FreePartitionHanlder(mountDevHandle, ref partTable);
        }

        /// <summary>
        /// 深度查找分区
        /// </summary>
        public int GetPhysicalPartitionsByScallEx(IntPtr mountDevHandle, FindVolumeCallBack lp, UInt64 offset, UInt32 intervalSec, ref DSK_PART_TABLE partTable)
        {
            return GetPhysicalPartitionsByScall(mountDevHandle, lp, offset, intervalSec, ref partTable);
        }

        /// <summary>
        ///  设置操作状态
        /// </summary>
        /// <param name="mountDevHandle">设备句柄</param>
        /// <param name="stauts">nStatus - 状态: 1 - 执行, 2 - 暂停, 3 - 停止</param>
        public void SetGetPartitionStatusEx(IntPtr mountDevHandle, Int32 stauts)
        {
            SetGetPartitionStatus(mountDevHandle, stauts);
        }

        /// <summary>
        /// 释放设备句柄
        /// </summary>
        /// <param name="mountDevHandle">设备句柄</param>
        public void UnloadDeviceHandleEx(ref IntPtr mountDevHandle)
        {
            UnloadDeviceHandle(ref mountDevHandle);
        }

        /// <summary>
        /// 装载簇块快照文件
        /// </summary>
        /// <param name="szFileName">簇块快照文件名</param>
        /// <param name="nMode">装载文件模式: 0 - 装载一个存在的文件, 1 - 装载一个新文件</param>
        /// <returns>簇块快照句柄</returns>
        public IntPtr MountSnapShotEx(StringBuilder szFileName, UInt32 nMode)
        {
            return MountSnapShot(szFileName, nMode);
        }

        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="hSnapshot"></param>
        /// <returns></returns>
        public IntPtr UninstallSnapShotEx(ref IntPtr hSnapshot)
        {
            return UninstallSnapShot(ref hSnapshot);
        }

        /// <summary>
        /// 获取簇块快照信息
        /// </summary>
        /// <param name="hSnapshot">簇块快照句柄</param>
        /// <param name="pSnapshotInfo">簇块快照头结构信息(输出)</param>
        /// <returns></returns>
        public UInt32 GetSnapShotInfoEx(IntPtr hSnapshot, ref IntPtr pSnapshotInfo)
        {
            return GetSnapShotInfo(hSnapshot, ref pSnapshotInfo);
        }

        /// <summary>
        /// 读取簇块快照信息
        /// </summary>
        /// <param name="hSnapshot">簇块快照句柄</param>
        /// <param name="offset_block">偏移块号</param>
        /// <param name="ino">输出文件目录id号</param>
        /// <param name="blk_status">0 - 未使用簇块, 1 - 正常使用簇块, 2 - 文件目录最后簇块</param>
        /// <returns></returns>
        public UInt32 ReadSnapShotInfoEx(IntPtr hSnapshot, UInt64 offset_block, ref UInt64 ino, ref Byte blk_status)
        {
            return ReadSnapShotInfo(hSnapshot, offset_block, ref ino, ref blk_status);
        }

        /// <summary>
        /// 读/写扇区数据
        /// </summary>
        /// <param name="hMountVolume">分区句柄</param>
        /// <param name="offset_sec">偏移扇区</param>
        /// <param name="secs">扇区个数</param>
        /// <param name="pBuf">数据缓冲区</param>
        /// <param name="cmd">操作命令: 0x11 - 读扇区, 0x12 - 写扇区</param>
        /// <returns>成功与否(0 - 成功,other - 错误码 )</returns>
        public UInt32 ReadOrWriteSectorsDataEx(IntPtr hMountVolume, UInt64 offset_sec, UInt32 secs, IntPtr pBuf, int cmd)
        {
            return ReadOrWriteSectorsData(hMountVolume, offset_sec, secs, pBuf, cmd);
        }

        public void ConvertStdTimeToDateFormatEx(Int32 stdTm, ref DATE_FORMAT dft, bool znFlag = false)
        {
            ConvertStdTimeToDateFormat(stdTm, ref dft, znFlag);
        }

        /// <summary>
        /// 装载镜像矩阵接口
        /// </summary>
        /// <param name="pRaidInfoEx">矩阵结构信息</param>
        /// <returns>阵列句柄</returns>
        public IntPtr MountRaidEx(ref RAID_INFO_EX pRaidInfoEx)
        {
            return MountRaid(ref pRaidInfoEx);
        }

        /// <summary>
        /// 卸载装载镜像矩阵接口
        /// </summary>
        /// <param name="hMntRaid">阵列句柄</param>
        public void UnMountRaidEx(ref IntPtr hMntRaid)
        {
            UnMountRaid(ref hMntRaid);
        }

        /// <summary>
        /// 获取-阵列信息
        /// </summary>
        /// <param name="hMntRaid">阵列句柄</param>
        /// <param name="pRaidInfoEx">输出阵列信息结构指针</param>
        public UInt32 GetRaidInfoEx(IntPtr hMntRaid, ref RAID_INFO_EX pRaidInfoEx)
        {
            return GetRaidInfo(hMntRaid, ref pRaidInfoEx);
        }

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
        public UInt32 ReadOrWriteRaidDataEx(IntPtr hMntRaid, UInt64 offset_sec, Int32 secs, byte[] pBuf, int cmd)
        {
            return ReadOrWriteRaidData(hMntRaid, offset_sec, secs, pBuf, cmd);
        }

        /// <summary>
        /// 分析-阵列信息
        /// </summary>
        /// <param name="hMntRaid">阵列装载句柄</param>
        /// <param name="lpfn">进度回调</param>
        /// <param name="pRaidInfoEx">输出阵列信息</param>
        /// <returns></returns>
        public UInt32 AnalyzeRaidInfoEx(IntPtr hMntRaid, lpfn_raid_update_progress lpfn, ref RAID_INFO_EX pRaidInfoEx)
        {
            return AnalyzeRaidInfo(hMntRaid, lpfn, ref pRaidInfoEx);
        }

        /// <summary>
        /// 操作状态
        /// </summary>
        /// <param name="hMntRaid">阵列句柄</param>
        /// <param name="nStatus">
        /// 状态控制 1 - 执行，2 - 暂停，3 - 停止
        /// </param>
        public void RaidStatusControlsEx(IntPtr hMntRaid, int nStatus)
        {
            RaidStatusControls(hMntRaid, nStatus);
        }


        #region 安卓设备底层交互接口
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
        /// 总是创建一个新的文件
        /// </summary>
        /// <param name="szName">文件全路径</param>
        /// <returns>文件句柄</returns>
        [DllImport(_Hd, EntryPoint = "fun_3")]
        public static extern IntPtr CreateNewFile(string szName);

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

        /// <summary>
        ///     
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
            byte mountPartitionFlag,
            byte deviceType);

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
        public static extern Int32 ScanFiles(IntPtr mountHandle, ScanCallBack lpfnprgs, ref LINK_DIR_FILE_NODE_INFO fileLinks, byte scanModel, ref LIST_FILE_RAW_TYPE_INFO rawList, bool SnapFlag);

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
        /// 恢复文件
        /// </summary>
        /// <param name="mountHandle">装载的分区句柄</param>
        /// <param name="newFileHandle">新建文件句柄</param>
        /// <param name="callback">文件恢复进度回调函数</param>
        /// <param name="recoveryFileInfo">被恢复文件结构</param>
        /// <returns></returns>
        [DllImport(_DskFsMaster, EntryPoint = "fs_fun_4")]
        public static extern Int32 RecoveryFile(IntPtr mountHandle, IntPtr newFileHandle, LpfnRecoveryFileProgress callback, ref FILE_RECOVERY_INFO recoveryFileInfo);

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
        [DllImport(_SnapShotDLL, EntryPoint = " ")]
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
    }

    #region 回调函数定义

    /// <summary>
    /// 读取扇区指针函数
    /// </summary>
    /// <returns></returns>
    public delegate Int32 LpfnReadSec(UInt64 offsetSec, Int32 sectors, UInt32 pBuf);
    /// <summary>
    /// 扫描文件进度回调函数
    /// </summary>
    /// <returns></returns>
    public delegate void LpfnUpdateProgress(IntPtr pdi);

    public delegate void LpfnRecoveryFileProgress(IntPtr pcf);

    #endregion

    #endregion
}
