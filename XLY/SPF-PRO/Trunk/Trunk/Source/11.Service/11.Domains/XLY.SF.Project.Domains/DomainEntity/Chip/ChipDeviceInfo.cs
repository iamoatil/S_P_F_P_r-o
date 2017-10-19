using System;
using System.ComponentModel;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.Domains
{
    [Serializable]
    public class ChipDeviceInfo : NotifyPropertyBase,INotifyPropertyChanged, ICloneable
    {
        #region 芯片状态
        /// <summary>
        /// 芯片状态
        /// </summary>
        private bool _Status;

        public bool Status
        {
            get { return this._Status; }
            set
            {
                this._Status = value;
                this.OnPropertyChanged("Status");
            }
        }
        #endregion

        #region 设备编号
        private byte _DiskNumber;

        /// <summary>
        /// 设备编号
        /// </summary>
        public byte DiskNumber
        {
            get { return this._DiskNumber; }
            set
            {
                this._DiskNumber = value;
                this.OnPropertyChanged("DiskNumber");
            }
        }

        #endregion 设备编号

        #region 设备类型

        /// <summary>
        /// 设备类型
        /// </summary>
        private string _DeviceType;

        public string DeviceType
        {
            get { return this._DeviceType; }
            set
            {
                this._DeviceType = value;
                this.OnPropertyChanged("DeviceType");
            }
        }
        #endregion

        #region 名称

        /// <summary>
        /// 名称
        /// </summary>
        private string _Name;

        public string Name
        {
            get { return this._Name; }
            set
            {
                this._Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        #endregion

        #region 大小

        /// <summary>
        /// 大小
        /// </summary>
        private string _Size;

        public string Size
        {
            get { return this._Size; }
            set
            {
                this._Size = value;
                this.OnPropertyChanged("Size");
            }
        }

        #endregion

        #region 总扇区数

        private ulong _TotalSector;
        public ulong TotalSector
        {
            get { return this._TotalSector; }
            set
            {
                this._TotalSector = value;
                this.OnPropertyChanged("TotalSector");
            }
        }
        #endregion

        #region 磁盘信息
        /// <summary>
        /// 磁盘信息
        /// </summary>
        private Disk _DiskInfo;

        public Disk DiskInfo
        {
            get { return this._DiskInfo; }
            set { this._DiskInfo = value; }
        }
        #endregion

        #region 释放对象
        /// <summary>
        /// 清空对象
        /// </summary>
        public void Clear()
        {
            this.DiskNumber = 0;
            this.Status = false;
            this.Name = string.Empty;
            this.DeviceType = string.Empty;
            this.TotalSector = 0;
            this.Size = string.Empty;
            this.DiskInfo = null;
        }
        #endregion

        #region 初始化芯片信息
        /// <summary>
        /// 初始化芯片信息
        /// </summary>
        /// <param name="chip"></param>
        public void InitChip(ChipDeviceInfo chip)
        {
            this.DiskNumber = chip.DiskNumber;
            this.Status = chip.Status;
            this.Name = chip.Name;
            this.DeviceType = chip.DeviceType;
            this.TotalSector = chip.TotalSector;
            this.Size = chip.Size;
            this.DiskInfo = chip.DiskInfo;
        }
        #endregion

        #region 克隆对象
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
