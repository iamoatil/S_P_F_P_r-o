/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/28 9:49:22 
 * explain :  
 *
*****************************************************************************/

using System;
using X64Service;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 镜像文件设备服务
    /// </summary>
    public class MirrorDeviceService : FileServiceAbstractX
    {
        /// <summary>
        /// 镜像文件设备服务
        /// </summary>
        /// <param name="device"></param>
        /// <param name="iAsyn"></param>
        public MirrorDeviceService(IFileSystemDevice device, IAsyncProgress iAsyn)
            : base(device, iAsyn)
        {

        }

        /// <summary>
        /// 获取打开设备句柄
        /// </summary>
        /// <returns></returns>
        public override IntPtr OpenDevice()
        {
            if (Device == null)
            {
                LoggerManagerSingle.Instance.Error("Invalid device");
                return IntPtr.Zero;
            }
            Device.Handle = FileServiceCoreDll.OpenFile(Device.Source.ToString());
            return Device.Handle;
        }

        /// <summary>
        /// 获取设备分区
        /// </summary>
        public override void LoadDevicePartitions()
        {
            try
            {
                Asyn?.Advance(1, LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.FileServiceLanguage_File_JiaZaiXiTongFenQuLieBiao));

                var rootTable = new DSK_PART_TABLE();
                var result = FileServiceCoreDll.GetMirrorFilePartitions(ref rootTable, Device.Source.ToString());
                if (result != 0)
                {
                    LoggerManagerSingle.Instance.Error("1/123获取镜像文件的分区信息失败并返回");
                    return;
                }
                if (rootTable.next == IntPtr.Zero)  //无法读取分区，需要进行深度分区扫描
                {
                    var handle = FileServiceCoreDll.MountDisk(Device.Handle, -1, (ulong)Device.TotalSectors, 0x12);
                    if (handle == IntPtr.Zero)
                    {
                        LoggerManagerSingle.Instance.Error(string.Format("2/123获取镜像文件的分区信息成功;无法读取分区,需要进行深度分区扫描;加载磁盘句柄失败(句柄:{0}, disNum:{1},扇区数:{2},设备类型:0x12)", Device.Handle, -1, Device.TotalSectors));
                    }

                    FindVolumeCallBack fv = (ref FIND_VOLUME_PROGRESS pdi) => { return 0; };
                    try
                    {
                        result = FileServiceCoreDll.GetPhysicalPartitionsByScall(handle, fv, 0, 1, ref rootTable);
                    }
                    catch (Exception ex)
                    {
                        LoggerManagerSingle.Instance.Error("深度查找分区异常", ex);
                    }

                    if (result != 0)
                    {
                        LoggerManagerSingle.Instance.Error("3/123获取镜像文件的分区信息成功; 无法读取分区,需要进行深度分区扫描; 深度查找分区失败并返回");
                        return;
                    }

                    var parts = CreatePartition(rootTable);
                    Device.Parts.AddRange(parts);
                    FileServiceCoreDll.UnloadDeviceHandle(ref handle);
                }
                else
                {
                    var parts = CreatePartition(rootTable);
                    Device.Parts.AddRange(parts);
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error("获取设备分区异常", ex);
            }
        }

        /// <summary>
        /// 停止当前工作
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            if (RunPartition == null)
            {
                return;
            }
            try
            {
                FileServiceCoreDll.Stop(RunPartition.Mount);
                if (RunPartition.Mount != IntPtr.Zero)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("停止操作并卸载分区(名称:{0},描述:{1},大小:{2},Mount:{3})", RunPartition.Name,
                        RunPartition.Discription, RunPartition.Size, RunPartition.Mount));

                    IntPtr mount = RunPartition.Mount;
                    var nodelist = RunPartition.NodeLinkList;
                    int result = FileServiceCoreDll.DisposeLinkTableRoom(mount, ref nodelist);
                    MirrorCoreDll.UnmountPartitionHandle(ref mount);
                    if (result == 0)
                    {
                        LoggerManagerSingle.Instance.Error(string.Format("释放目录文件节点链表空间(不会释放自己定义的顶层节点空间)(名称:{0},描述:{1},大小:{2},Mount:{3})成功，错误码：{4}，卸载分区", RunPartition.Name, RunPartition.Discription, RunPartition.Size, RunPartition.Mount, result));
                    }
                    else
                    {
                        LoggerManagerSingle.Instance.Error(string.Format("释放目录文件节点链表空间(不会释放自己定义的顶层节点空间)(名称:{0},描述:{1},大小:{2},Mount:{3})失败，错误码：{4}，卸载分区", RunPartition.Name, RunPartition.Discription, RunPartition.Size, RunPartition.Mount, result));
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(string.Format("停止分区(名称:{0},描述:{1},大小:{2},Mount:{3})时出现错误, 信息:{4}", RunPartition.Name, RunPartition.Discription, RunPartition.Size, RunPartition.Mount, ex.Message));
            }
            finally
            {
                RunPartition.Mount = IntPtr.Zero;
                _isStop = true;
            }
        }

        /// <summary>
        /// 设备关闭
        /// </summary>
        public override void Close()
        {
            if (Device == null || Device.Parts.Count == 0)
            {
                return;
            }

            try
            {
                // 卸载装载分区句柄
                foreach (var part in Device.Parts)
                {
                    var fpart = (FileSystemPartition)part;
                    if (fpart.Mount != IntPtr.Zero)
                    {
                        IntPtr mount = fpart.Mount;
                        var nodelist = fpart.NodeLinkList;
                        FileServiceCoreDll.DisposeLinkTableRoom(mount, ref nodelist);
                        MirrorCoreDll.UnmountPartitionHandle(ref mount);
                        fpart.Mount = IntPtr.Zero;

                        LoggerManagerSingle.Instance.Info(string.Format("卸载分区并释放节点链表空间(名称:{0},描述:{1},大小:{2},Mount:{3})", fpart.Name, fpart.Discription, fpart.Size, fpart.Mount));
                    }
                }
                // 释放设备打开句柄
                MirrorCoreDll.CloseDevice(Device.Handle);
                LoggerManagerSingle.Instance.Info(string.Format("关闭设备(名称:{0},大小:{1},Handle:{2})成功", Device.Name, Device.Size, Device.Handle));
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(string.Format("关闭设备时出错(名称:{0},大小:{1},Handle:{2})时出现错误, 信息:{3}", Device.Name, Device.Size, Device.Handle, ex.Message));
            }

        }

    }
}
