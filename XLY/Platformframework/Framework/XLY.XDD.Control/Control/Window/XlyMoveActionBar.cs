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
    /// 移动激活条
    /// </summary>
    public class XlyMoveActionBar : System.Windows.Controls.ContentControl
    {
        static XlyMoveActionBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyMoveActionBar), new FrameworkPropertyMetadata(typeof(XlyMoveActionBar)));
        }

        public XlyMoveActionBar()
        {
            this.timer = new System.Timers.Timer(1000);
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            this.timer.Start();
            this.PreviewMouseMove += new MouseEventHandler(XlyMoveActionBar_PreviewMouseMove);
        }

        ~XlyMoveActionBar()
        {
            if (this.timer != null)
                this.timer.Stop();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.InvokeEx(() =>
            {
                ++this.time;
                if (this.time >= this.WaitForHiddenTime.TotalSeconds && !this.IsKeepShow)
                {
                    if (this.Content is FrameworkElement)
                    {
                        FrameworkElement element = this.Content as FrameworkElement;
                        Point point = Mouse.GetPosition(element);
                        HitTestResult result = VisualTreeHelper.HitTest(element, point);
                        if (result == null || result.VisualHit == null)
                        {
                            element.BeginHideWithOpacityAndVisibility(this.ShowOrHideDuration, null);
                        }
                        else
                        {
                            return;
                        }
                    }
                    this.timer.Stop();
                    this.isStop = true;
                }
            });
        }

        private void XlyMoveActionBar_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            this.time = 0;
            if (this.Content is FrameworkElement)
            {
                FrameworkElement element = this.Content as FrameworkElement;
                if (element.Visibility == System.Windows.Visibility.Collapsed)
                {
                    element.BeginShowWithOpacityAndVisibility(this.ShowOrHideDuration, null);
                }
            }
            if (this.isStop)
            {
                this.timer.Start();
                this.isStop = false;
            }
        }

        #region WaitForHiddenTime -- 等待隐藏的时间

        private double time;
        private bool isStop = false;
        private System.Timers.Timer timer;

        /// <summary>
        /// 等待隐藏的时间
        /// </summary>
        public TimeSpan WaitForHiddenTime
        {
            get { return (TimeSpan)GetValue(WaitForHiddenTimeProperty); }
            set { SetValue(WaitForHiddenTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaitForHiddenTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaitForHiddenTimeProperty =
            DependencyProperty.Register("WaitForHiddenTime", typeof(TimeSpan), typeof(XlyMoveActionBar), new UIPropertyMetadata(TimeSpan.FromSeconds(3)));

        #endregion

        #region ShowOrHideDuration -- 显示或隐藏过程需要的时间

        /// <summary>
        /// 显示或隐藏过程需要的时间
        /// </summary>
        public TimeSpan ShowOrHideDuration
        {
            get { return (TimeSpan)GetValue(ShowOrHideDurationProperty); }
            set { SetValue(ShowOrHideDurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowOrHideDuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowOrHideDurationProperty =
            DependencyProperty.Register("ShowOrHideDuration", typeof(TimeSpan), typeof(XlyMoveActionBar), new UIPropertyMetadata(TimeSpan.FromSeconds(0.5)));

        #endregion

        #region IsKeepShow -- 是否保持显示

        /// <summary>
        /// 是否保持显示
        /// </summary>
        public bool IsKeepShow
        {
            get { return (bool)GetValue(IsKeepShowProperty); }
            set { SetValue(IsKeepShowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsKeepShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsKeepShowProperty =
            DependencyProperty.Register("IsKeepShow", typeof(bool), typeof(XlyMoveActionBar), new UIPropertyMetadata(false, new PropertyChangedCallback((s, e) =>
            {
                XlyMoveActionBar bar = s as XlyMoveActionBar;
                bool iskeepshow = (bool)e.NewValue;
                if (iskeepshow && bar.Content is FrameworkElement)
                {
                    FrameworkElement element = bar.Content as FrameworkElement;
                    if (bar.isStop)
                    {
                        element.BeginShowWithOpacityAndVisibility(bar.ShowOrHideDuration, null);
                    }
                }
                if (!iskeepshow)
                {
                    bar.timer.Start();
                    bar.isStop = false;
                }
            })));

        #endregion
    }
}
