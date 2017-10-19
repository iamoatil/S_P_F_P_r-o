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
    /// 提示询问
    /// </summary>
    [TemplatePart(Name = "PART_Yes", Type = typeof(Button))]
    [TemplatePart(Name = "PART_No", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Cancel", Type = typeof(Button))]
    public class XlyMessageBoxQuestion : System.Windows.Controls.Control
    {
        static XlyMessageBoxQuestion()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyMessageBoxQuestion), new FrameworkPropertyMetadata(typeof(XlyMessageBoxQuestion)));
        }

        #region Message -- 消息

        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(XlyMessageBoxQuestion), new UIPropertyMetadata(null));

        #endregion

        /// <summary>
        /// 返回值
        /// </summary>
        public bool? ResultValue { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Yes = this.Template.FindName("PART_Yes", this) as Button;
            this.PART_Yes.Click -= new RoutedEventHandler(PART_Yes_Click);
            this.PART_Yes.Click += new RoutedEventHandler(PART_Yes_Click);

            this.PART_No = this.Template.FindName("PART_No", this) as Button;
            this.PART_No.Click -= new RoutedEventHandler(PART_No_Click);
            this.PART_No.Click += new RoutedEventHandler(PART_No_Click);

            this.PART_Cancel = this.Template.FindName("PART_Cancel", this) as Button;
            this.PART_Cancel.Click -= new RoutedEventHandler(PART_Cancel_Click);
            this.PART_Cancel.Click += new RoutedEventHandler(PART_Cancel_Click);

            this.PART_Yes.Focus();
        }

        private void PART_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.ResultValue = null;
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }

        private void PART_No_Click(object sender, RoutedEventArgs e)
        {
            this.ResultValue = false;
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }

        private void PART_Yes_Click(object sender, RoutedEventArgs e)
        {
            this.ResultValue = true;
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }

        #region PART

        private Button PART_Yes;
        private Button PART_No;
        private Button PART_Cancel;

        #endregion

        #region YesButtonLabel -- Yes按钮的标签

        /// <summary>
        /// Yes按钮的标签
        /// </summary>
        public string YesButtonLabel
        {
            get { return (string)GetValue(YesButtonLabelProperty); }
            set { SetValue(YesButtonLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YesButtonLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YesButtonLabelProperty =
            DependencyProperty.Register("YesButtonLabel", typeof(string), typeof(XlyMessageBoxQuestion), new UIPropertyMetadata("Yes"));

        #endregion

        #region NoButtonLabel -- No按钮的标签

        /// <summary>
        /// No按钮的标签
        /// </summary>
        public string NoButtonLabel
        {
            get { return (string)GetValue(NoButtonLabelProperty); }
            set { SetValue(NoButtonLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoButtonLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoButtonLabelProperty =
            DependencyProperty.Register("NoButtonLabel", typeof(string), typeof(XlyMessageBoxQuestion), new UIPropertyMetadata("No"));

        #endregion

        #region CancelButtonLabel -- 取消按钮的标签

        /// <summary>
        /// 取消按钮的标签
        /// </summary>
        public string CancelButtonLabel
        {
            get { return (string)GetValue(CancelButtonLabelProperty); }
            set { SetValue(CancelButtonLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelButtonLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelButtonLabelProperty =
            DependencyProperty.Register("CancelButtonLabel", typeof(string), typeof(XlyMessageBoxQuestion), new UIPropertyMetadata("Cancel"));

        #endregion

        #region IsThreeButton -- 是否是三个按钮

        /// <summary>
        /// 是否是三个按钮
        /// </summary>
        public bool IsThreeButton
        {
            get { return (bool)GetValue(IsThreeButtonProperty); }
            set { SetValue(IsThreeButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsThreeButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsThreeButtonProperty =
            DependencyProperty.Register("IsThreeButton", typeof(bool), typeof(XlyMessageBoxQuestion), new UIPropertyMetadata(false));

        #endregion
    }

    /// <summary>
    /// 提示询问
    /// </summary>
    [TemplatePart(Name = "PART_Yes", Type = typeof(Button))]
    [TemplatePart(Name = "PART_No", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Cancel", Type = typeof(Button))]
    public class XlyMirrorMessageBoxQuestion : System.Windows.Controls.Control
    {
        static XlyMirrorMessageBoxQuestion()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyMirrorMessageBoxQuestion), new FrameworkPropertyMetadata(typeof(XlyMirrorMessageBoxQuestion)));
        }

        #region Message -- 消息

        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(XlyMirrorMessageBoxQuestion), new UIPropertyMetadata(null));

        #endregion

        /// <summary>
        /// 返回值
        /// </summary>
        public bool? ResultValue { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Yes = this.Template.FindName("PART_Yes", this) as Button;
            this.PART_Yes.Click -= new RoutedEventHandler(PART_Yes_Click);
            this.PART_Yes.Click += new RoutedEventHandler(PART_Yes_Click);

            this.PART_No = this.Template.FindName("PART_No", this) as Button;
            this.PART_No.Click -= new RoutedEventHandler(PART_No_Click);
            this.PART_No.Click += new RoutedEventHandler(PART_No_Click);

            this.PART_Cancel = this.Template.FindName("PART_Cancel", this) as Button;
            this.PART_Cancel.Click -= new RoutedEventHandler(PART_Cancel_Click);
            this.PART_Cancel.Click += new RoutedEventHandler(PART_Cancel_Click);

            this.PART_Yes.Focus();
        }

        private void PART_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.ResultValue = null;
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }

        private void PART_No_Click(object sender, RoutedEventArgs e)
        {
            this.ResultValue = false;
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }

        private void PART_Yes_Click(object sender, RoutedEventArgs e)
        {
            this.ResultValue = true;
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }

        #region PART

        private Button PART_Yes;
        private Button PART_No;
        private Button PART_Cancel;

        #endregion

        #region YesButtonLabel -- Yes按钮的标签

        /// <summary>
        /// Yes按钮的标签
        /// </summary>
        public string YesButtonLabel
        {
            get { return (string)GetValue(YesButtonLabelProperty); }
            set { SetValue(YesButtonLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YesButtonLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YesButtonLabelProperty =
            DependencyProperty.Register("YesButtonLabel", typeof(string), typeof(XlyMirrorMessageBoxQuestion), new UIPropertyMetadata("Yes"));

        #endregion

        #region NoButtonLabel -- No按钮的标签

        /// <summary>
        /// No按钮的标签
        /// </summary>
        public string NoButtonLabel
        {
            get { return (string)GetValue(NoButtonLabelProperty); }
            set { SetValue(NoButtonLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoButtonLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoButtonLabelProperty =
            DependencyProperty.Register("NoButtonLabel", typeof(string), typeof(XlyMirrorMessageBoxQuestion), new UIPropertyMetadata("No"));

        #endregion

        #region CancelButtonLabel -- 取消按钮的标签

        /// <summary>
        /// 取消按钮的标签
        /// </summary>
        public string CancelButtonLabel
        {
            get { return (string)GetValue(CancelButtonLabelProperty); }
            set { SetValue(CancelButtonLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelButtonLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelButtonLabelProperty =
            DependencyProperty.Register("CancelButtonLabel", typeof(string), typeof(XlyMirrorMessageBoxQuestion), new UIPropertyMetadata("Cancel"));

        #endregion

        #region IsThreeButton -- 是否是三个按钮

        /// <summary>
        /// 是否是三个按钮
        /// </summary>
        public bool IsThreeButton
        {
            get { return (bool)GetValue(IsThreeButtonProperty); }
            set { SetValue(IsThreeButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsThreeButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsThreeButtonProperty =
            DependencyProperty.Register("IsThreeButton", typeof(bool), typeof(XlyMirrorMessageBoxQuestion), new UIPropertyMetadata(false));

        #endregion
    }
}
