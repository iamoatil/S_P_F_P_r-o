using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X64Service;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 山寨机 - 镜像设备服务类
    /// 手机芯片类型：MTK、WP、MStar、WindowsMobile、WebOS、Bada、Brew、Infineon、CoolSand、ADI等
    /// @Author luochao
    /// @Date 20160906
    /// @Copy XLY
    /// </summary>
    public class CottageMirrorDeviceService : MirrorDeviceService
    {
        #region Constructors

        public CottageMirrorDeviceService(IFileSystemDevice device, IAsyncProgress iAsyn)
            : base(device, iAsyn)
        {
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取打开设备句柄
        /// </summary>
        /// <returns></returns>
        public override IntPtr OpenDevice()
        {
            base.OpenDevice();
            if (null == this.Device || this.Device.Handle == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            FileInfo file = new FileInfo(this.Device.Source.ToString());
            long l_fileSize = file.Length;
            UInt64 u_fileSize = (UInt64)l_fileSize;
            IntPtr handle = FileServiceCoreDll.MountDeviceHandle(this.Device.Handle, u_fileSize, (byte)this.Device.DevType, (uint)this.Device.FlshType);
            if (handle == IntPtr.Zero)
            {
                this.Device.Handle_Flsh = IntPtr.Zero;
                //LogHelper.Error(LanguageHelper.Get("LANGKEY_ZhuangZaiSheBeiJuBingShiBai_04944"));
            }
            else
            {
                this.Device.Handle_Flsh = handle; ;
                //LogHelper.Error(LanguageHelper.Get("LANGKEY_ZhuangZaiSheBeiJuBingChengGong_04945"));
            }
            return this.Device.Handle_Flsh;
        }

        /// <summary>
        /// 获取设备分区
        /// </summary>
        public override void LoadDevicePartitions()
        {
            if (this.Device.Parts.Count != 0)
            {
                return;
            }
            var link = new DSK_PART_TABLE();
            UInt64 totalSectors = 0;
            var result1 = FileServiceCoreDll.DeviceTotalSectors(this.Device.Handle_Flsh, ref totalSectors);
            var handle = FileServiceCoreDll.MountDisk(this.Device.Handle_Flsh, -1, totalSectors, 0x1B);
            var result2 = FileServiceCoreDll.GetPhysicalPartitions(handle, ref link);
            if (result2 != 0)
            {
                //LogHelper.Error(LanguageHelper.Get("LANGKEY_HuoQuSaiBanJingXiangWenJianFen_04946") + result2);
                return;
            }
            var parts = this.CreatePartition(link);
            foreach (var part in parts)
            {
                part.DevType = 0x12;
                part.PartType = 0;
                this.Device.Parts.Add(part);
            }
            FileServiceCoreDll.FreePartitionTableHandle(ref link);
        }

        /// <summary>
        /// 装载设备
        /// </summary>
        /// <returns></returns>
        public override IntPtr MountDevice()
        {
            if (this.RunPartition == null)
            {
                return IntPtr.Zero;
            }
            if (this.RunPartition.Mount != IntPtr.Zero)
            {
                return this.RunPartition.Mount;
            }
            byte fs = (byte)(this.Device.ScanModel == 0xC0 ? 0xFF : this.RunPartition.FileSystem);   //高级模式或者RAW模式下设置为0xFF
            IntPtr mount = IntPtr.Zero;
            this.RunPartition.DevType = 0x1B;
            mount = FileServiceCoreDll.MountPartition(this.RunPartition.SnapShotHandle, this.Device.Handle_Flsh,
                this.RunPartition.SectorOffset, this.RunPartition.TotalSectors, fs,
                this.RunPartition.PartType, this.RunPartition.DevType);
            if (mount.ToInt32() == 0)
            {
                //LogHelper.Error(string.Format(LanguageHelper.Get("LANGKEY_ZhuangZaiXiTongFenQuMingChengM_04947"), RunPartition.Name, RunPartition.Discription, RunPartition.Size));
            }

            this.RunPartition.Mount = mount;
            return mount;
        }

        /// <summary>
        /// 设备关闭
        /// </summary>
        public override void Close()
        {
            if (null == this.Device || null == this.Device.Parts || this.Device.Parts.Count == 0)
            {
                return;
            }
            // 卸载装载分区句柄
            foreach (var part in this.Device.Parts)
            {
                var fpart = (FileSystemPartition)part;
                if (fpart.Mount != IntPtr.Zero)
                {
                    //LogHelper.Info(string.Format(LanguageHelper.Get("LANGKEY_XieZaiFenQuBingShiFangJieDianL_04948"), fpart.Name, fpart.Discription, fpart.Size, fpart.Mount));
                    IntPtr mount = fpart.Mount;
                    var nodelist = fpart.NodeLinkList;
                    FileServiceCoreDll.DisposeLinkTableRoom(mount, ref nodelist);
                    MirrorCoreDll.UnmountPartitionHandle(ref mount);
                    fpart.Mount = IntPtr.Zero;
                }
            }
            // 卸载设备句柄
            IntPtr handle = this.Device.Handle_Flsh;
            FileServiceCoreDll.UnmountDeviceHandle(ref handle);
            //LogHelper.Info(string.Format(LanguageHelper.Get("LANGKEY_XieZaiSheBeiJuBingMingChengDaX_04949"), Device.Name, Device.Size, Device.Handle_Flsh));
            // 释放设备句柄
            MirrorCoreDll.CloseDevice(this.Device.Handle);
            //LogHelper.Info(string.Format(LanguageHelper.Get("LANGKEY_ShiFangSheBeiJuBingMingChengDa_04950"), Device.Name, Device.Size, Device.Handle));
        }

        #endregion

        #endregion
    }
}
