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
    /// 文件视图合并容器
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    public class FileViewerMultiContainer : System.Windows.Controls.Control, IFileViewer
    {
        static FileViewerMultiContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileViewerMultiContainer), new FrameworkPropertyMetadata(typeof(FileViewerMultiContainer)));
        }

        #region PART

        private Grid PART_Root;

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Root = this.Template.FindName("PART_Root", this) as Grid;
            this._UpdateFileViewers();
        }

        #region FileViewers -- 文件视图集合

        /// <summary>
        /// 文件视图集合
        /// </summary>
        public FileViewerMultiCollection FileViewers
        {
            get { return (FileViewerMultiCollection)GetValue(FileViewersProperty); }
            set { SetValue(FileViewersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileViewers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileViewersProperty =
            DependencyProperty.Register("FileViewers", typeof(FileViewerMultiCollection), typeof(FileViewerMultiContainer), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                FileViewerMultiContainer container = s as FileViewerMultiContainer;
                if (container.PART_Root == null)
                    container.ApplyTemplate();
                if (container.PART_Root == null)
                    return;
                container._UpdateFileViewers();
            })));

        private void _UpdateFileViewers()
        {
            this.PART_Root.Children.Clear();
            if (this.FileViewers == null || this.FileViewers.Count == 0)
                return;
            foreach (UIElement ui in this.FileViewers)
            {
                if (ui != null)
                {
                    this.PART_Root.Children.Add(ui);
                }
            }
        }

        #endregion

        #region CurrentFileViewer -- 当前的文件视图

        /// <summary>
        /// 当前的文件视图
        /// </summary>
        public IFileViewer CurrentFileViewer
        {
            get { return (IFileViewer)GetValue(CurrentFileViewerProperty); }
            private set { SetValue(CurrentFileViewerPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey CurrentFileViewerPropertyKey =
            DependencyProperty.RegisterReadOnly("CurrentFileViewer", typeof(IFileViewer), typeof(FileViewerMultiContainer), new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for CurrentFileViewer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentFileViewerProperty = CurrentFileViewerPropertyKey.DependencyProperty;

        #endregion

        #region IFileViewer

        /// <summary>
        /// 根据后缀选择最佳视图，并且切换至最佳视图
        /// </summary>
        /// <param name="extension">要切换的后缀</param>
        /// <param name="argsType">打开方式</param>
        private IFileViewer _Open(string extension, FileViewerArgsType argsType)
        {
            if (this.FileViewers == null || this.FileViewers.Count == 0)
                return null;
            FileViewerType type = FileViewerConfig.Config.BestSupport(extension, argsType, this.FileViewers.Select(p => p.ViewerType).ToArray());
            IFileViewer viewer = this.FileViewers.Where(p => p.ViewerType == type).FirstOrDefault();
            foreach (UIElement ui in this.FileViewers)
            {
                if (ui == null)
                    continue;
                if (ui == viewer)
                {
                    ui.Visibility = System.Windows.Visibility.Visible;
                    IFileViewer ifv = ui as IFileViewer;
                }
                else
                {
                    ui.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            this.CurrentFileViewer = viewer;
            return viewer;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Open(string path)
        {
            string extension = System.IO.Path.GetExtension(path);
            IFileViewer viewer = this._Open(extension, FileViewerArgsType.Path);
            if (viewer != null)
            {
                viewer.Open(path);
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
            IFileViewer viewer = this._Open(extension, FileViewerArgsType.Stream);
            if (viewer != null)
            {
                viewer.Open(stream, extension);
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
            IFileViewer viewer = this._Open(extension, FileViewerArgsType.Buffer);
            if (viewer != null)
            {
                viewer.Open(buffer, extension);
            }
            this.OpenArgs = buffer;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="args">打开文件参数</param>
        public void Open(IFileViewerArgs args)
        {
            string extension = args.Extension;
            IFileViewer viewer = null;
            if (args.Buffer == null && string.IsNullOrEmpty(args.Path) && args.Stream == null)
                viewer = this.FileViewers.Where(c => c.ViewerType == FileViewerType.None).FirstOrDefault();
            else
                viewer = this._Open(extension, args.Type);
            if (viewer != null)
            {
                viewer.Open(args);
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
            try
            {
                this.FileViewers.ForEach(c =>
                {
                    try
                    {
                        c.Close();
                    }
                    catch
                    { }
                });
            }
            catch
            {
                this._Open(string.Empty, FileViewerArgsType.Path);
            }
        }

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionString()
        {
            if (this.CurrentFileViewer != null)
                return this.CurrentFileViewer.GetSelectionString();
            else
                return string.Empty;
        }

        /// <summary>
        /// 拷贝选中的内容
        /// </summary>
        public void Copy()
        {
            if (this.CurrentFileViewer != null)
                this.CurrentFileViewer.Copy();
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            if (this.CurrentFileViewer != null)
                this.CurrentFileViewer.SelectAll();
        }

        /// <summary>
        /// 是否支持该文件后缀
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns>是否支持该文件后缀</returns>
        public bool IsSupport(string extension)
        {
            return true;
        }

        /// <summary>
        /// 视图类型
        /// </summary>
        public FileViewerType ViewerType
        {
            get { return FileViewerType.MultiContainer; }
        }

        #endregion

        /// <summary>
        /// 强制切换至某视图
        /// </summary>
        /// <param name="type">要切换至的视图类型</param>
        /// <returns>是否成功切换</returns>
        public bool ChangeToView(FileViewerType type)
        {
            if (this.FileViewers == null || this.FileViewers.Count == 0)
                return false;
            IFileViewer ifv = this.FileViewers.Where(p => p.ViewerType == type).FirstOrDefault();
            if (ifv != null && ifv is UIElement)
            {
                UIElement ui = ifv as UIElement;
                foreach (UIElement element in this.FileViewers)
                {
                    if (element != null && element != ui)
                    {
                        element.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                ui.Visibility = System.Windows.Visibility.Visible;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
