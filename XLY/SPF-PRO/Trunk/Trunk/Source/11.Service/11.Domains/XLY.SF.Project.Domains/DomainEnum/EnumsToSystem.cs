using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/16 18:04:46
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.Domains
{
    #region EnumOSType 设备系统类型

    /// <summary>
    /// 设备系统类型
    /// </summary>
    [Flags]
    public enum EnumOSType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("LANGKEY_Wu_00321")]
        None = 1,

        /// <summary>
        /// Android
        /// </summary>
        [Description("Android")]
        Android = 2,

        /// <summary>
        /// IOS
        /// </summary>
        [Description("IOS")]
        IOS = 4,

        /// <summary>
        /// WindowsPhone
        /// </summary>
        [Description("WindowsPhone")]
        WindowsPhone = 8,

        /// <summary>
        /// SDCard
        /// </summary>
        [Description("SDCard")]
        SDCard = 16,

        /// <summary>
        /// SIMCard
        /// </summary>
        [Description("SIMCard")]
        SIMCard = 32,

        /// <summary>
        /// 镜像文件
        /// </summary>
        [Description("Mirror")]
        Mirror = 64,
        /// <summary>
        ///HTC未开启USB调试模式
        /// </summary>
        [Description("LANGKEY_WeiKaiQiUSBDiaoShiMoShi_00322")]
        HtcBoUsbMode = 128,

        /// <summary>
        /// BlackBerry
        /// </summary>
        [Description("BlackBerry")]
        BlackBerry = 256,

        /// <summary>
        /// Symbian
        /// </summary>
        [Description("Symbian")]
        Symbian = 512,

        /// <summary>
        /// MTK
        /// </summary>
        [Description("MTK")]
        MTK = 1024,

        /// <summary>
        /// MStar
        /// </summary>
        [Description("MStar")]
        MStar = 2048,

        /// <summary>
        /// Spreadtrum
        /// </summary>
        [Description("Spreadtrum")]
        Spreadtrum = 4096,

        /// <summary>
        /// WindowsMobile
        /// </summary>
        [Description("WindowsMobile")]
        WindowsMobile = 8192,

        /// <summary>
        /// WebOS
        /// </summary>
        [Description("WebOS")]
        WebOS = 16384,

        /// <summary>
        /// Bada
        /// </summary>
        [Description("Bada")]
        Bada = 32768,

        /// <summary>
        /// 自动检测
        /// </summary>
        [Description("MobileAutoCheck")]
        MobileAutoCheck = 65536,

        /// <summary>
        /// Brew
        /// </summary>
        [Description("Brew")]
        Brew = 131072,

        /// <summary>
        /// Infineon
        /// </summary>
        [Description("Infineon")]
        Infineon = 262144,

        /// <summary>
        /// CoolSand
        /// </summary>
        [Description("CoolSand")]
        CoolSand = 524288,

        /// <summary>
        /// ADI
        /// </summary>
        [Description("ADI")]
        ADI = 1048576,

        /// <summary>
        /// Sky
        /// </summary>
        [Description("Sky")]
        Sky = 2097152,
    }

    #endregion

    #region EnumDeviceType 设备类型

    /// <summary>
    /// 设备类型
    /// </summary>
    public enum EnumDeviceType
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 1,

        /// <summary>
        /// 手机
        /// </summary>
        Phone = 2,

        /// <summary>
        /// 手机芯片
        /// </summary>
        Chip = 3,

        /// <summary>
        /// 本地磁盘
        /// </summary>
        Disk = 4,

        /// <summary>
        /// SD卡、U盘
        /// </summary>
        SDCard = 5,

        /// <summary>
        /// SIM卡
        /// </summary>
        SIM = 6,
    }

    #endregion

    #region EnumDeviceStatus 设备状态
    /// <summary>
    /// 设备状态
    /// </summary>
    public enum EnumDeviceStatus
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 在线
        /// </summary>
        Online = 1,

        /// <summary>
        /// 离线
        /// </summary>
        Offline = 2,

        /// <summary>
        /// 使用中
        /// </summary>
        InUse = 3,

        /// <summary>
        /// 系统恢复
        /// </summary>
        [Description("LANGKEY_XiTongHuiFu_00328")]
        Recovery = 11,

        /// <summary>
        /// 系统启动
        /// </summary>
        [Description("LANGKEY_XiTongQiDong_00329")]
        BootLoader = 12,

        /// <summary>
        /// 下载
        /// </summary>
        [Description("LANGKEY_XiaZai_00330")]
        Download = 13,
        /// <summary>
        ///HTC未开启手机USB调试
        /// </summary>
        [Description("LANGKEY_WeiKaiQiDiaoShiMoShi_00331")]
        HtcNoUsb,
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 14
    }
    #endregion

    #region EnumTaskStatus 任务状态
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum EnumTaskStatus
    {
        /// <summary>
        /// 空闲
        /// </summary>
        Free = 1,

        /// <summary>
        /// 正在提取
        /// </summary>
        Extracting = 2,

        /// <summary>
        /// 正在解析
        /// </summary>
        Parsing = 3,

        /// <summary>
        /// 正在保存
        /// </summary>
        OnSave = 4,

        /// <summary>
        /// 正在镜像
        /// </summary>
        Mirroring = 5,

        /// <summary>
        /// 正在加载
        /// </summary>
        Loading = 6,

        /// <summary>
        /// 正在分析
        /// </summary>
        Analysing = 7,

        /// <summary>
        /// 暂停
        /// </summary>
        Pause = 8,
    }
    #endregion

    #region EnumDataItem 数据项类别
    /// <summary>
    /// 数据项类别
    /// </summary>
    [Flags]
    public enum EnumDataItem
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 1,

        /// <summary>
        /// 数据包
        /// </summary>
        DataPackage = 2,

        /// <summary>
        /// 任务日志
        /// </summary>
        TaskLog = 4,
    }
    #endregion

    #region EnumMirror（镜像枚举）

    /// <summary>
    /// 镜像枚举
    /// </summary>
    public enum EnumMirror
    {
        /// <summary>
        /// 设备
        /// </summary>
        Device = 0,

        /// <summary>
        /// SIM
        /// </summary>
        SIM = 1,

        /// <summary>
        /// sdcard
        /// </summary>
        SDCard = 2,

        /// <summary>
        /// 手机芯片
        /// </summary>
        Chip = 3
    }

    #endregion

    #region EnumPump（数据泵/提取方式）

    /// <summary>
    /// 数据泵/提取方式
    /// </summary>
    [Flags]
    public enum EnumPump
    {
        /// <summary>
        /// USB连接提取
        /// </summary>
        USB = 1,

        /// <summary>
        /// 镜像提取
        /// </summary>
        Mirror = 2,

        /// <summary>
        /// Wifi app提取
        /// </summary>
        Wifi = 4,

        /// <summary>
        /// 蓝牙提取
        /// </summary>
        Bluetooth = 8,

        /// <summary>
        /// SdCard
        /// </summary>
        SDCard = 16,

        /// <summary>
        /// SIMCard
        /// </summary>
        SIMCard = 32,

        /// <summary>
        /// 手机芯片
        /// </summary>
        Chip = 64,

        /// <summary>
        /// 阵列
        /// </summary>
        Raid = 128,

        /// <summary>
        /// MTP
        /// </summary>
        MTP = 256,

        /// <summary>
        /// 本地文件夹提取
        /// </summary>
        LocalData = 512,
    }

    #endregion

    #region EnumScanModel 设备扫描模式

    /// <summary>
    /// 设备扫描模式
    /// </summary>
    public enum EnumScanModel : byte
    {
        /// <summary>
        /// 快速模式
        /// </summary>
        [Description("LANGKEY_KuaiSuMoShi_00332")]
        Quick = 0x40,

        /// <summary>
        /// 深度模式
        /// </summary>
        [Description("LANGKEY_ShenDuMoShi_00333")]
        Depth = 0x80,

        /// <summary>
        /// Raw扫描
        /// </summary>
        [Description("LANGKEY_RawMoShi_00334")]
        Raw = 0xC0,

        /// <summary>
        /// 高级扫描
        /// </summary>
        [Description("LANGKEY_GaoJiMoShi_00335")]
        Expert = 0x87
    }

    #endregion

    #region PhoneType 联系人、短信、通话记录 - 解析芯片类型参数

    /// <summary>
    /// 联系人 - 解析芯片类型参数
    /// </summary>
    public enum PhoneType_Contact : uint
    {
        PT_MTK_Contact = 0xB000,            //MTK芯片
        PT_Spreadtrum_Contact = 0xB001,     //展讯芯片
        PT_Symbian_Contact = 0xB002,        //诺机亚手机芯片
        PT_WindowsPhone_Contact = 0xB003,   //WindowsPhone手机芯片
        PT_MStar_Contact = 0xB004,          //晨星芯片(MStar)
        PT_WindowsMobile_Contact = 0xB005,  //WindowsMobile手机系统
        PT_WebOS_Contact = 0xB006,          //WebOS手机系统
        PT_Bada_Contact = 0xB007,           //Bada手机系统
        PT_Brew_Contact = 0xB008,           //Brew手机系统
        PT_Infineon_Contact = 0xB009,       //Infineon手机系统
        PT_CoolSand_Contact = 0xB00A,       //CoolSand手机系统
        PT_ADI_Contact = 0xB00B,            //ADI手机系统
        PT_Sky_Contact = 0xB00C             //Sky手机系统
    }

    /// <summary>
    /// 短信 - 解析芯片类型参数
    /// </summary>
    public enum PhoneType_SMS : uint
    {
        PT_MTK_SMS = 0xC000,                //MTK芯片
        PT_Spreadtrum_SMS = 0xC001,         //展讯芯片
        PT_Symbian_SMS = 0xC002,            //诺机亚手机芯片
        PT_WindowsPhone_SMS = 0xC003,       //WindowsPhone手机芯片
        PT_MStar_SMS = 0xC004,              //晨星芯片(MStar)
        PT_WindowsMobile_SMS = 0xC005,      //WindowsMobile手机系统
        PT_WebOS_SMS = 0xC006,              //WebOS手机系统
        PT_Bada_SMS = 0xC007,               //Bada手机系统
        PT_Brew_SMS = 0xC008,               //Brew手机系统
        PT_Infineon_SMS = 0xC009,           //Infineon手机系统
        PT_CoolSand_SMS = 0xC00A,           //CoolSand手机系统
        PT_ADI_SMS = 0xC00B,                //ADI手机系统
        PT_Sky_SMS = 0xC00C                 //Sky手机系统
    }

    /// <summary>
    /// 通话记录 - 解析芯片类型参数
    /// </summary>
    public enum PhoneType_Call : uint
    {
        PT_MTK_Call = 0xD000,               //MTK芯片
        PT_Spreadtrum_Call = 0xD001,        //展讯芯片
        PT_Symbian_Call = 0xD002,           //诺机亚手机芯片
        PT_WindowsPhone_Call = 0xD003,      //WindowsPhone手机芯片
        PT_MStar_Call = 0xD004,             //晨星芯片(MStar)
        PT_WindowsMobile_Call = 0xD005,     //WindowsMobile手机系统
        PT_WebOS_Call = 0xD006,             //WebOS手机系统
        PT_Bada_Call = 0xD007,              //Bada手机系统
        PT_Brew_Call = 0xD008,              //Brew手机系统
        PT_Infineon_Call = 0xD009,          //Infineon手机系统
        PT_CoolSand_Call = 0xD00A,          //CoolSand手机系统
        PT_ADI_Call = 0xD00B,               //ADI手机系统
        PT_Sky_Call = 0xD00C                //Sky手机系统
    }

    #endregion

    #region FlshType 芯片类型 - 山寨机设备句柄装载
    /// <summary>
    /// 芯片类型 - 山寨机设备句柄装载
    /// </summary>
    public enum FlshType : uint
    {
        FT_MTK = 0xA000,           //联发科手机芯片(mtk)
        FT_Spreadtrum = 0xA001,    //展讯手机芯片
        FT_Symbian = 0xA002,       //诺机亚手机芯片
        FT_WindowsPhone = 0xA003,  //WindowsPhone手机芯片
        FT_MStar = 0xA004,         //晨星芯片(MStar)
        FT_WindowsMobile = 0xA005, //WindowsMobile手机系统
        FT_WebOS = 0xA006,         //WebOS手机系统
        FT_Bada = 0xA007,           //Bada手机系统
        FT_Brew = 0xA008,           //Brew
        FT_Infineon = 0xA009,       //Infineon手机系统
        FT_CoolSand = 0xA00A,       //CoolSand手机系统
        FT_ADI = 0xA00B,            //ADI手机系统
        FT_Sky = 0xA00C             //Sky手机系统
    }

    #endregion

    #region DevType 设备类型 - 山寨机设备句柄装载
    /// <summary>
    /// 设备类型 - 山寨机设备句柄装载
    /// </summary>
    public enum DevType : byte
    {
        DT_11 = 0x11,   //普通设备: 如:磁盘、分区、镜像文件
        DT_12 = 0x12,   //阵列
        DT_13 = 0x13,   //动态卷
        DT_14 = 0x14,   //芯片(NAND FLASH)
        DT_15 = 0x15,   //虚拟磁盘
        DT_16 = 0x16,   //E01文件
        DT_17 = 0x17,   //AFF文件
        DT_18 = 0x18,   //ssd硬盘	
        DT_19 = 0x19,   //光盘
        DT_1A = 0x1A,   //dd镜像文件
        DT_1B = 0x1B,   //手机芯片( 联发科手机芯片(mtk), 展讯手机芯片
        DT_80 = 0x80    //效率源USB3.0设备
    }
    #endregion
}
