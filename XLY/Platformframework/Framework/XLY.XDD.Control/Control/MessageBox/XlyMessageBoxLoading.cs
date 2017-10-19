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
    /// 等待信息
    /// </summary>
    public class XlyMessageBoxLoading : System.Windows.Controls.Control
    {
        static XlyMessageBoxLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyMessageBoxLoading), new FrameworkPropertyMetadata(typeof(XlyMessageBoxLoading)));
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
            DependencyProperty.Register("Message", typeof(string), typeof(XlyMessageBoxLoading), new UIPropertyMetadata(null));

        #endregion
    }
}
