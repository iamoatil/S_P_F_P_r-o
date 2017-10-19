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
    /// 提示信息输入一段文本
    /// </summary>
    public class XlyMessageBoxInput : System.Windows.Controls.Control
    {
        static XlyMessageBoxInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyMessageBoxInput), new FrameworkPropertyMetadata(typeof(XlyMessageBoxInput)));
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
            DependencyProperty.Register("Message", typeof(string), typeof(XlyMessageBoxInput), new UIPropertyMetadata(null));

        #endregion

        #region InputText -- 输入的文本内容

        /// <summary>
        /// 输入的文本内容
        /// </summary>
        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(XlyMessageBoxInput), new PropertyMetadata(string.Empty, new PropertyChangedCallback(
                (o, args) =>
                {
                    // ad by liujiangwei 2015-10-26 最大值限制
                    //if (args.NewValue != null)
                    //{
                    //    string nvalue = (string)args.NewValue;

                    //    if (nvalue.Length > 15)
                    //    {

                    //        var mbi = o as XlyMessageBoxInput;

                    //        mbi.InputText = (string)args.OldValue;
                    //    }
                    //}

                })));

        #endregion

        #region MaxLengthProperty -- 最大长度

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(
            "MaxLength", typeof (int), typeof (XlyMessageBoxInput), new PropertyMetadata(default(int)));

        public int MaxLength
        {
            get { return (int) GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        #endregion


        /// <summary>
        /// 是否点击了确定
        /// </summary>
        public bool IsEnter { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Enter = this.Template.FindName("PART_Enter", this) as Button;
            this.PART_Enter.Click -= new RoutedEventHandler(PART_Enter_Click);
            this.PART_Enter.Click += new RoutedEventHandler(PART_Enter_Click);
            this.PART_Cancel = this.Template.FindName("PART_Cancel", this) as Button;
            this.PART_Cancel.Click -= new RoutedEventHandler(PART_Cancel_Click);
            this.PART_Cancel.Click += new RoutedEventHandler(PART_Cancel_Click);
        }

        private void PART_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            this.IsEnter = false;
            if (window != null)
            {
                window.Close();
            }
        }

        /// <summary>
        /// 当确定键点击时触发
        /// </summary>
        public event EventHandler<RoutedEventArgs> OnEnterButtonClick;

        private void PART_Enter_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnEnterButtonClick != null)
            {
                this.OnEnterButtonClick(sender, e);
                if (e.Handled == true)
                {
                    return;
                }
               
            }
            Window window = Window.GetWindow(this);
            this.IsEnter = true;
            if (window != null)
            {
                window.Close();
            }
        }

        #region PART

        private Button PART_Enter;
        private Button PART_Cancel;

        #endregion

    }
}
