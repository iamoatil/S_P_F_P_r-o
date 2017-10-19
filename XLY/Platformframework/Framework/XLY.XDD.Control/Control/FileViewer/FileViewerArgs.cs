using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文件打开参数
    /// </summary>
    public class FileViewerArgs : IFileViewerHighlightArgs
    {
        /// <summary>
        /// 要打开的文件流
        /// </summary>
        public System.IO.Stream Stream { get; set; }

        /// <summary>
        /// 要打开的文件Buffer
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// 要打开的文件路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 要打开的文件扩展名
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 要打开文件的编码方式
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 打开类型
        /// </summary>
        public FileViewerArgsType Type { get; set; }

        /// <summary>
        /// 高亮类型
        /// </summary>
        public XlyHighlightMode HighlightMode { get; set; }

        /// <summary>
        /// 高亮文本
        /// </summary>
        public string HighlightText { get; set; }

        /// <summary>
        /// 高亮偏移量
        /// </summary>
        public int HighlightOffset { get; set; }

        /// <summary>
        /// 高亮长度
        /// </summary>
        public int HighlightLength { get; set; }
    }
}
