using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 图标按钮
    /// </summary>
    public class XlyIconButton : Button
    {
        #region Icon -- 图标

        /// <summary>
        /// 图标
        /// </summary>
        public Brush Icon
        {
            get { return (Brush)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Brush), typeof(XlyIconButton), new UIPropertyMetadata(null));

        #endregion

        #region IconHover -- 鼠标划过时的图标

        /// <summary>
        /// 鼠标划过时的图标
        /// </summary>
        public Brush IconHover
        {
            get { return (Brush)GetValue(IconHoverProperty); }
            set { SetValue(IconHoverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconHover.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHoverProperty =
            DependencyProperty.Register("IconHover", typeof(Brush), typeof(XlyIconButton), new UIPropertyMetadata(null));

        #endregion
    }
}
