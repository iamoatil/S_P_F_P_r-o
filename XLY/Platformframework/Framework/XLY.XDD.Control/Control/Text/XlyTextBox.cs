using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文本输入框
    /// </summary>
    [TemplatePart(Name = "PART_Message", Type = typeof(TextBlock))]
    public class XlyTextBox : TextBox
    {
        static XlyTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyTextBox), new FrameworkPropertyMetadata(typeof(XlyTextBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Message = this.Template.FindName("PART_Message", this) as TextBlock;
        }

        private TextBlock PART_Message;

        #region HintMessage -- 默认提示信息

        public static readonly DependencyProperty HintMessageProperty =
            DependencyProperty.Register("HintMessage", typeof(string), typeof(XlyTextBox), new PropertyMetadata(null));

        /// <summary>
        /// 默认提示信息
        /// </summary>
        public string HintMessage
        {
            get { return (string)GetValue(HintMessageProperty); }
            set { SetValue(HintMessageProperty, value); }
        }

        #endregion

        #region HintForeground -- 提示前景色

        /// <summary>
        /// 提示前景色
        /// </summary>
        public Brush HintForeground
        {
            get { return (Brush)GetValue(HintForegroundProperty); }
            set { SetValue(HintForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HintForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HintForegroundProperty =
            DependencyProperty.Register("HintForeground", typeof(Brush), typeof(XlyTextBox), new UIPropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88ffffff"))));

        #endregion

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (this.PART_Message != null)
            {
                if (this.Text.IsNullOrEmptyOrWhiteSpace())
                {
                    this.PART_Message.Visibility = Visibility.Visible;
                }
                else
                {
                    this.PART_Message.Visibility = Visibility.Collapsed;
                }
            }
            base.OnLostFocus(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (this.PART_Message != null)
            {
                this.PART_Message.Visibility = Visibility.Collapsed;
            }
            base.OnGotFocus(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (this.PART_Message != null)
            {
                if (this.Text.IsNullOrEmptyOrWhiteSpace() && !this.IsFocused)
                {
                    this.PART_Message.Visibility = Visibility.Visible;
                }
                else
                {
                    this.PART_Message.Visibility = Visibility.Collapsed;
                }
            }

            base.OnTextChanged(e);
        }
    }
}
