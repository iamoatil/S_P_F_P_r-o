using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：IMail  
* Author     ：Fhjun
* Create Date：2017/3/24 14:35:55
* ==============================================================================*/

namespace XLY.SF.Project.Domains.Contract.DataItemContract
{
    /// <summary>
    /// 邮件模式接口
    /// </summary>
    public interface IMail
    {
        /// <summary>
        /// 发件人
        /// </summary>
        string Sender { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        string Receiver { get; set; }

        /// <summary>
        /// 发件时间
        /// </summary>
        DateTime? StartDate { get; set; }

        /// <summary>
        /// 收件时间
        /// </summary>
        DateTime? RecvDataTime { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        string TextContent { get; set; }

        /// <summary>
        /// 附件清单
        /// </summary>
        IEnumerable<string> Attachments { get; set; }

        /// <summary>
        /// 附件路径或下载地址
        /// </summary>
        IEnumerable<string> AttachmentPaths { get; set; }
    }
}
