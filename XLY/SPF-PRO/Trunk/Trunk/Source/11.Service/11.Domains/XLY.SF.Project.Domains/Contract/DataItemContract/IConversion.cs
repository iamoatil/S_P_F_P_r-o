using System;

/* ==============================================================================
* Description：IConversion  
* Author     ：Fhjun
* Create Date：2017/3/24 14:33:50
* ==============================================================================*/

namespace XLY.SF.Project.Domains.Contract.DataItemContract
{
    /// <summary>
    /// 会话模式接口
    /// </summary>
    public interface IConversion
    {
        /// <summary>
        /// 发送者姓名
        /// </summary> 
        string SenderName { get; set; }

        /// <summary>
        /// 发送者图片
        /// </summary>
        string SenderImage { get; set; }

        /// <summary>
        /// 时间，可空
        /// </summary>
        DateTime? Date { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        EnumColumnType Type { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// 发送状态
        /// </summary>
        EnumSendState SendState { get; set; }
    }
}
