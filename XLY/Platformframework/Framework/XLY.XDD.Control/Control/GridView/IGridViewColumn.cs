using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 列表列信息
    /// </summary>
    public interface IGridViewColumn
    {
        /// <summary>
        /// 列头
        /// </summary>
        string Header { get; set; }

        /// <summary>
        /// 绑定的字段名
        /// </summary>
        string FieldName { get; set; }

        /// <summary>
        /// 是否使用动态绑定（数据源是动态类型或者类型不确定时使用）
        /// </summary>
        bool IsDynamic { get; set; }

        /// <summary>
        /// 是否支持排序
        /// </summary>
        bool IsSort { get; set; }

        /// <summary>
        /// 是否支持去重（指示该字段在去重是将被使用）
        /// </summary>
        bool IsDistinct { get; set; }

        /// <summary>
        /// 期望的列宽
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// 创建之后保留的对真实列的引用
        /// </summary>
        object Column { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// 列位序
        /// </summary>
        int VisibleIndex { get; set; }

        bool IsFileNameColor { get; set; }

        bool IsFilex { get; set; }
    }
}
