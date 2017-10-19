using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 高亮文本内容列生成器
    /// </summary>
    public class GridViewHighlitCellTemplateProvider : GridViewCellTemplateProvider
    {
        public GridViewHighlitCellTemplateProvider(System.Windows.Controls.Control control)
            : base(control)
        {
            this.DefaultHighlightTextForeground = All_DefaultHighlightTextForeground;
            this.DefaultHighlightTextBrush = All_DefaultHighlightTextBrush;
            this.DefaultIsIgnoringCase = All_DefaultIsIgnoringCase;
        }

        static GridViewHighlitCellTemplateProvider()
        {
            All_DefaultHighlightTextForeground = new SolidColorBrush(Colors.Red);
            All_DefaultHighlightTextBrush = new SolidColorBrush(Colors.Yellow);
            All_DefaultIsIgnoringCase = true;
        }

        public override DataTemplate CreateCellTemplate(IGridViewColumn column)
        {
            if (column is GridViewHighlitColumn)
            {
                GridViewHighlitColumn c = column as GridViewHighlitColumn;
                string template = Resource.HighlitColumnTemplate;

                template = this.ReplaceDefaultArgs(c, template);

                SolidColorBrush highlightTextForeground = (c.HighlightTextForeground ?? this.DefaultHighlightTextForeground) as SolidColorBrush;
                SolidColorBrush highlightTextBrush = (c.HighlightTextBrush ?? this.DefaultHighlightTextBrush) as SolidColorBrush;
                bool isIgnoringCase = c.IsIgnoringCase ?? this.DefaultIsIgnoringCase;
                template = template.Replace("#HighlightText#", this._GetBindingString(c.IsDynamic, c.HighlightText));
                template = template.Replace("#HighlightTextForeground#", highlightTextForeground.Color.ToString());
                template = template.Replace("#HighlightTextBrush#", highlightTextBrush.Color.ToString());
                template = template.Replace("#IsIgnoringCase#", isIgnoringCase.ToString());

                return this.CreateDataTemplate(template);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 默认高亮字体颜色
        /// </summary>
        public Brush DefaultHighlightTextForeground { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认高亮字体颜色
        /// </summary>
        public static Brush All_DefaultHighlightTextForeground { get; set; }

        /// <summary>
        /// 默认高亮背景色
        /// </summary>
        public Brush DefaultHighlightTextBrush { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认高亮背景色
        /// </summary>
        public static Brush All_DefaultHighlightTextBrush { get; set; }

        /// <summary>
        /// 默认是否忽略大小写
        /// </summary>
        public bool DefaultIsIgnoringCase { get; set; }

        /// <summary>
        /// 用于所有的解析器的默认是否忽略大小写
        /// </summary>
        public static bool All_DefaultIsIgnoringCase { get; set; }
    }
}
