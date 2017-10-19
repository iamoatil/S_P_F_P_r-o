using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Language;

namespace XLY.SF.Project.Domains
{
    #region 关注列表
    /// <summary>
    /// 关注列表
    /// follow_table
    /// </summary>
    [Serializable]
    public class SinaMicoBlogUser : AbstractDataItem
    {
        /// <summary>
        /// 被关注用户Id
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 被关注用户昵称
        /// </summary>
        [Display]
        public string Nick { get; set; }

        /// <summary>
        /// 被关注用户是否是VIP
        /// </summary>
        [Display(Key = "isVip")]
        public string IsVip { get; set; }

        /// <summary>
        /// 被关注分组名
        /// </summary>
        [Display(Key = "FriendGroupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [Display]
        public string Level { get; set; }

        /// <summary>
        /// 被关注头像存储地址
        /// </summary>
        [Display]
        public string Portrait { get; set; }

        [Display]
        public string Introduction { get; set; }
    }
    #endregion

    #region 发微博
    /// <summary>
    /// 发送微博流水记录（所有账号的发送信息——多个账号使用这个手机）
    /// home_table
    /// </summary>
    [Serializable]
    public class SinaMicroBlogContent : MessageCore
    {
        /// <summary>
        /// 微博内容图片存储地址
        /// </summary>
        [Display]
        public string PicPath { get; set; }

        /// <summary>
        /// 来源平台(手机客户端，网页或者其他)
        /// </summary>
        [Display]
        public string SendPlatform { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [Display]
        public string Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [Display]
        public string Latitude { get; set; }

        /// <summary>
        /// 被转发内容用户的昵称
        /// </summary>
        [Display]
        public string NickForwarded { get; set; }

        /// <summary>
        /// 被转发原因
        /// </summary>
        [Display]
        public string ReasonForwarded { get; set; }

        /// <summary>
        /// 转发次数
        /// </summary>
        [Display]
        public string CountForwarded { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        [Display]
        public string CountComment { get; set; }

    }
    #endregion

    #region 私信
    /// <summary>
    /// 私信
    /// </summary>
    [Serializable]
    public sealed class SinaMicroBlogLetter : MessageCore
    {
        /// <summary>
        /// 附件名
        /// </summary>
        [Display]
        public string AttachName { get; set; }

        /// <summary>
        /// 附件大小
        /// </summary>
        [Display]
        public string AttachSize { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        [Display]
        public string AttachType { get; set; }

        /// <summary>
        /// 附件存储地址
        /// </summary>
        [Display]
        public string AttachUrl { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [Display]
        public string Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [Display]
        public string Longituid { get; set; }

        /// <summary>
        /// 本地时间
        /// </summary>
        public DateTime? _ReadTime { get; set; }

        [Display]
        public string ReadTime
        {
            get { return this._ReadTime.ToDateTimeString(); }
        }

        /// <summary>
        /// 是否发送成功
        /// 1.成功
        /// 0.失败
        /// </summary>
        [Display]
        public string IsSendSuccess { get; set; }
    }

    #endregion

    /// <summary>
    /// 群组
    /// </summary>
    [Serializable]
    public class SinaMicoBlogGroup : AbstractDataItem
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 群组名称
        /// </summary>
        [Display]
        public string GroupName { get; set; }

        /// <summary>
        /// 群头像
        /// </summary>
        [Display(Key = "Avatar")]
        public string GroupAvatar { get; set; }


        /// <summary>
        /// 群组人数
        /// </summary>
        [Display]
        public string GroupCount { get; set; }

        /// <summary>
        /// 群主
        /// </summary>
        [Display]
        public string GroupOwner { get; set; }

    }

    /// <summary>
    /// 搜索记录
    /// </summary>
    [Serializable]
    public class SinaMicoBlogSearchEntry : AbstractDataItem
    {

        [Display]
        public DateTime? SearchTime { get; set; }

        [Display]
        public string Content { get; set; }

        [Display]
        public string ResultDesc
        {
            get { return ResultType == "0" ? LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_ResultType_YouXiaoShuJu) : LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_ResultType_WuXiaoShuJu); }
        }

        public string ResultType { get; set; }
    }
}
