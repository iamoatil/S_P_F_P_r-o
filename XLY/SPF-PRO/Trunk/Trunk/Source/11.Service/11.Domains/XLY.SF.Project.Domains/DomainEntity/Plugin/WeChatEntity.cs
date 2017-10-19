using System;
using System.Collections.Generic;
using System.Linq;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 微信登录帐号结构对象
    /// </summary>
    [Serializable]
    public class WeChatLoginShow : AbstractDataItem
    {
        public WeChatLoginShow()
        {
            Gender = EnumSex.None;
        }

        /// <summary>
        /// 微信ID 唯一标识
        /// </summary>
        [Display]
        public string WeChatId { get; set; }

        /// <summary>
        /// 微信帐号 
        /// </summary>
        [Display]
        public string WeChatAccout { get; set; }

        /// <summary>
        /// 联系人显示字符串  昵称(微信号)
        /// </summary>
        public string ShowName
        {
            get { return WeChatAccout.IsValid() ? string.Format("{0}({1})", Nick, WeChatAccout) : string.Format("{0}({1})", Nick, WeChatId); }
        }

        /// <summary>
        /// 微信昵称
        /// </summary>
        [Display]
        public string Nick { get; set; }

        /// <summary>
        /// 微信个性签名
        /// </summary>
        [Display]
        public string Signature { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display]
        public EnumSex Gender { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Display(Key = "Address")]
        public string Address { get; set; }

        /// <summary>
        /// 微信号所绑定的QQ号
        /// </summary>
        [Display(Key = "BindingQQ")]
        public string BindingQQ { get; set; }

        /// <summary>
        /// 微信号所绑定的手机号
        /// </summary>
        [Display(Key = "BindingPhone")]
        public string BindingPhone { get; set; }

        /// <summary>
        /// 微信号所绑定的邮箱号
        /// </summary>
        [Display(Key = "BindingEmail")]
        public string BindingEmail { get; set; }

        /// <summary>
        /// 微信所绑定的微博
        /// </summary>
        [Display(Key = "BindingWeiBo")]
        public string BindingWeiBo { get; set; }

        /// <summary>
        /// 微博昵称
        /// </summary>
        [Display(Key = "BindingWeiBoNick")]
        public string WeiBoNick { get; set; }

        /// <summary>
        /// 本地头像文件路径
        /// </summary>
        [Display]
        public string HeadPng { get; set; }

        /// <summary>
        /// 头像链接
        /// </summary>
        [Display]
        public string HeadUrl { get; set; }

    }

    /// <summary>
    /// 微信好友结构对象
    /// </summary>
    [Serializable]
    public class WeChatFriendShow : AbstractDataItem
    {
        public WeChatFriendShow()
        {
            FriendType = WeChatFriendTypeEnum.None;
            Gender = EnumSex.None;
        }

        /// <summary>
        /// 微信ID
        /// </summary>
        [Display]
        public string WeChatId { get; set; }

        /// <summary>
        /// 微信帐号
        /// </summary>
        [Display]
        public string WeChatAccout { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Display]
        public string Nick { get; set; }

        /// <summary>
        /// 联系人显示字符串  昵称(微信号)
        /// </summary>
        public string ShowName
        {
            get { return WeChatAccout.IsValid() ? string.Format("{0}({1})", Nick, WeChatAccout) : string.Format("{0}({1})", Nick, WeChatId); }
        }

        /// <summary>
        /// 备注名
        /// </summary>
        [Display]
        public string Remark { get; set; }

        [Display]
        public EnumSex Gender { get; set; }

        /// <summary>
        /// 好友类型
        /// </summary>
        [Display]
        public WeChatFriendTypeEnum FriendType { get; set; }

        /// <summary>
        /// 好友头像
        /// </summary>
        [Display]
        public string HeadPng { get; set; }

        /// <summary>
        /// 头像链接
        /// </summary>
        [Display]
        public string HeadUrl { get; set; }

        /// <summary>
        /// 好友所在地理位置
        /// </summary>
        [Display]
        public string Address { get; set; }

        /// <summary>
        /// 签名信息
        /// </summary>
        [Display]
        public string Signature { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display]
        public string Description { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Display]
        public string Email { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [Display]
        public string Mobile { get; set; }

    }

    /// <summary>
    /// 微信好友类型
    /// </summary>
    public enum WeChatFriendTypeEnum
    {
        /// <summary>
        /// 普通好友
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 星标好友
        /// </summary>
        Asterisk = 1,
        /// <summary>
        /// 群聊成员
        /// </summary>
        ChatRoom = 2,
        /// <summary>
        /// 公众号
        /// </summary>
        Subscription = 3,
        /// <summary>
        /// 未知
        /// </summary>
        None = 4,
    }

    /// <summary>
    /// 微信群组结构对象
    /// </summary>
    [Serializable]
    public class WeChatGroupShow : AbstractDataItem
    {
        /// <summary>
        /// 微信号 
        /// </summary>
        [Display]
        public string WeChatId { get; set; }

        /// <summary>
        /// 群名称
        /// </summary>
        [Display]
        public string GroupName { get; set; }

        /// <summary>
        /// 显示字符串  昵称(微信号)
        /// </summary>
        public string ShowName
        {
            get { return GroupName.IsValid() ? string.Format("{0}({1})", GroupName, WeChatId) : WeChatId; }
        }

        /// <summary>
        /// 备注名
        /// </summary>
        [Display]
        public string RemarkName { get; set; }

        /// <summary>
        /// 微信群创建者的微信号
        /// </summary>
        [Display(Key = "Creator")]
        public string GroupOwnerUser { get; set; }

        /// <summary>
        /// 群成员数量
        /// </summary>
        [Display(Key = "MemberCount")]
        public int MemberNum { get; set; }

        /// <summary>
        /// 群成员(数据)
        /// </summary>
        [Display(Key = "Member")]
        public string Member { get; set; }

        /// <summary>
        /// 最近更新时间
        /// </summary>
        [Display]
        public DateTime? LastMessageTime { get; set; }

        /// <summary>
        /// 群公告
        /// </summary>
        [Display(Key = "Notice")]
        public string Notice { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [Display]
        public string HeadPng { get; set; }

        /// <summary>
        /// 头像链接
        /// </summary>
        [Display]
        public string HeadUrl { get; set; }

    }

    /// <summary>
    /// 微信删除消息 只有消息内容
    /// </summary>
    public class DelWeChatMessageCore : AbstractDataItem
    {
        [Display]
        public string Content { get; set; }

    }

    /// <summary>
    /// 漂流瓶联系人
    /// </summary>
    [Serializable]
    public class WeChatBottleContact : AbstractDataItem
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [Display(Key = "NickName")]
        public string NickName { get; set; }

        /// <summary>
        /// 用户的标识
        /// </summary>
        public string Guid { get; set; }
    }

    /// <summary>
    /// 摇一摇
    /// </summary>
    [Serializable]
    public class WeChatShakeItem : AbstractDataItem
    {

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Display(Key = "NickName")]
        public string NickName { get; set; }

        /// <summary>
        /// 所在地
        /// </summary>
        [Display(Key = "Address")]
        public string Address { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [Display(Key = "Signature")]
        public string Signature { get; set; }

        /// <summary>
        /// 相距
        /// </summary>
        [Display(Key = "Distance")]
        public string Distance { get; set; }
    }

    /// <summary>
    /// 银行卡
    /// </summary>
    [Serializable]
    public class WeChatBackCard : AbstractDataItem
    {

        /// <summary>
        /// 银行名称
        /// </summary>
        [Display]
        public string BankName { get; set; }

        /// <summary>
        /// 银行卡类型
        /// </summary>
        [Display]
        public string BankType { get; set; }

        /// <summary>
        /// 银行卡卡号，一般只有后4位
        /// </summary>
        [Display]
        public string Number { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [Display]
        public string Phone { get; set; }
    }

    /// <summary>
    /// 登录过的设备
    /// </summary>
    [Serializable]
    public class WeChatSafeDevice : AbstractDataItem
    {

        /// <summary>
        /// 手机标识
        /// </summary>
        [Display]
        public string Guid { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [Display]
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备系统类型(Android? IOS?)
        /// </summary>
        [Display]
        public string SystemType { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>

        public DateTime? LoginTime { get; set; }
        [Display]
        public string _LoginTime
        {
            get { return this.LoginTime.ToDateTimeString(); }
        }
    }

    /// <summary>
    /// 朋友圈
    /// </summary>
    [Serializable]
    public class WeChatSns : AbstractDataItem
    {
        public WeChatSns()
        {
            MediaList = new List<string>();
        }

        /// <summary>
        /// 微信ID（微信号）
        /// </summary>
        [Display]
        public string WeChatId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Display]
        public string NickName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Display]
        public string TypeDesc { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display]
        public string Content { get; set; }

        /// <summary>
        /// 附件(图片、小视频)
        /// </summary>
        [Display]
        public List<string> MediaList { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 定位
        /// </summary>
        [Display]
        public string Location { get; set; }

    }

    /// <summary>
    /// IOS微信朋友圈
    /// </summary>
    [Serializable]
    public class WeChatIosSns : AbstractDataItem
    {

        /// <summary>
        /// 创建者 昵称(帐号)
        /// </summary>
        [Display]
        public string Creater { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        [Display]
        public DateTime? Createtime { get; set; }

        /// <summary>
        /// 发表内容
        /// </summary>
        [Display]
        public string Content { get; set; }

        /// <summary>
        /// 定位信息
        /// </summary>
        [Display]
        public string LocationInfo { get; set; }

        /// <summary>
        /// 附件(图片、小视频)
        /// </summary>
        [Display]
        public List<string> MediaList { get; set; }

        /// <summary>
        /// 附件数量(图片、小视频)
        /// </summary>
        [Display]
        public int MediaCount { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Display]
        public string Comment { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        [Display]
        public int CommentCount { get; set; }

        /// <summary>
        /// 点赞
        /// </summary>
        [Display]
        public string Like { get; set; }

        /// <summary>
        /// 点赞数量
        /// </summary>
        [Display]
        public int LikeCount { get; set; }

        [Display]
        public string LikeFlagDes { get { return LikeFlag ? LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Status_Yes) : LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Status_No); } }

        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool LikeFlag { get; set; }

        /// <summary>
        /// App名称
        /// </summary>
        [Display]
        public string AppName { get; set; }

        /// <summary>
        /// 详细信息
        /// </summary>
        public string DetailStr
        {
            get { return string.Format("{0} {1}:{2}\r\n{3}", Createtime.Value.ToString("yyyy-MM-dd HH:mm:ss"), Creater, Content, Comment); }
        }

    }

    /// <summary>
    /// 我的收藏
    /// </summary>
    [Serializable]
    public class WeChatFavorite : AbstractDataItem
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        [Display]
        public string DataType { get; set; }

        [Display]
        public string Text { get; set; }

        [Display]
        public string Tag { get; set; }

        [Display]
        public string Source { get; set; }

        [Display]
        public string Sender { get; set; }

        [Display]
        public string Receiver { get; set; }

        /// <summary>
        /// 收藏时间
        /// </summary>
        [Display]
        public DateTime? Time { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display]
        public DateTime? UpdateTime { get; set; }

    }

    /// <summary>
    /// 微信小程序
    /// </summary>
    [Serializable]
    public class WeChatAppBrand : AbstractDataItem
    {
        public WeChatAppBrand()
        {
            DataState = EnumDataState.Normal;
        }

        [Display(Key = "Icon", ColumnType = EnumColumnType.Image)]
        public string AppIcon { get; set; }

        [Display(Key = "AppId")]
        public string AppId { get; set; }

        [Display(Key = "Account")]
        public string BrandId { get; set; }

        [Display(Key = "Name")]
        public string AppName { get; set; }

        [Display(Key = "Description")]
        public string Signature { get; set; }

        [Display(Key = "FilePath")]
        public string PkgPath { get; set; }

        [Display(Key = "Source")]
        public string DownloadURL { get; set; }

    }

    /// <summary>
    /// 微信小程序数据
    /// </summary>
    [Serializable]
    public class WeChatAppBrandKVData : AbstractDataItem
    {
        public WeChatAppBrandKVData()
        {
            DataState = EnumDataState.Normal;
        }

        [Display(Key = "Key")]
        public string Key { get; set; }

        [Display(Key = "Type")]
        public string DataType { get; set; }

        [Display]
        public string DataSize { get; set; }

        [Display(Key = "Content")]
        public string Data { get; set; }

    }
}
