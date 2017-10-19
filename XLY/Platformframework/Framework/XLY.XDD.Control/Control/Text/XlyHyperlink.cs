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
using System.Diagnostics;
using System.Utility.Logger;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 超链接
    /// </summary>
    public class XlyHyperlink : System.Windows.Controls.Control
    {
        static XlyHyperlink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyHyperlink), new FrameworkPropertyMetadata(typeof(XlyHyperlink)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(XlyHyperlink_PreviewMouseLeftButtonUp);
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(XlyHyperlink_PreviewMouseLeftButtonUp);
        }

        private void XlyHyperlink_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!this.IsActionEnabled)
                    return;

                Process.Start(string.IsNullOrWhiteSpace(this.Hyperlink) ? this.Text : this.Hyperlink);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Click Hyperlink error.", ex);
            }
        }

        #region Text -- 内容

        /// <summary>
        /// 内容
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(XlyHyperlink), new UIPropertyMetadata(null));

        #endregion

        #region Hyperlink -- 链接

        /// <summary>
        /// 链接
        /// </summary>
        public string Hyperlink
        {
            get { return (string)GetValue(HyperlinkProperty); }
            set { SetValue(HyperlinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Hyperlink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HyperlinkProperty =
            DependencyProperty.Register("Hyperlink", typeof(string), typeof(XlyHyperlink), new UIPropertyMetadata(null));

        #endregion

        #region IsActionEnabled -- 是否启用行为

        /// <summary>
        /// 是否启用行为
        /// </summary>
        public bool IsActionEnabled
        {
            get { return (bool)GetValue(IsActionEnabledProperty); }
            set { SetValue(IsActionEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsActionEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActionEnabledProperty =
            DependencyProperty.Register("IsActionEnabled", typeof(bool), typeof(XlyHyperlink), new UIPropertyMetadata(true));

        #endregion
    }
}
