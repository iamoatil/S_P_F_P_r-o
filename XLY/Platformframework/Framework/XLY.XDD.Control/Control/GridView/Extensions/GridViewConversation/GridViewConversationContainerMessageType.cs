using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 会话模式消息类型
    /// </summary>
    public enum GridViewConversationContainerMessageType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        String,
        /// <summary>
        /// 图片
        /// </summary>
        Image,
        /// <summary>
        /// 视频
        /// </summary>
        Media,
        /// <summary>
        /// 音频
        /// </summary>
        Audio,
        /// <summary>
        /// 文件
        /// </summary>
        File,
        /// <summary>
        /// 文件夹
        /// </summary>
        Floder,
        /// <summary>
        /// emoji表情
        /// </summary>
        LongImageEmoji,
        /// <summary>
        /// 超链接
        /// </summary>
        Hyperlinks
    }
}
