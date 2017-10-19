using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文件预览视图参数
    /// </summary>
    public interface IFileViewerArgs
    {
        /// <summary>
        /// 要打开文件的流
        /// </summary>
        System.IO.Stream Stream { get; set; }

        /// <summary>
        /// 要打开文件的Buffer
        /// </summary>
        byte[] Buffer { get; set; }

        /// <summary>
        /// 要打开文件的路径
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// 要打开文件的扩展名
        /// </summary>
        string Extension { get; set; }

        /// <summary>
        /// 要打开文件的编码方式
        /// </summary>
        Encoding Encoding { get; set; }

        /// <summary>
        /// 打开类型
        /// </summary>
        FileViewerArgsType Type { get; set; }
    }
}
