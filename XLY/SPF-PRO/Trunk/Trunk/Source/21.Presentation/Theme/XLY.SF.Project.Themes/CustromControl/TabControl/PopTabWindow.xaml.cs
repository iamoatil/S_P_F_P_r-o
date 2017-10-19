using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XLY.SF.Project.Themes.CustromControl.TabControl
{
    /// <summary>
    /// PopTabWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PopTabWindow : Window
    {
        /// <summary>
        /// 是否允许拖动
        /// </summary>
        private bool _allowMove;

        /// <summary>
        /// TabControl的Header
        /// </summary>
        public object TabItemHeader { get; set; }

        /// <summary>
        /// 窗体拖动事件
        /// </summary>
        public event Action<object, object, Point> Evt_MoveWindow;

        /// <summary>
        /// 加载动画
        /// </summary>
        private Storyboard _loadStoryboard;
        /// <summary>
        /// 是否播放动画
        /// </summary>
        private bool _isPlayStoryboard;

        /// <summary>
        /// 是否在本界面移动
        /// </summary>
        private bool _isCurWindowMove;

        private Window _parentWindow;

        public PopTabWindow(Window parentWindow, bool isPlayStoryboard = false)
        {
            InitializeComponent();
            this._parentWindow = parentWindow;
            this._isPlayStoryboard = isPlayStoryboard;
        }

        void Window2_Loaded(object sender, RoutedEventArgs e)
        {
            _loadStoryboard = this.Template.Resources["OnLoaded1"] as Storyboard;
            if (_isPlayStoryboard)
                _loadStoryboard.Begin(this, this.Template, false);
        }

        #region 窗体的拖动

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var a = sender as Border;
            _allowMove = true;
            a.CaptureMouse();
        }

        private void Border_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var a = sender as Border;
            _allowMove = false;
            _isCurWindowMove = false;
            this.Topmost = false;
            a.ReleaseMouseCapture();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            var a = sender as Border;
            var b = e.GetPosition(a);
            var c = e.GetPosition(this);
            var d = PointToScreen(c);
            if (_allowMove && e.LeftButton == MouseButtonState.Pressed)
            {
                if (_isCurWindowMove)
                {
                    this.Top = d.Y - 15;
                    this.Left = d.X - 60;
                }
                else
                {
                    if (b.X < 0 || b.Y < 0 || b.X > a.ActualWidth || b.Y > a.ActualHeight)
                    {
                        if (_parentWindow.WindowState == System.Windows.WindowState.Minimized)
                        {
                            _isCurWindowMove = true;
                            this.Topmost = true;
                            a.CaptureMouse();
                        }
                        else
                        {
                            if (Evt_MoveWindow != null)
                            {
                                this.Close();
                                Evt_MoveWindow(this.Content, TabItemHeader, d);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                this.WindowState = System.Windows.WindowState.Normal;
            }
            else
            {
                this.SizeToContent = System.Windows.SizeToContent.Manual;
                this.WindowState = System.Windows.WindowState.Maximized;
            }
        }
    }
}
