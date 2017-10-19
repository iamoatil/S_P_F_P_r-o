using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 激活按钮，当点击时根据解析器处理行为
    /// </summary>
    public class XlyActionButton : Button
    {
        static XlyActionButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyActionButton), new FrameworkPropertyMetadata(typeof(XlyActionButton)));
        }

        #region ActionProvider -- 激活解析器

        /// <summary>
        /// 激活解析器
        /// </summary>
        public IXlyActionButtonProvider ActionProvider
        {
            get { return (IXlyActionButtonProvider)GetValue(ActionProviderProperty); }
            set { SetValue(ActionProviderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActionProvider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionProviderProperty =
            DependencyProperty.Register("ActionProvider", typeof(IXlyActionButtonProvider), typeof(XlyActionButton), new UIPropertyMetadata(null));

        #endregion

        #region Type -- 激活类型

        /// <summary>
        /// 激活类型
        /// </summary>
        public object Type
        {
            get { return (object)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(object), typeof(XlyActionButton), new UIPropertyMetadata(null));

        #endregion

        #region Args -- 参数

        /// <summary>
        /// 参数
        /// </summary>
        public object Args
        {
            get { return (object)GetValue(ArgsProperty); }
            set { SetValue(ArgsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Args.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArgsProperty =
            DependencyProperty.Register("Args", typeof(object), typeof(XlyActionButton), new UIPropertyMetadata(null));

        #endregion

        public XlyActionButton()
        {
            this.Click -= new RoutedEventHandler(XlyActionButton_Click);
            this.Click += new RoutedEventHandler(XlyActionButton_Click);
        }

        private void XlyActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ActionProvider != null)
            {
                this.ActionProvider.Action(this.Type, this.Args);
            }
        }
    }
}
