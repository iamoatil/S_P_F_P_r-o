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
    /// 播放组建
    /// </summary>
    [TemplatePart(Name = "PART_Play", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Pause", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Stop", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Timeline", Type = typeof(Slider))]
    [TemplatePart(Name = "PART_Voiceline", Type = typeof(Slider))]
    [TemplatePart(Name = "PART_Media", Type = typeof(MediaElement))]
    public class MediaViewer : System.Windows.Controls.Control, IFileViewer
    {
        static MediaViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaViewer), new FrameworkPropertyMetadata(typeof(MediaViewer)));
        }

        #region PART

        private MediaElement PART_Media;
        private Button PART_Play;
        private Button PART_Pause;
        private Button PART_Stop;
        private Slider PART_Timeline;
        private Slider PART_Voiceline;

        private static double S_Voice = 50;

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Play = this.Template.FindName("PART_Play", this) as Button;
            this.PART_Pause = this.Template.FindName("PART_Pause", this) as Button;
            this.PART_Stop = this.Template.FindName("PART_Stop", this) as Button;
            this.PART_Timeline = this.Template.FindName("PART_Timeline", this) as Slider;
            this.PART_Voiceline = this.Template.FindName("PART_Voiceline", this) as Slider;
            this.PART_Media = this.Template.FindName("PART_Media", this) as MediaElement;

            this.PART_Play.Click -= new RoutedEventHandler(PART_Play_Click);
            this.PART_Play.Click += new RoutedEventHandler(PART_Play_Click);
            this.PART_Pause.Click -= new RoutedEventHandler(PART_Pause_Click);
            this.PART_Pause.Click += new RoutedEventHandler(PART_Pause_Click);
            this.PART_Stop.Click -= new RoutedEventHandler(PART_Stop_Click);
            this.PART_Stop.Click += new RoutedEventHandler(PART_Stop_Click);
            this.PART_Timeline.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(PART_Timeline_ValueChanged);
            this.PART_Timeline.ValueChanged += new RoutedPropertyChangedEventHandler<double>(PART_Timeline_ValueChanged);
            this.PART_Voiceline.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(PART_Voiceline_ValueChanged);
            this.PART_Voiceline.ValueChanged += new RoutedPropertyChangedEventHandler<double>(PART_Voiceline_ValueChanged);
            this.PART_Media.PositionChanged -= new RoutedEventHandler(PART_Media_PositionChanged);
            this.PART_Media.PositionChanged += new RoutedEventHandler(PART_Media_PositionChanged);
            this.PART_Media.EndReached -= new RoutedEventHandler(PART_Media_EndReached);
            this.PART_Media.EndReached += new RoutedEventHandler(PART_Media_EndReached);

            this.PART_Voiceline.Maximum = 100;
            this.PART_Voiceline.Value = S_Voice;
            this.PART_Play.IsEnabled = false;
            this.PART_Pause.IsEnabled = false;
            this.PART_Stop.IsEnabled = false;
            this.PART_Timeline.IsEnabled = false;
            this.PART_Voiceline.IsEnabled = false;
        }

        #region Public Interface

        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            this.PART_Media.Play();
            this.PART_Timeline.Minimum = 0;
            this.PART_Play.IsEnabled = false;
            this.PART_Pause.IsEnabled = true;
            this.PART_Stop.IsEnabled = true;
            this.PART_Timeline.IsEnabled = true;
            this.PART_Voiceline.IsEnabled = true;
            this.IsOpen = true;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            this.PART_Media.Pause();
            this.PART_Play.IsEnabled = true;
            this.PART_Pause.IsEnabled = false;
            this.PART_Stop.IsEnabled = true;
            this.PART_Timeline.IsEnabled = true;
            this.PART_Voiceline.IsEnabled = true;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (!this.IsOpen)
            {
                return;
            }

            this.PART_Media.Stop();
            this.PART_Play.IsEnabled = true;
            this.PART_Pause.IsEnabled = false;
            this.PART_Stop.IsEnabled = false;
            this.PART_Timeline.Value = 0;
            this.PART_Timeline.IsEnabled = false;
            this.PART_Voiceline.IsEnabled = true;
            this.IsOpen = false;
        }

        #endregion

        /// <summary>
        /// 播放完成
        /// </summary>
        private void PART_Media_EndReached(object sender, RoutedEventArgs e)
        {
            this.PART_Play.IsEnabled = true;
            this.PART_Pause.IsEnabled = false;
            this.PART_Stop.IsEnabled = false;
            this.PART_Timeline.IsEnabled = false;
            this.PART_Timeline.Value = 0;
        }

        /// <summary>
        /// 播放器位置改变
        /// </summary>
        private void PART_Media_PositionChanged(object sender, RoutedEventArgs e)
        {
            if (this.PART_Media.Length.HasValue)
                this.PART_Timeline.Maximum = this.PART_Media.Length.Value.TotalMilliseconds;
            if (this.PART_Media.Position.HasValue)
                this.PART_Timeline.Value = this.PART_Media.Position.Value.TotalMilliseconds;
            else
                this.PART_Timeline.Value = 0;
        }

        /// <summary>
        /// 播放
        /// </summary>
        private void PART_Play_Click(object sender, RoutedEventArgs e)
        {
            this.Play();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        private void PART_Pause_Click(object sender, RoutedEventArgs e)
        {
            this.Pause();
        }

        /// <summary>
        /// 停止
        /// </summary>
        private void PART_Stop_Click(object sender, RoutedEventArgs e)
        {
            this.Stop();
        }

        /// <summary>
        /// 时间线值改变
        /// </summary>
        private void PART_Timeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.PART_Media.Position = TimeSpan.FromMilliseconds(e.NewValue);
        }

        /// <summary>
        /// 声音值改变
        /// </summary>
        private void PART_Voiceline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.PART_Media.Volume = (e.NewValue / 100) * 2;
            S_Voice = e.NewValue;
        }

        #region IsOpen -- 文件是否已经被打开

        /// <summary>
        /// 文件是否已经被打开
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(MediaViewer), new UIPropertyMetadata(false));

        #endregion

        #region IFileViewer

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Open(string path)
        {
            this.Close();

            this.PART_Media.Open(path);
            this.PART_Play.IsEnabled = true;
            this.PART_Voiceline.IsEnabled = true;
            this.OpenArgs = path;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(System.IO.Stream stream, string extension)
        {
            this.OpenArgs = stream;

        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="buffer">文件Buffer</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(byte[] buffer, string extension)
        {
            this.OpenArgs = buffer;

        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="args">打开文件参数</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(IFileViewerArgs args)
        {
            if (args.Type == FileViewerArgsType.Path)
                this.Open(args.Path);
            else if (args.Type == FileViewerArgsType.Stream)
                this.Open(args.Stream, args.Extension);
            else if (args.Type == FileViewerArgsType.Buffer)
                this.Open(args.Buffer, args.Extension);
            this.OpenArgs = args;
        }

        /// <summary>
        /// 打开参数
        /// </summary>
        public object OpenArgs { get; set; }

        /// <summary>
        /// 关闭文件
        /// </summary>
        public void Close()
        {
            if (this.PART_Timeline == null)
            {
                this.ApplyTemplate();
            }
            if (this.PART_Timeline == null)
            {
                return;
            }
            this.PART_Timeline.Value = 0;
            this.PART_Timeline.Maximum = 0;
            this.PART_Play.IsEnabled = false;
            this.PART_Pause.IsEnabled = false;
            this.PART_Stop.IsEnabled = false;
            this.PART_Timeline.IsEnabled = false;
            this.PART_Voiceline.IsEnabled = false;

            if (!this.IsOpen)
            {
                return;
            }

            this.PART_Media.Close();
            this.IsOpen = false;
        }

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionString()
        {
            return string.Empty;
        }

        /// <summary>
        /// 拷贝选中的内容
        /// </summary>
        public void Copy()
        {

        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {

        }

        /// <summary>
        /// 是否支持该文件后缀
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns>是否支持该文件后缀</returns>
        public bool IsSupport(string extension)
        {
            return FileViewerConfig.Config.IsSupport(FileViewerType.Media, extension);
        }

        /// <summary>
        /// 视图类型
        /// </summary>
        public FileViewerType ViewerType
        {
            get { return FileViewerType.Media; }
        }

        #endregion
    }
}
