using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XLY.XDD.Control
{
    /// <summary>
    /// Xly矩形
    /// </summary>
    public class XlyRectangle : System.Windows.Controls.Control
    {
        static XlyRectangle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyRectangle), new FrameworkPropertyMetadata(typeof(XlyRectangle)));
        }

        #region Orientation -- 排列方式

        /// <summary>
        /// 排列方式
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(XlyRectangle), new UIPropertyMetadata(Orientation.Horizontal));

        #endregion

        #region Maximum -- 最大值

        /// <summary>
        /// 最大值
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(XlyRectangle), new UIPropertyMetadata(1d));

        #endregion

        #region Minimum -- 最小值

        /// <summary>
        /// 最小值
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(XlyRectangle), new UIPropertyMetadata(0d));

        #endregion

        #region Value -- 当前值

        /// <summary>
        /// 当前值
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(XlyRectangle), new UIPropertyMetadata(0d));

        #endregion

        #region ValueBrush -- 值画刷

        /// <summary>
        /// 值画刷
        /// </summary>
        public Brush ValueBrush
        {
            get { return (Brush)GetValue(ValueBrushProperty); }
            set { SetValue(ValueBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueBrushProperty =
            DependencyProperty.Register("ValueBrush", typeof(Brush), typeof(XlyRectangle), new UIPropertyMetadata(new SolidColorBrush(Colors.Red)));

        #endregion
    }
}
