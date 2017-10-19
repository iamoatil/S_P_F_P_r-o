using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    public class CellbriteDeviceService : FileServiceAbstractX
    {
        #region Fields
        /// <summary>
        /// 当前的RAID对象
        /// </summary>
        private RAID_INFO_EX RaidInfoEx;

        #endregion

        #region Constructors

        public CellbriteDeviceService(IFileSystemDevice device, IAsyncProgress iAsyn) 
            : base(device, iAsyn)
        {
        }

        #endregion

        #region Methods

        #region Public

        public override void Close()
        {
        }

        public override void LoadDevicePartitions()
        {
            if (this.Device.HasPartition)
            {
                return;
            }
            var partlink = new DISK_INFO();
            var result = FileServiceCoreDll.GetDiskInfo(ref partlink, this.Device.DiskNumber);
            if (result != 0)
            {
                //LogHelper.Error(string.Format(LanguageHelper.Get("LANGKEY_HuoQuCiPanFenQuXinXiShiBaiDiCe_01591"), this.Device.Name, result));
                return;
            }
            if (partlink.part_counts == 0)
            {
                //LogHelper.Info(string.Format(LanguageHelper.Get("LANGKEY_CiPanFenQuGeShuWei_01592"), this.Device.Name));
                return;
            }
            for (int i = 0; i < partlink.part_counts; i++)
            {
                var part = partlink.pt[i];
                var partition = new FileSystemPartition();
                partition.Name = part.driver_letter.ToString();
                partition.Discription = part.vol_name;
                partition.FileSystem = part.file_system;
                partition.SerialNumber = string.Format("{0:X}", part.serial_num);
                partition.Size = (ulong)part.sectors.ToSafeInt64() * 512;
                partition.SectorOffset = (UInt64)part.start_lba.ToSafeInt64();
                partition.TotalSectors = (UInt64)part.sectors.ToSafeInt64();
                this.Device.Parts.Add(partition);
            }
        }

        public override IntPtr OpenDevice()
        {
            var handle = this.MountRaid(((CellbriteDevice)this.Device).Mirrors);
            this.Device.Handle = handle;
            this.Device.Size = this.RaidInfoEx.rd_total_bytes;
            return handle;
        }

        #endregion

        #region Private

        /// <summary>
        /// 装载UFED镜像
        /// </summary>
        /// <param name="mirrors"></param>
        /// <returns></returns>
        private IntPtr MountRaid(IList<string> mirrors)
        {
            var raid = new RAID_INFO_EX
            {
                dvi = new DEVICE_BASE_INFO[128]
            };
            for (var i = 0; i < mirrors.Count; i++)
            {
                var file = mirrors[i];
                if (!System.IO.File.Exists(file))
                {
                    //LogHelper.Error(string.Format(LanguageHelper.Get("LANGKEY_WenJianBuCunZai_01571"), file));
                    continue;
                }
                var info = new DEVICE_BASE_INFO();
                info.dev_type = 0x11;
                info.offset_sec = 0;
                info.len = (ulong)FileHelper.GetFileLength(file);
                info.secs = (info.len + 511) / 512;
                info.hDev = FileServiceCoreDll.OpenFile(file);
                raid.dvi[i] = info;
            }
            // 阵列类型
            raid.raid_type = 0x8000;
            // 磁盘个数
            raid.dsk_counts = (uint)mirrors.Count;
            var resut = FileServiceCoreDll.MountRaid(ref raid);
            this.RaidInfoEx = raid;
            return resut;
        }

        #endregion

        #endregion
    }
}
