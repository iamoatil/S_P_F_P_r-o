using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/16 18:05:53
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.Domains
{
    #region EnumState：状态枚举
    /// <summary>
    /// 状态枚举
    /// </summary>
    public enum EnumState
    {
        /// <summary>
        /// 正常使用
        /// </summary>
        [Description("LANGKEY_ZhengChangShiYong_00336")]
        Used = 1,
        /// <summary>
        /// 冻结
        /// </summary>
        [Description("LANGKEY_DongJie_00337")]
        Disable = 2,
    }
    #endregion

    #region EnumSex：性别
    /// <summary>
    /// 性别
    /// </summary>
    public enum EnumSex
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("LANGKEY_WeiZhi_00338")]
        None = 0,
        /// <summary>
        /// 男
        /// </summary>
        [Description("LANGKEY_Nan_00339")]
        Male = 1,
        /// <summary>
        /// 女
        /// </summary>
        [Description("LANGKEY_Nv_00340")]
        Female = 2,
    }
    #endregion

    #region 日志类型 EnumLogType
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum EnumLogType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("LANGKEY_Wu_00341")]
        None = 0,

        /// <summary>
        /// 系统日志
        /// </summary>
        SystemLog = 1,
        /// <summary>
        /// 操作日志
        /// </summary>
        Operation = 2
    }
    #endregion

    #region EnumOperator 运营商类型
    /// <summary>
    /// 运营商类型
    /// </summary>
    public enum EnumOperator
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("LANGKEY_WeiZhi_00342")]
        None = 0,

        /// <summary>
        /// 中国联通
        /// </summary>
        [Description("LANGKEY_ZhongGuoLianTong_00343")]
        Unicom = 1,

        /// <summary>
        /// 中国移动
        /// </summary>
        [Description("LANGKEY_ZhongGuoYiDong_00344")]
        Mobile = 2,

        /// <summary>
        /// 中国电信
        /// </summary>
        [Description("LANGKEY_ZhongGuoDianXin_00345")]
        Telecom = 3,
    }
    #endregion

    #region EnumCallType：通话来电类型
    /// <summary>
    /// 通话来电类型
    /// </summary>
    public enum EnumCallType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("Unknown")]
        None = 0,

        /// <summary>
        /// 呼入
        /// </summary>
        [Description("CallIn")]
        CallIn = 1,

        /// <summary>
        /// 呼出
        /// </summary>
        [Description("CallOut")]
        CallOut = 2,

        /// <summary>
        /// 呼入未接
        /// </summary>
        [Description("MissedCallIn")]
        MissedCallIn = 3,

        /// <summary>
        /// 呼入未接(未查看)
        /// </summary>
        [Description("MissedCallInNotViewed")]
        MissedCallInNotViewed = 4,

        /// <summary>
        /// 呼入未接(未打开)
        /// </summary>
        [Description("MissedCallInAlreadyOpen")]
        MissedCallInAlreadyOpen = 5,

        /// <summary>
        /// 呼出未接
        /// </summary>
        [Description("MissedCallOut")]
        MissedCallOut = 9,
    }
    #endregion

    #region EnumSMSState：短信状态
    /// <summary>
    /// 短信状态
    /// </summary>
    public enum EnumSMSState
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("None")]
        None = 0,

        /// <summary>
        /// 接收的短信
        /// </summary>
        [Description("ReceiveSMS")]
        ReceiveSMS = 1,

        /// <summary>
        /// 发送的短信
        /// </summary>
        [Description("SendSMS")]
        SendSMS = 2,

        /// <summary>
        /// 短信草稿
        /// </summary>
        [Description("DraftSMS")]
        DraftSMS = 3,

        /// <summary>
        /// 接收的彩信
        /// </summary>
        [Description("ReceiveMMS")]
        ReceiveMMS = 151,

        /// <summary>
        /// 发送的彩信
        /// </summary>
        [Description("SendMMS")]
        SendMMS = 137,
    }
    #endregion

    #region EnumReadState：读取状态
    /// <summary>
    /// 读取状态
    /// </summary>
    public enum EnumReadState
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("Unknown")]
        Unknown = 0,

        /// <summary>
        /// 已读
        /// </summary>
        [Description("Read")]
        Read = 1,

        /// <summary>
        /// 未读
        /// </summary>
        [Description("Unread")]
        Unread = 2,

        /// <summary>
        /// 已查看
        /// </summary>
        [Description("AlreadyView")]
        AlreadyView = 3,

        /// <summary>
        /// 未查看
        /// </summary>
        [Description("NotViewed")]
        NotViewed = 4,

        /// <summary>
        /// 成功
        /// </summary>
        [Description("Success")]
        Success = 5,

        /// <summary>
        /// 失败
        /// </summary>
        [Description("Fail")]
        Fail = 6,
    }
    #endregion

    #region EnumSMSType 手机短信类型
    /// <summary>
    /// 手机短信类型
    /// </summary>
    public enum EnumSMSType
    {
        /// <summary>
        /// 短信
        /// </summary>
        [Description("SMS")]
        SMS = 1,

        /// <summary>
        /// 彩信
        /// </summary>
        [Description("MMS")]
        MMS = 2
    }
    #endregion

    #region WebTracesType 浏览器痕迹类别

    /// <summary>
    /// 浏览器痕迹类别
    /// </summary>
    public enum WebTracesType
    {
        [Description("LANGKEY_ShuQian_04515")]
        Bookmark,

        [Description("LANGKEY_LiuLanJiLu_04516")]
        History,

        [Description("LANGKEY_ShouCang_04517")]
        Favorite,

        [Description("LANGKEY_TouTiao_04518")]
        HeadLine,

        [Description("LANGKEY_DingYueHao_04519")]
        SubscriptionNumber,

        [Description("LANGKEY_XiaZaiWenJian_04520")]
        DownloadFile,

        [Description("LANGKEY_SouSuoJiLu_04521")]
        SearchHistory,

        [Description("WebCookie")]
        WebCookie,

        [Description("WebCache")]
        WebCache,

        [Description("WebSitePwd")]
        WebSitePwd
    }
    #endregion

    #region 黑莓备份文件[Manifest.xml]对应UID描述
    /// <summary>
    /// 黑莓备份文件[Manifest.xml]对应UID描述
    /// </summary>
    public enum BlackBerryUidName
    {
        [Description("Address Book - All")]
        AddressBookAll,
        [Description("Phone History")]
        PhoneHistory,
        [Description("Phone Call Logs")]
        PhoneCallLogs,
        [Description("SMS Messages")]
        SMSMessages
    }
    #endregion

    #region 黑莓数据解析底层DLL调用功能反馈错误编码在language.xml中的节点配置对照，描述为底层解析DLL实际反馈编码
    /// <summary>
    /// 黑莓数据解析底层DLL调用功能反馈错误编码在language.xml中的节点配置对照，描述为底层解析DLL实际反馈编码
    /// </summary>
    public enum BlackBerryErrcode
    {
        [Description("10010")]
        BlackBerry_10010,
        [Description("10020")]
        BlackBerry_10020,
        [Description("10030")]
        BlackBerry_10030,
        [Description("10040")]
        BlackBerry_10040,
        [Description("10050")]
        BlackBerry_10050
    }
    #endregion

    #region SwitchTime：开关机类型
    /// <summary>
    /// 开关机类型
    /// </summary>
    public enum EnumSwitchTimeType
    {
        /// <summary>
        /// 开机
        /// </summary>
        [Description("Boot")]
        Boot = 0,
        /// <summary>
        /// SIM卡改变时间
        /// </summary>
        [Description("SIMChange")]
        SIMChange = 0,
        /// <summary>
        /// 关机
        /// </summary>
        [Description("Shutdown")]
        Shutdown = 1
    }
    #endregion
}
