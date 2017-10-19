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
    /// 文件预览容器
    /// </summary>
    public class FileViewerContainer : System.Windows.Controls.Control, IFileViewer
    {
        static FileViewerContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileViewerContainer), new FrameworkPropertyMetadata(typeof(FileViewerContainer)));
        }

        public FileViewerContainer()
        {
            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(FileViewerContainer_IsVisibleChanged);
        }

        ~FileViewerContainer()
        {
            if (this.timer != null)
                this.timer.Dispose();
        }

        /// <summary>
        /// 执行打开文件
        /// </summary>
        private void DoOpen()
        {
            if (this.lastArgs == null || this.IsVisible == false || this.FileViewer == null)
                return;
            lock (this.lastArgs)
            {
                IFileViewerArgs temp = this.lastArgs;
                this.lastArgs = null;
                try
                {
                    this.FileViewer.Open(temp);
                    this.timer.Stop();
                    this.IsDelaying = false;
                }
                catch
                {
                    this.timer.Stop();
                    this.IsDelaying = false;
                }
            }
        }

        private void FileViewerContainer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.DoOpen();
        }

        #region FileViewer -- 文件预览控件

        /// <summary>
        /// 文件预览控件
        /// </summary>
        public IFileViewer FileViewer
        {
            get { return (IFileViewer)GetValue(FileViewerProperty); }
            set { SetValue(FileViewerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileViewer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileViewerProperty =
            DependencyProperty.Register("FileViewer", typeof(IFileViewer), typeof(FileViewerContainer), new UIPropertyMetadata(null));

        #endregion

        #region IsDelayOpen -- 是否迟延打开

        /// <summary>
        /// 是否迟延打开
        /// </summary>
        public bool IsDelayOpen
        {
            get { return (bool)GetValue(IsDelayOpenProperty); }
            set { SetValue(IsDelayOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDelayOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDelayOpenProperty =
            DependencyProperty.Register("IsDelayOpen", typeof(bool), typeof(FileViewerContainer), new UIPropertyMetadata(false, new PropertyChangedCallback((s, e) =>
            {
                FileViewerContainer container = s as FileViewerContainer;
                if ((bool)e.NewValue == false && container.timer != null)
                {
                    container.timer.Stop();
                }
            })));

        #endregion

        #region DelayTime -- 迟延时间

        /// <summary>
        /// 迟延时间
        /// </summary>
        public TimeSpan DelayTime
        {
            get { return (TimeSpan)GetValue(DelayTimeProperty); }
            set { SetValue(DelayTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DelayTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelayTimeProperty =
            DependencyProperty.Register("DelayTime", typeof(TimeSpan), typeof(FileViewerContainer), new UIPropertyMetadata(TimeSpan.FromSeconds(0.5), new PropertyChangedCallback((s, e) =>
            {
                FileViewerContainer container = s as FileViewerContainer;
                container.ResetTimer(true);
            })));

        #endregion

        #region IsDelaying -- 是否正处于迟延状态

        /// <summary>
        /// 是否正处于迟延状态
        /// </summary>
        public bool IsDelaying
        {
            get { return (bool)GetValue(IsDelayingProperty); }
            set { SetValue(IsDelayingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDelaying.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDelayingProperty =
            DependencyProperty.Register("IsDelaying", typeof(bool), typeof(FileViewerContainer), new UIPropertyMetadata(false));

        #endregion

        // 计时器
        private System.Timers.Timer timer;

        // 最后一次需要处理的信息
        private IFileViewerArgs lastArgs;

        /// <summary>
        /// 重置计时器
        /// </summary>
        /// <param name="isResetInterval">是否重置计时器间隔</param>
        private void ResetTimer(bool isResetInterval)
        {
            if (this.timer == null)
            {
                this.timer = new System.Timers.Timer();
                this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            }
            if (isResetInterval)
            {
                this.timer.Interval = this.DelayTime.TotalSeconds * 1000;
            }
            this.timer.Stop();
            if (this.IsDelayOpen && this.lastArgs != null)
            {
                this.timer.Start();
            }
        }

        // 计时器到达时触发
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.IsVisible && this.lastArgs != null)
            {
                this.BeginInvokeEx(() =>
                {
                    this.DoOpen();
                });
            }
        }

        #region IFileViewer

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Open(string path)
        {
            if (this.FileViewer == null)
                return;
            if (!this.IsDelayOpen)
            {
                this.FileViewer.Open(path);
            }
            else
            {
                FileViewerArgs args = new FileViewerArgs();
                args.Path = path;
                args.Type = FileViewerArgsType.Path;
                this.lastArgs = args;
                this.IsDelaying = true;
                this.ResetTimer(false);
            }
            this.OpenArgs = path;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(System.IO.Stream stream, string extension)
        {
            if (this.FileViewer == null)
                return;
            if (!this.IsDelayOpen)
            {
                this.FileViewer.Open(stream, extension);
            }
            else
            {
                FileViewerArgs args = new FileViewerArgs();
                args.Stream = stream;
                args.Extension = extension;
                args.Type = FileViewerArgsType.Stream;
                this.lastArgs = args;
                this.IsDelaying = true;
                this.ResetTimer(false);
            }
            this.OpenArgs = stream;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="buffer">文件Buffer</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(byte[] buffer, string extension)
        {
            if (this.FileViewer == null)
                return;
            if (!this.IsDelayOpen)
            {
                this.FileViewer.Open(buffer, extension);
            }
            else
            {
                FileViewerArgs args = new FileViewerArgs();
                args.Buffer = buffer;
                args.Extension = extension;
                args.Type = FileViewerArgsType.Buffer;
                this.lastArgs = args;
                this.IsDelaying = true;
                this.ResetTimer(false);
            }
            this.OpenArgs = buffer;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="args">打开文件参数</param>
        public void Open(IFileViewerArgs args)
        {
            if (this.FileViewer == null)
                return;
            if (!this.IsDelayOpen)
            {
                this.FileViewer.Open(args);
            }
            else
            {
                this.lastArgs = args;
                this.IsDelaying = true;
                this.ResetTimer(false);
            }
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
            if (this.FileViewer == null)
                return;
            this.FileViewer.Close();
            this.IsDelaying = false;
        }

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionString()
        {
            if (this.FileViewer == null)
                return string.Empty;
            return this.FileViewer.GetSelectionString();
        }

        /// <summary>
        /// 拷贝选中的内容
        /// </summary>
        public void Copy()
        {
            if (this.FileViewer == null)
                return;
           
            this.FileViewer.Copy();
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            if (this.FileViewer == null)
                return;

            this.FileViewer.SelectAll();
        }

        /// <summary>
        /// 是否支持该文件后缀
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns>是否支持该文件后缀</returns>
        public bool IsSupport(string extension)
        {
            if (this.FileViewer == null)
                return false;
            return this.FileViewer.IsSupport(extension);
        }

        /// <summary>
        /// 视图类型
        /// </summary>
        public FileViewerType ViewerType
        {
            get { return FileViewerType.Container; }
        }

        #endregion

    }
}
