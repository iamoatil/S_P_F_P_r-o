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
using System.Windows.Media.Animation;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 等待控件
    /// </summary>
    public class XlyLoading : System.Windows.Controls.ContentControl
    {
        static XlyLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyLoading), new FrameworkPropertyMetadata(typeof(XlyLoading)));
        }

        #region Showed -- 显示之后触发

        /// <summary>
        /// 显示之后触发
        /// </summary>
        public event RoutedEventHandler Showed
        {
            add { this.AddHandler(ShowedEvent, value); }
            remove { this.RemoveHandler(ShowedEvent, value); }
        }

        public static readonly RoutedEvent ShowedEvent =
            EventManager.RegisterRoutedEvent("Showed", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(XlyLoading));

        #endregion

        #region Hideed -- 隐藏之后

        /// <summary>
        /// 显示之后触发
        /// </summary>
        public event RoutedEventHandler Hideed
        {
            add { this.AddHandler(HideedEvent, value); }
            remove { this.RemoveHandler(HideedEvent, value); }
        }

        public static readonly RoutedEvent HideedEvent =
            EventManager.RegisterRoutedEvent("Hideed", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(XlyLoading));

        #endregion

        // 是否已经完成控件加载
        private bool _IsAlreadyApplyTemplate;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.IsShow)
            {
                this.RaiseEvent(new RoutedEventArgs(ShowedEvent));
            }
            else
            {
                this.RaiseEvent(new RoutedEventArgs(HideedEvent));
            }
            this._IsAlreadyApplyTemplate = true;
        }

        #region IsShow -- 是否显示

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow
        {
            get { return (bool)GetValue(IsShowProperty); }
            set { SetValue(IsShowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.Register("IsShow", typeof(bool), typeof(XlyLoading), new UIPropertyMetadata(false, new PropertyChangedCallback((s, e) =>
            {
                XlyLoading loading = s as XlyLoading;
                if (!loading._IsAlreadyApplyTemplate)
                    loading.ApplyTemplate();
                if ((bool)e.NewValue)
                {
                    loading.RaiseEvent(new RoutedEventArgs(ShowedEvent));
                }
                else
                {
                    loading.RaiseEvent(new RoutedEventArgs(HideedEvent));
                }
            })));

        #endregion
    }
}
