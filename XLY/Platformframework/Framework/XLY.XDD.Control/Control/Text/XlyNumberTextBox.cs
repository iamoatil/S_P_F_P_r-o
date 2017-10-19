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
    /// 数字输入框
    /// </summary>
    public class XlyNumberTextBox : TextBox
    {
        static XlyNumberTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyNumberTextBox), new FrameworkPropertyMetadata(typeof(XlyNumberTextBox)));
        }

        public XlyNumberTextBox()
        {
            this.KeyDown += new KeyEventHandler(XlyNumberTextBox_KeyDown);
            this.TextChanged += new TextChangedEventHandler(XlyNumberTextBox_TextChanged);
        }

        private void XlyNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double d;
            if (!double.TryParse(this.Text, out d))
            {
                this.IsError = true;
                return;
            }
            if (!this.IsDecimal && d > Math.Floor(d))
            {
                this.IsError = true;
                return;
            }
            if (d < this.MinValue || d > this.MaxValue)
            {
                this.IsError = true;
                return;
            }
            this.IsError = false;
        }

        private void XlyNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                return;
            }
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && (e.Key == Key.V || e.Key == Key.C))
            {
                return;
            }
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9
                || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9
                || e.Key == Key.OemPeriod
                || e.Key == Key.Decimal
                || e.Key == Key.Subtract
                || this.IsSuportNegativeNumber && e.Key == Key.OemMinus))
            {
                e.Handled = true;
            }
        }

        #region IsSuportNegativeNumber --  是否支持负数

        /// <summary>
        /// 是否支持负数
        /// </summary>
        public bool IsSuportNegativeNumber
        {
            get { return (bool)GetValue(IsSuportNegativeNumberProperty); }
            set { SetValue(IsSuportNegativeNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSuportNegativeNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSuportNegativeNumberProperty =
            DependencyProperty.Register("IsSuportNegativeNumber", typeof(bool), typeof(XlyNumberTextBox), new UIPropertyMetadata(false));

        #endregion

        #region MaxValue -- 最大值

        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(XlyNumberTextBox), new UIPropertyMetadata(double.MaxValue));

        #endregion

        #region MinValue -- 最小值

        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(XlyNumberTextBox), new UIPropertyMetadata(double.MinValue));

        #endregion

        #region IsDecimal -- 是否支持小数

        /// <summary>
        /// 是否支持小数
        /// </summary>
        public bool IsDecimal
        {
            get { return (bool)GetValue(IsDecimalProperty); }
            set { SetValue(IsDecimalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDecimal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDecimalProperty =
            DependencyProperty.Register("IsDecimal", typeof(bool), typeof(XlyNumberTextBox), new UIPropertyMetadata(false));

        #endregion

        #region IsError -- 是否处于错误状态

        /// <summary>
        /// 是否处于错误状态
        /// </summary>
        public bool IsError
        {
            get { return (bool)GetValue(IsErrorProperty); }
            set { SetValue(IsErrorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsError.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsErrorProperty =
            DependencyProperty.Register("IsError", typeof(bool), typeof(XlyNumberTextBox), new UIPropertyMetadata(false));

        #endregion
    }
}
