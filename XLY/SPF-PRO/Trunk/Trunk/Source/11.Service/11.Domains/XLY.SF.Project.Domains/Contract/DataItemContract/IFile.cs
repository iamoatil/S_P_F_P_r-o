using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Description：IFile  
* Author     ：Fhjun
* Create Date：2017/3/24 14:48:45
* ==============================================================================*/

namespace XLY.SF.Project.Domains.Contract.DataItemContract
{
    /// <summary>
    /// 文件数据接口
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// 文件类型
        /// </summary>
        EnumColumnType Type { get; set; }

        /// <summary>
        /// 本地文件路径
        /// </summary>
        string LocalFilePath { get; set; }

        /// <summary>
        /// 手机文件路径
        /// </summary>
        string AbstractFilePath { get; set; }

        /// <summary>
        /// 文件MD5值
        /// </summary>
        string FileMd5 { get; set; }
    }
}
