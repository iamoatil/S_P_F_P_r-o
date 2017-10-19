using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Description：IThumbnail  
* Author     ：Fhjun
* Create Date：2017/3/24 14:37:47
* ==============================================================================*/

namespace XLY.SF.Project.Domains.Contract.DataItemContract
{
    /// <summary>
    /// 缩略图模式接口
    /// </summary>
    public interface IThumbnail
    { 
        /// <summary>
        /// 名称或描述
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 时间，可空
        /// </summary>
        DateTime? Date { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        EnumColumnType Type { get; set; }

        /// <summary>
        /// 数据内容
        /// </summary>
        string Content { get; set; } 
    }
}
