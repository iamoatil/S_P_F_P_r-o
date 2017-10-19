using System;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 信息核心对象。
    /// </summary>
    [Serializable]
    public class MessageCore : AbstractDataItem
    {
        /// <summary>
        /// 发送者
        /// </summary>
        [Display]
        public string SenderName { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        [Display]
        public string Receiver { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [Display]
        public string Content { get; set; }

        /// <summary>
        /// 信息类型（文本、图片、音频）
        /// </summary>
        [Display]
        public string MessageType { get; set; }

        /// <summary>
        /// 信息时间
        /// </summary>
        [Display]
        public DateTime? Date { get; set; }

        public MessageCore()
        {
            DataState = EnumDataState.Normal;
            Type = EnumColumnType.String;
        }

        #region IConversion

        /// <summary>
        /// 用于会话模式的头像
        /// </summary>
        [Display]
        public string SenderImage { get; set; }

        /// <summary>
        /// 用于会话模式的数据类型
        /// </summary>
        [Display]
        public EnumColumnType Type { get; set; }

        /// <summary>
        /// 用于会话模式的信息发送状态
        /// </summary>
        [Display]
        public EnumSendState SendState { get; set; }

        #endregion

    }
}
