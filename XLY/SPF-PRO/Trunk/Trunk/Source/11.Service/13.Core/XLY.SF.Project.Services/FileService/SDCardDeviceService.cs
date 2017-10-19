using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    public class SDCardDeviceService : FileServiceAbstractX
    {
        #region Constructors

        public SDCardDeviceService(IFileSystemDevice device, IAsyncProgress iAsyn) 
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
                partition.SectorOffset = (ulong)part.start_lba.ToSafeInt64();
                partition.TotalSectors = (ulong)part.sectors.ToSafeInt64();
                this.Device.Parts.Add(partition);
            }
        }

        public override IntPtr OpenDevice()
        {
            IntPtr handle = FileServiceCoreDll.OpenDevice(this.Device.DiskNumber);
            Device.Handle = handle;
            return handle;
        }

        #endregion

        #endregion
    }
}
