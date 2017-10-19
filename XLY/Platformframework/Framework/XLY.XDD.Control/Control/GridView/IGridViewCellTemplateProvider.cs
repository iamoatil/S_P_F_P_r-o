using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 列表样式生成器
    /// </summary>
    public interface IGridViewCellTemplateProvider
    {
        /// <summary>
        /// 根据列信息生成单元格模版
        /// </summary>
        /// <param name="column">列信息</param>
        /// <returns>模版字符串</returns>
        DataTemplate CreateCellTemplate(IGridViewColumn column);

        /// <summary>
        /// 获取转换器xmal解析上下文
        /// </summary>
        /// <returns></returns>
        ParserContext GetConvertParsercontext();

        /// <summary>
        /// 默认的字体颜色
        /// </summary>
        Brush DefaultForeground { get; set; }

        /// <summary>
        /// 默认的字体大小
        /// </summary>
        double DefaultFontSize { get; set; }

        /// <summary>
        /// 默认字体
        /// </summary>
        FontFamily DefaultFontFamily { get; set; }

        /// <summary>
        /// 默认最大单元格高度
        /// </summary>
        double DefaultCellMaxHeight { get; set; }
    }
}
