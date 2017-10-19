using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文件预览类型
    /// </summary>
    public enum FileViewerType
    {
        /// <summary>
        /// 空
        /// </summary>
        None,
        /// <summary>
        /// 视图容器（延时容器）
        /// </summary>
        Container,
        /// <summary>
        /// 视图合并容器
        /// </summary>
        MultiContainer,
        /// <summary>
        /// Excel文档
        /// </summary>
        Excel,
        /// <summary>
        /// Html文档
        /// </summary>
        Html,
        /// <summary>
        /// 媒体播放器
        /// </summary>
        Media,
        /// <summary>
        /// Pdf文档
        /// </summary>
        Pdf,
        /// <summary>
        /// Ppt文档
        /// </summary>
        Ppt,
        /// <summary>
        /// Word文档
        /// </summary>
        Word,
        /// <summary>
        /// 文件详细信息
        /// </summary>
        Detail,
        /// <summary>
        /// 图片
        /// </summary>
        Image,
        /// <summary>
        /// 二进制
        /// </summary>
        Hex,
        /// <summary>
        /// 文本预览
        /// </summary>
        Text,
        /// <summary>
        /// 地图
        /// </summary>
        Map
    }
}
