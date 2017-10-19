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

namespace XLY.SF.Shell.CommWindow
{
    /// <summary>
    /// MessageBoxWin.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBoxWin : Window
    {
        #region 关闭动画控制器

        /// <summary>
        /// 是否播放关闭动画
        /// </summary>
        private bool _playClose;

        /// <summary>
        /// 是否允许关闭
        /// </summary>
        private bool _allowClose;

        /// <summary>
        /// 关闭动画
        /// </summary>
        private Storyboard _closeWindow;
        
        /// <summary>
        /// 窗体关闭结果
        /// </summary>
        public bool DialogResultEx { get; private set; }

        #endregion

        public MessageBoxWin()
        {
            InitializeComponent();
        }

        #region 关闭动画

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Closing += MessageBoxWin_Closing;
        }

        void MessageBoxWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_playClose)
            {
                _playClose = true;
                CloseStoryboard();
            }
            e.Cancel = !_allowClose;
        }

        void _closeWindow_Completed(object sender, EventArgs e)
        {
            _allowClose = true;
            this.Close();
        }

        #endregion

        #region 内容与标题定义

        public string MsgTitle { get; private set; }

        public string MsgContent { get; private set; }

        /// <summary>
        /// 设置消息框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        public void SetMsgBox(string title,string content)
        {
            this.MsgTitle = title;
            this.MsgContent = content;
            this.DataContext = this;
        }

        #endregion

        #region 界面操作

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResultEx = true;
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        #endregion

        #region 关闭动画

        private void CloseStoryboard()
        {
            _closeWindow = new Storyboard();
            _closeWindow.Completed += _closeWindow_Completed;
            #region 创建关闭动画

            DoubleAnimationUsingKeyFrames keyFrame1 = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame ea = new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0)));
            EasingDoubleKeyFrame ea1 = new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5)));
            ElasticEase be = new ElasticEase();
            be.EasingMode = EasingMode.EaseIn;
            be.Springiness = 5;
            be.Oscillations = 1;
            ea1.EasingFunction = be;
            keyFrame1.KeyFrames.Add(ea);
            keyFrame1.KeyFrames.Add(ea1);

            Storyboard.SetTarget(keyFrame1, grid);
            Storyboard.SetTargetProperty(keyFrame1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            _closeWindow.Children.Add(keyFrame1);

            
            DoubleAnimationUsingKeyFrames keyFrame2 = new DoubleAnimationUsingKeyFrames();
            ea = new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0)));
            ea1 = new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5)));
            be = new ElasticEase();
            be.EasingMode = EasingMode.EaseIn;
            be.Springiness = 5;
            be.Oscillations = 1;
            ea1.EasingFunction = be;
            keyFrame2.KeyFrames.Add(ea);
            keyFrame2.KeyFrames.Add(ea1);

            Storyboard.SetTarget(keyFrame2, grid);
            Storyboard.SetTargetProperty(keyFrame2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            _closeWindow.Children.Add(keyFrame2);

            #endregion
            _closeWindow.Begin(grid);
        }

        #endregion
    }
}
