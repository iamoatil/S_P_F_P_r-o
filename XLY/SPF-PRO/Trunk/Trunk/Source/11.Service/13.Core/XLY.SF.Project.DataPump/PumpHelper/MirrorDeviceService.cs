using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Domain;

namespace XLY.SF.Project.DataPump.PumpHelper
{
    /// <summary>
    /// 镜像文件设备信息;
    /// </summary>
    public class MirrorDeviceService : FileServiceAbstractX
    {
        /// <summary>
        /// 获取打开设备句柄
        /// </summary>
        /// <returns></returns>
        public override IntPtr OpenDevice()
        {
            if (this.Device == null)
            {
                return IntPtr.Zero;
            }
            //this.Device.Handle = FileServiceCoreDll.OpenFile(this.Device.Source.ToString());
            return this.Device.Handle;
        }

        /// <summary>
        /// 获取设备分区
        /// </summary>
        public override void LoadDevicePartitions()
        {
            try
            {
                if (this.Asyn != null)
                {
                    //this.Asyn.Advance(1, LanguageHelper.Get("LANGKEY_JiaZaiXiTongFenQuLieBiao_04954"));
                }
                var rootTable = new Domains.DllElement.DSK_PART_TABLE();
                //var result = FileServiceCoreDll.GetMirrorFilePartitions(ref rootTable, this.Device.Source.ToString());
                //if (result != 0)
                {
                    //LogHelper.Error(LanguageHelper.Get("LANGKEY_HuoQuJingXiangWenJianDeFenQuXi_04955"));
                    return;
                }
                if (rootTable.next == IntPtr.Zero)  //无法读取分区，需要进行深度分区扫描
                {
                    //var handle = FileServiceCoreDll.MountDisk(this.Device.Handle, -1, (ulong)this.Device.TotalSectors, 0x12);
                    //if (handle == IntPtr.Zero)
                    //{
                    //    LogHelper.Error(string.Format(LanguageHelper.Get("LANGKEY_HuoQuJingXiangWenJianDeFenQuXi_04956"), this.Device.Handle, -1, (ulong)this.Device.TotalSectors));
                    //}
                    //FindVolumeCallBack fv = (ref FIND_VOLUME_PROGRESS pdi) => { return 0; };
                    //Executer.TryRunLogExceptioin(() => result = FileServiceCoreDll.GetPhysicalPartitionsByScall(handle, fv, 0, 1, ref rootTable), LanguageHelper.Get("LANGKEY_ShenDuChaZhaoFenQuYiChang_04957"));
                    //if (result != 0)
                    //{
                    //    LogHelper.Error(LanguageHelper.Get("LANGKEY_HuoQuJingXiangWenJianDeFenQuXi_04958"));
                    //    return;
                    //}
                    var parts = this.CreatePartition(rootTable);
                    this.Device.Parts.AddRange(parts);
                    //FileServiceCoreDll.UnloadDeviceHandle(ref handle);
                }
                else
                {
                    var parts = this.CreatePartition(rootTable);
                    this.Device.Parts.AddRange(parts);
                }
            }
            catch (Exception ex)
            {
                //LogHelper.Error(LanguageHelper.Get("LANGKEY_HuoQuSheBeiFenQuYiChang_04959"), ex);
            }
        }

        /// <summary>
        /// 停止当前工作
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            if (this.RunPartition == null)
            {
                return;
            }
            try
            {
                //FileServiceCoreDll.Stop(this.RunPartition.Mount);
                if (this.RunPartition.Mount != IntPtr.Zero)
                {
                    //LogHelper.Error(string.Format(LanguageHelper.Get("LANGKEY_TingZhiCaoZuoBingXieZaiFenQuMi_01586"), RunPartition.Name,
                    //    RunPartition.Discription, RunPartition.Size, RunPartition.Mount));
                    IntPtr mount = this.RunPartition.Mount;
                    var nodelist = this.RunPartition.NodeLinkList;
                    //int result = FileServiceCoreDll.DisposeLinkTableRoom(mount, ref nodelist);
                    //MirrorCoreDll.UnmountPartitionHandle(ref mount);
                    //if (result == 0)
                    //{
                    //    LogHelper.Error(string.Format("释放目录文件节点链表空间(不会释放自己定义的顶层节点空间)(名称:{0},描述:{1},大小:{2},Mount:{3})成功，错误码：{4}，卸载分区", RunPartition.Name,
                    //        RunPartition.Discription, RunPartition.Size, RunPartition.Mount, result));
                    //}
                    //else
                    //{
                    //    LogHelper.Error(string.Format("释放目录文件节点链表空间(不会释放自己定义的顶层节点空间)(名称:{0},描述:{1},大小:{2},Mount:{3})失败，错误码：{4}，卸载分区", RunPartition.Name,
                    //        RunPartition.Discription, RunPartition.Size, RunPartition.Mount, result));
                    //}
                }
            }
            catch (Exception ex)
            {
                //LogHelper.Error(string.Format(LanguageHelper.Get("LANGKEY_TingZhiFenQuMingChengMiaoShuDa_01587"), RunPartition.Name,
                //        RunPartition.Discription, RunPartition.Size, RunPartition.Mount, ex.Message));
            }
            finally
            {
                this.RunPartition.Mount = IntPtr.Zero;
                this._isStop = true;
            }
        }

        /// <summary>
        /// 设备关闭
        /// </summary>
        public override void Close()
        {
            if (this.Device == null)
            {
                return;
            }
            if (this.Device.Parts.Count == 0)
            {
                return;
            }
            try
            {
                // 卸载装载分区句柄
                foreach (var part in this.Device.Parts)
                {
                    var fpart = (FileSystemPartition)part;
                    if (fpart.Mount != IntPtr.Zero)
                    {
                        //LogHelper.Info(string.Format(LanguageHelper.Get("LANGKEY_XieZaiFenQuBingShiFangJieDianL_01588"), fpart.Name,
                        //   fpart.Discription, fpart.Size, fpart.Mount));
                        IntPtr mount = fpart.Mount;
                        var nodelist = fpart.NodeLinkList;
                        //FileServiceCoreDll.DisposeLinkTableRoom(mount, ref nodelist);
                        //MirrorCoreDll.UnmountPartitionHandle(ref mount);
                        fpart.Mount = IntPtr.Zero;
                    }
                }
                // 释放设备打开句柄
                //MirrorCoreDll.CloseDevice(this.Device.Handle);
                //LogHelper.Info(string.Format(LanguageHelper.Get("LANGKEY_GuanBiSheBeiMingChengDaXiaoHan_01589"), Device.Name,
                //         Device.Size, Device.Handle));
            }
            catch (Exception ex)
            {
                //LogHelper.Error(string.Format(LanguageHelper.Get("LANGKEY_GuanBiSheBeiShiChuCuoMingCheng_01590"), Device.Name,
                //         Device.Size, Device.Handle, ex.Message));
            }
        }

        public MirrorDeviceService(Domains.DomainEntity.Device.IFileSystemDevice device, IAsyncResult iAsyn)
            : base(device, iAsyn)
        {
        }
    }
}
