using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    #region EnumDataState (数据状态)
    /// <summary>
    /// 数据状态
    /// </summary>
    public enum EnumDataState
    {

        /// <summary>
        /// 未知
        /// </summary>
        None = 0,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 2,

        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = 1,

        /// <summary>
        /// 碎片
        /// </summary>
        Fragment = 4,
    }
    #endregion

    #region EnumEncodingType（字符编码方式）


    #endregion

    #region EnumAlignment（对齐方式）

    /// <summary>
    /// 对齐方式
    /// </summary>
    public enum EnumAlignment
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 左对齐
        /// </summary>
        Left = 1,

        /// <summary>
        /// 居中
        /// </summary>
        Center = 2,

        /// <summary>
        /// 右对齐
        /// </summary>
        Right = 3
    }

    #endregion

    #region EnumColumnType （列数据类型）

    /// <summary>
    /// 列数据类型
    /// </summary>
    public enum EnumColumnType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        String = 0,
        /// <summary>
        /// 整形
        /// </summary>
        Int = 1,
        /// <summary>
        /// 浮点型
        /// </summary>
        Double = 2,
        /// <summary>
        /// 日期时间
        /// </summary>
        DateTime = 3,
        /// <summary>
        /// 图片
        /// </summary>
        Image = 4,
        /// <summary>
        /// URL
        /// </summary>
        URL = 5,
        /// <summary>
        /// 视频
        /// </summary>
        Video = 6,
        /// <summary>
        /// 音频
        /// </summary>
        Audio = 7,
        /// <summary>
        /// 文档
        /// </summary>
        Word = 17,
        /// <summary>
        /// html
        /// </summary>
        HTML = 8,
        /// <summary>
        /// 枚举
        /// </summary>
        Enum = 9,
        /// <summary>
        /// 地理位置
        /// </summary>
        Location = 10,
        /// <summary>
        /// 名片
        /// </summary>
        Card = 11,
        /// <summary>
        /// 视频聊天
        /// </summary>
        VideoChat = 12,
        /// <summary>
        /// 邮件
        /// </summary>
        Mail = 13,
        /// <summary>
        /// 集合
        /// </summary>
        List = 14,
        /// <summary>
        /// 系统消息
        /// </summary>
        System = 15,
        /// <summary>
        /// 未知类型
        /// </summary>
        None = 16,
        /// <summary>
        /// 视频通话
        /// </summary>
        VideoCall = 18,
        /// <summary>
        /// 文件
        /// </summary>
        File = 19,
        /// <summary>
        /// 语音聊天
        /// </summary>
        AudioCall = 20,
        /// <summary>
        /// 微信红包
        /// </summary>
        WeChatRedPack = 21,
        /// <summary>
        /// 表情
        /// </summary>
        Emoji = 22,
        /// <summary>
        /// 缩略图
        /// </summary>
        Thumbnail = 23,
        /// <summary>
        /// 微信转账
        /// </summary>
        WeChatTransfer = 24,
        /// <summary>
        /// 微信支付
        /// </summary>
        WeChatZhifu = 25,
        /// <summary>
        /// 公众号
        /// </summary>
        GongZhongHao = 26,
    }

    #endregion

    #region EnumOrder（排序类型）

    /// <summary>
    /// 排序类型
    /// </summary>
    public enum EnumOrder
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 升序
        /// </summary>
        Asc = 1,

        /// <summary>
        /// 降序
        /// </summary>
        Desc = 2,
    }

    #endregion

    #region EnumSendState（信息发送状态）

    /// <summary>
    /// 信息发送状态
    /// </summary>
    public enum EnumSendState
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("None")]
        None = 0,

        /// <summary>
        /// 接收
        /// </summary>
        [Description("Receive")]
        Receive = 1,

        /// <summary>
        /// 发送
        /// </summary>
        [Description("Send")]
        Send = 2,
    }

    #endregion

    #region EnumDisplayVisible (列数据显示方式)
    /// <summary>
    /// 列数据显示方式，为数据库可见/界面显示可见。
    /// 解决枚举列的问题
    /// </summary>
    public enum EnumDisplayVisibility
    {
        /// <summary>
        /// 数据库可见/界面显示可见，默认值
        /// </summary>
        Visible = 0,
        /// <summary>
        /// 仅为数据库可见，不需要展示给用户，但需要存储到数据库中
        /// </summary>
        ShowInDatabase = 1,
        /// <summary>
        /// 仅为界面显示可见，不需要存储
        /// </summary>
        ShowInUI = 2,
    }
    #endregion
}
