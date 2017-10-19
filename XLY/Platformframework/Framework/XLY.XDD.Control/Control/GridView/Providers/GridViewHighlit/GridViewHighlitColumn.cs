using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 高亮列
    /// </summary>
    public class GridViewHighlitColumn : GridViewColumnBase
    {
        #region HighlightText -- 高亮文本

        private string _HighlightText;
        /// <summary>
        /// 高亮文本
        /// </summary>
        public string HighlightText
        {
            get { return _HighlightText; }
            set { _HighlightText = value; this.OnPropertyChanged("HighlightText"); }
        }

        #endregion

        #region HighlightTextForeground -- 高亮字体颜色

        private Brush _HighlightTextForeground;
        /// <summary>
        /// 高亮字体颜色
        /// </summary>
        public Brush HighlightTextForeground
        {
            get { return _HighlightTextForeground; }
            set { _HighlightTextForeground = value; this.OnPropertyChanged("HighlightTextForeground"); }
        }

        #endregion

        #region HighlightTextBrush -- 高亮背景色

        private Brush _HighlightTextBrush;
        /// <summary>
        /// 高亮背景色
        /// </summary>
        public Brush HighlightTextBrush
        {
            get { return _HighlightTextBrush; }
            set { _HighlightTextBrush = value; this.OnPropertyChanged("HighlightTextBrush"); }
        }

        #endregion

        #region IsIgnoringCase -- 是否忽略大小写

        private bool? _IsIgnoringCase;
        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        public bool? IsIgnoringCase
        {
            get { return _IsIgnoringCase; }
            set { _IsIgnoringCase = value; this.OnPropertyChanged("IsIgnoringCase"); }
        }

        #endregion
    }
}
