using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 腾讯微博
    /// </summary>
    [Serializable]
    public class TencentMicroBlog : MessageCore
    {
        /// <summary>
        /// 转发次数
        /// </summary>
        [Display]
        public string ForwardCount { get; set; }

        [Display]
        public string ForwardNick { get; set; }

        [Display]
        public string ContentUrl { get; set; }
    }

    /// <summary>
    /// 腾信私信
    /// </summary>
    [Serializable]
    public class TencentLetters : MessageCore
    {
        /// <summary>
        /// 附件地址
        /// </summary>
        [Display]
        public string AttachUrl { get; set; }
    }

    /// <summary>
    /// 微博联系人列表。
    /// </summary>
    [Serializable]
    public class TencentMicroBlogUser : AbstractDataItem
    {

        /// <summary>
        /// 微博账号。
        /// </summary>
        [Display]
        public string MicroBlogId { get; set; }

        /// <summary>
        /// 昵称。
        /// </summary>
        [Display]
        public string Nick { get; set; }

        /// <summary>
        /// Gender.
        /// </summary>
        [Display]
        public string Gender { get; set; }

        /// <summary>
        /// 是否为Vip用户。
        /// </summary>
        [Display]
        public string IsVip { get; set; }

        /// <summary>
        /// 头像地址。
        /// </summary>
        [Display]
        public string Portrait { get; set; }

        /// <summary>
        /// 关联时间。
        /// </summary>

        public DateTime? RelateTime { get; set; }

        [Display]
        public string _RelateTime
        {
            get { return this.RelateTime.ToDateTimeString(); }
        }

        /// <summary>
        /// 关联的QQ号码。
        /// </summary>
        public string QQNumber { get; set; }
    }

    [Serializable]
    public class MicroBlogComment : AbstractDataItem
    {

        [Display]
        public string Commentor { get; set; }

        [Display(Key = "MicroBlogContent")]
        public string Content { get; set; }

        [Display]
        public string OriMicroBlogComent { get; set; }

        [Display]
        public string OriSender { get; set; }

        public DateTime? Time { get; set; }

        [Display]
        public string _Time
        {
            get { return this.Time.ToDateTimeString(); }
        }
    }

    [Serializable]
    public class CommentType
    {
        [Display]
        public string OneType { get; set; }
    }
}
