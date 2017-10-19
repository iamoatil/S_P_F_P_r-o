using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 表视图列类型
    /// </summary>
    public enum GridViewColumnType
    {
        /// 字符类型
        /// </summary>
        String,
        /// <summary>
        /// Bool类型
        /// </summary>
        Bool,
        /// <summary>
        /// 超链接
        /// </summary>
        Hyperlink,
        /// <summary>
        /// 图片类型
        /// </summary>
        Image,
        /// <summary>
        /// 缩略图
        /// </summary>
        ThumbnailImage,
        /// <summary>
        /// 自定义
        /// </summary>
        Custom,
        /// <summary>
        /// 日期时间型
        /// </summary>
        DateTime,
        /// <summary>
        /// 枚举类型
        /// </summary>
        Enum,
        /// <summary>
        /// 数字类型
        /// </summary>
        Int
    }
}
