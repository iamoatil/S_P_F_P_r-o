using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文件视图高亮
    /// </summary>
    public interface IFileViewerHighlightArgs : IFileViewerArgs
    {
        /// <summary>
        /// 高亮类型
        /// </summary>
        XlyHighlightMode HighlightMode { get; set; }

        /// <summary>
        /// 高亮文本
        /// </summary>
        string HighlightText { get; set; }

        /// <summary>
        /// 高亮偏移量
        /// </summary>
        int HighlightOffset { get; set; }

        /// <summary>
        /// 高亮长度
        /// </summary>
        int HighlightLength { get; set; }
    }
}
