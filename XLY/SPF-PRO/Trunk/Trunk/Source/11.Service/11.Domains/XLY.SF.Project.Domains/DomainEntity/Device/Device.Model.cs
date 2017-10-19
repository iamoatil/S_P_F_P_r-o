using System;
using System.Collections.Generic;
using System.Windows.Media;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 设备信息领域模型
    /// </summary>
    public partial class Device : NotifyPropertyBase, IDevice
    {
        public Device()
        {
            OSType = EnumOSType.None;
            Status = EnumDeviceStatus.Offline;
        }

        /// <summary>
        /// 构造只带number的设备
        /// </summary>
        public Device(string number)
        {
            OSType = EnumOSType.None;
            Status = EnumDeviceStatus.Offline;
            ID = number;
            SerialNumber = number;
            DeviceType = EnumDeviceType.Phone;
        }

        #region -- 是否可用，ture：可用

        private bool _Enable;

        /// <summary>
        /// 是否可用，ture：可用
        /// </summary>
        public bool Enable
        {
            get { return _Enable; }
            set
            {
                _Enable = value;
                OnPropertyChanged();
            }
        }

        #endregion

        /// <summary>
        /// 设备ID 唯一标识符
        /// </summary>
        public string ID { get; set; }

        #region DeviceType -- 设备类型

        private EnumDeviceType _DeviceType;

        /// <summary>
        /// 设备类型
        /// </summary>
        public EnumDeviceType DeviceType
        {
            get { return _DeviceType; }
            set
            {
                _DeviceType = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region ChipInfo -- 芯片信息

        private ChipDeviceInfo _ChipInfo;
        /// <summary>
        /// 芯片信息
        /// </summary>
        public ChipDeviceInfo ChipInfo
        {
            get { return _ChipInfo; }
            set
            {
                _ChipInfo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region BMac -- 蓝牙Mac

        /// <summary>
        /// 蓝牙Mac
        /// </summary>
        public string BMac { get; set; }

        #endregion

        #region TMac -- 终端Mac

        /// <summary>
        /// 终端Mac
        /// </summary>
        public string TMac { get; set; }

        #endregion

        #region Name -- 名称

        private string _Name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Manufacture -- 厂商

        private string _Manufacture;
        /// <summary>
        /// 厂商
        /// </summary>
        public string Manufacture
        {
            get { return _Manufacture; }
            set
            {
                _Manufacture = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region Brand -- 品牌

        private string _Brand;
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand
        {
            get { return _Brand; }
            set
            {
                _Brand = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Model -- 型号

        private string _Model;
        /// <summary>
        /// 型号
        /// </summary>
        public string Model
        {
            get { return _Model; }
            set
            {
                _Model = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region SerialNumber -- 序列号

        private string _SerialNumber;
        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialNumber
        {
            get { return _SerialNumber; }
            set
            {
                _SerialNumber = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region OSType -- 系统类型

        private EnumOSType _OSType;
        /// <summary>
        /// 系统类型
        /// </summary>
        public EnumOSType OSType
        {
            get { return _OSType; }
            set
            {
                _OSType = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region IMEI --

        private string _IMEI;
        /// <summary>
        /// IMEI
        /// </summary>
        public string IMEI
        {
            get { return _IMEI; }
            set { _IMEI = value; }
        }

        #endregion

        #region IMSI --

        private string _IMSI;
        /// <summary>
        /// IMSI
        /// </summary>
        public string IMSI
        {
            get { return _IMSI; }
            set
            {
                _IMSI = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region IsRoot -- 验证是否Root(越狱)。True为已Root

        private bool _IsRoot;
        /// <summary>
        /// 验证是否Root(越狱)。True为已Root
        /// </summary>
        public bool IsRoot
        {
            get { return _IsRoot; }
            set
            {
                _IsRoot = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否root描述，已解决系统类型、语言问题。
        /// </summary>
        public string IsRootDesc
        {
            get
            {
                switch (OSType)
                {
                    //case EnumOSType.IOS: return IsRoot ? LanguageHelperSingle.Instance.Language.OtherLanguage.Device_IOS_IsRoot_Root : LanguageHelperSingle.Instance.Language.OtherLanguage.Device_IOS_IsRoot_NotRoot;
                    //case EnumOSType.Android: return IsRoot ? LanguageHelperSingle.Instance.Language.OtherLanguage.Device_Android_IsRoot_Root : LanguageHelperSingle.Instance.Language.OtherLanguage.Device_Android_IsRoot_NotRoot;
                    case EnumOSType.IOS: return IsRoot ? LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Device_IOS_IsRoot_Root) : LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Device_IOS_IsRoot_NotRoot);
                    case EnumOSType.Android: return IsRoot ? LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Device_Android_IsRoot_Root) : LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Device_Android_IsRoot_NotRoot);
                }
                return IsRoot.ToString();
            }
        }

        public bool IsRootEX { get; set; }
        #endregion

        #region OSVersion -- 系统版本

        private string _OSVersion;
        /// <summary>
        /// 系统版本
        /// </summary>
        public string OSVersion
        {
            get { return _OSVersion; }
            set
            {
                _OSVersion = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Status -- 设备状态

        [NonSerialized]
        private EnumDeviceStatus _Status;
        /// <summary>
        /// 设备状态
        /// </summary>
        public EnumDeviceStatus Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged();
            }
        }

        [NonSerialized]
        public EnumDeviceStatus RealStatus;

        #endregion

        #region InstalledApps
        private List<AppEntity> _apps;

        /// <summary>
        /// 已安装的应用列表
        /// </summary>
        public List<AppEntity> InstalledApps
        {
            get
            {
                return _apps;
            }
            set
            {
                _apps = value;
            }
        }

        #endregion

        #region ScreenImage -- 屏幕快照图片

        [NonSerialized]
        private ImageSource _ScreenImage;

        /// <summary>
        /// 屏幕快照图片
        /// </summary>
        public ImageSource ScreenImage
        {
            get { return _ScreenImage; }
            set { _ScreenImage = value; }
        }
        #endregion

        /// <summary>
        /// 设备SDCard路径
        /// </summary>
        public string SDCardPath
        {
            get;
            set;
        }


        /// <summary>
        /// 芯片类型：
        /// 0xA000 联发科手机芯片(mtk)
        /// 0xA001 展讯手机芯片
        /// 0xA002 诺机亚手机芯片
        /// 0xA003 WindowsPhone手机芯片
        /// 0xA004 晨星芯片(MStar)
        /// </summary>
        public FlshType FlshType { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DevType DevType { get; set; }

        #region Properties
        /// <summary>
        /// 设备属性信息
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }
        #endregion

        #region IEquatable

        /// <summary>
        /// 根据序列化判定是否统一设备
        /// </summary>
        public override bool Equals(object other)
        {
            if (other is Device)
                return string.Equals(ID, ((Device)other).ID) || string.Equals(SerialNumber, ((Device)other).SerialNumber);
            return false;
        }

        #endregion

        #region IEqualityComparer
        public bool Equals(Device x, Device y)
        {
            return x.ID.Equals(y.ID) || x.SerialNumber.Equals(y.SerialNumber);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        #endregion

        /// <summary>
        /// su命令格式 例如  su -c "{0}"
        /// </summary>
        public string SU { get; set; }

        /// <summary>
        /// ls命令格式 例如  ls -l /data/data
        /// </summary>
        public string LS { get; set; }
    }
}
