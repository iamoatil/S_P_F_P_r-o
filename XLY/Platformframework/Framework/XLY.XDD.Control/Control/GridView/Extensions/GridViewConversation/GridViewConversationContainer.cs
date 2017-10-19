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
    /// 会话模式容器
    /// </summary>
    public class GridViewConversationContainer : System.Windows.Controls.Control
    {
        static GridViewConversationContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridViewConversationContainer), new FrameworkPropertyMetadata(typeof(GridViewConversationContainer)));
        }

        /// <summary>
        /// 超链接打开功能实现。
        /// </summary>
        public ICommand HyperClick
        {
            get
            {
                return new XLYRelayCommand(s =>
                {
                    if (null != s)
                    {
                        var path = (s as System.Uri).AbsoluteUri;
                        if (!string.IsNullOrEmpty(path))
                            System.Diagnostics.Process.Start(path);
                    }
                });
            }
        }

        #region 依赖属性

        #region HyperUrl  -- 超链接地址。


        /// <summary>
        /// 超链接地址
        /// </summary>
        public string HyperUrl
        {
            get { return (string)GetValue(HyperUrlProperty); }
            set { SetValue(HyperUrlProperty, value); }
        }

        public static readonly DependencyProperty HyperUrlProperty =
            DependencyProperty.Register("HyperUrl", typeof(string), typeof(GridViewConversationContainer), new PropertyMetadata(null));

        #endregion

        #region Role -- 角色

        /// <summary>
        /// 角色
        /// </summary>
        public GridViewConversationContainerRole Role
        {
            get { return (GridViewConversationContainerRole)GetValue(RoleProperty); }
            set { SetValue(RoleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Role.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.Register("Role", typeof(GridViewConversationContainerRole), typeof(GridViewConversationContainer), new UIPropertyMetadata(GridViewConversationContainerRole.Other));

        #endregion

        #region Avatar -- 头像

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar
        {
            get { return (string)GetValue(AvatarProperty); }
            set { SetValue(AvatarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Avatar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvatarProperty =
            DependencyProperty.Register("Avatar", typeof(string), typeof(GridViewConversationContainer), new UIPropertyMetadata(null));

        #endregion

        #region Sender -- 发送者

        /// <summary>
        /// 发送者
        /// </summary>
        public string Sender
        {
            get { return (string)GetValue(SenderProperty); }
            set { SetValue(SenderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Sender.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SenderProperty =
            DependencyProperty.Register("Sender", typeof(string), typeof(GridViewConversationContainer), new UIPropertyMetadata(null));

        #endregion

        #region Time -- 发送时间

        /// <summary>
        /// 发送时间
        /// </summary>
        public string Time
        {
            get { return (string)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Time.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(string), typeof(GridViewConversationContainer), new UIPropertyMetadata(null));

        #endregion

        #region MessageType -- 消息类型

        /// <summary>
        /// 消息类型
        /// </summary>
        public GridViewConversationContainerMessageType MessageType
        {
            get { return (GridViewConversationContainerMessageType)GetValue(MessageTypeProperty); }
            set { SetValue(MessageTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageTypeProperty =
            DependencyProperty.Register("MessageType", typeof(GridViewConversationContainerMessageType), typeof(GridViewConversationContainer), new UIPropertyMetadata(GridViewConversationContainerMessageType.String));

        #endregion

        #region Message -- 消息内容

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(GridViewConversationContainer), new UIPropertyMetadata(null));

        #endregion

        #region ContentMaxWidth -- 内容最大宽度

        /// <summary>
        /// 内容最大宽度
        /// </summary>
        public double ContentMaxWidth
        {
            get { return (double)GetValue(ContentMaxWidthProperty); }
            set { SetValue(ContentMaxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentMaxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentMaxWidthProperty =
            DependencyProperty.Register("ContentMaxWidth", typeof(double), typeof(GridViewConversationContainer), new UIPropertyMetadata(200d));

        #endregion 

        #endregion
    }
}
