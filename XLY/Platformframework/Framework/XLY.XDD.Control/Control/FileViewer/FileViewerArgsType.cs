using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文件打开参数类型
    /// </summary>
    public enum FileViewerArgsType
    {
        /// <summary>
        /// 采用文件路径打开
        /// </summary>
        Path,
        /// <summary>
        /// 采用流打开（在该模式下需要指定打开文件的格式）
        /// </summary>
        Stream,
        /// <summary>
        /// 采用Buffer打开（在该模式下需要指定打开文件的格式）
        /// </summary>
        Buffer
    }
}
