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
    /// Word预览控件
    /// </summary>
    public partial class WordViewer : UserControl, IFileViewer
    {
        public WordViewer()
        {
            InitializeComponent();
        }

        #region ContextMenuActions -- 右键菜单行为

        /// <summary>
        /// 右键菜单行为
        /// </summary>
        public MenuActionInfoCollection ContextMenuActions
        {
            get { return (MenuActionInfoCollection)GetValue(ContextMenuActionsProperty); }
            set { SetValue(ContextMenuActionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContextMenuActions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextMenuActionsProperty =
            DependencyProperty.Register("ContextMenuActions", typeof(MenuActionInfoCollection), typeof(WordViewer), new UIPropertyMetadata(null));

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.ContextMenuActions != null)
            {
                ContextMenu cm = new ContextMenu();
                for (int i = 0; i < this.ContextMenuActions.Count; ++i)
                {
                    MenuActionInfo info = this.ContextMenuActions[i];

                    MenuItem item = new MenuItem();
                    item.Header = info.Header;
                    if (!info.Icon.IsNullOrEmptyOrWhiteSpace())
                        item.Icon = StringImageConverter.Convert(info.Icon);
                    item.Click += (s, a) =>
                    {
                        info.DoOnClick(this, a);
                    };
                    cm.Items.Add(item);
                }
                this.ContextMenu = cm;
            }
        }

        #region IFileViewer

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Open(string path)
        {
            this.richEditControl.LoadDocument(path);
            this.OpenArgs = path;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(System.IO.Stream stream, string extension)
        {
            DevExpress.XtraRichEdit.DocumentFormat format;
            switch (extension.Trim().ToLower())
            {
                case ".htm":
                case ".html": format = DevExpress.XtraRichEdit.DocumentFormat.Html; break;
                case ".doc": format = DevExpress.XtraRichEdit.DocumentFormat.Doc; break;
                case ".docx": format = DevExpress.XtraRichEdit.DocumentFormat.OpenXml; break;
                case ".rtf": format = DevExpress.XtraRichEdit.DocumentFormat.Rtf; break;
                default: format = DevExpress.XtraRichEdit.DocumentFormat.PlainText; break;
            }
            this.richEditControl.LoadDocument(stream, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
            this.OpenArgs = stream;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="buffer">文件Buffer</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(byte[] buffer, string extension)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            {
                this.Open(ms, extension);
            }
            this.OpenArgs = buffer;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="args">打开文件参数</param>
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
            this.richEditControl.CreateNewDocument();

            if (null != this.OpenArgs && this.OpenArgs is System.IO.Stream)
                (this.OpenArgs as System.IO.Stream).Dispose();

            this.OpenArgs = null;
        }

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionString()
        {
            if (this.richEditControl == null)
            {
                return string.Empty;
            }
            return this.richEditControl.Selection ?? string.Empty;
        }

        /// <summary>
        /// 拷贝选中的内容
        /// </summary>
        public void Copy()
        {
            if (this.richEditControl != null)
            {
                this.richEditControl.Copy();
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            if (this.richEditControl != null)
            {
                this.richEditControl.SelectAll();
            }
        }

        /// <summary>
        /// 是否支持该文件后缀
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns>是否支持该文件后缀</returns>
        public bool IsSupport(string extension)
        {
            return FileViewerConfig.Config.IsSupport(FileViewerType.Word, extension);
        }

        /// <summary>
        /// 视图类型
        /// </summary>
        public FileViewerType ViewerType
        {
            get { return FileViewerType.Word; }
        }

        #endregion

        private void richEditControl_PopupMenuShowing(object sender, DevExpress.Xpf.RichEdit.PopupMenuShowingEventArgs e)
        {
            if (this.ContextMenuActions == null)
                return;

            e.Menu.ItemLinks.Clear();

            if (this.ContextMenu != null)
            {
                this.ContextMenu.IsOpen = true;
            }
        }
    }
}
