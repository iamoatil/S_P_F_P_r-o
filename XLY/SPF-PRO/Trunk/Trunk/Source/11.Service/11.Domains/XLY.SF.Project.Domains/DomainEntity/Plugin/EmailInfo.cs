using System;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 邮件实体。
    /// </summary>
    [Serializable]
    public class EmailInfo : AbstractDataItem
    {
        /// <summary>
        /// 发送者。
        /// </summary>
        [Display]
        public string Sender { get; set; }

        /// <summary>
        /// 接收者。
        /// </summary>
        [Display]
        public string Receiver { get; set; }

        /// <summary>
        /// 主题。
        /// </summary>
        [Display(ColumnType = EnumColumnType.String)]
        public string Subject { get; set; }

        /// <summary>
        /// 邮件内容。
        /// </summary>
        [Display]
        public string TextContent { get; set; }

        /// <summary>
        /// 发送时间。
        /// </summary>
        [Display(Alignment = EnumAlignment.Center)]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 接收时间。
        /// </summary>
        public DateTime? RecvDataTime { get; set; }
        [Display(Alignment = EnumAlignment.Center)]
        public string _RecvDataTime
        {
            get { return this.RecvDataTime.ToDateTimeString(); }
        }
        
        /// <summary>
        /// 详细信息
        /// </summary>
        public string DetailContent
        {
            get { return this.ToString(); }
        }

        /// <summary>
        /// 附件清单
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> Attachments { get; set; }

        /// <summary>
        /// 附件地址
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> AttachmentPaths { get; set; }
    }

    [Serializable]
    public class EmailAccount
    {
        public int Id { get; set; }

        [Display]
        public string Nick { get; set; }

        [Display]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public EnumDataState DataState { get; set; }
    }
}
