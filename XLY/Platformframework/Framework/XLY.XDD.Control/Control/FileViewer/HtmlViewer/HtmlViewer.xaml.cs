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
using System.IO;
using System.Text.RegularExpressions;
using System.Utility.Logger;

namespace XLY.XDD.Control
{
    /// <summary>
    /// HtmlViewer.xaml 的交互逻辑
    /// </summary>
    public partial class HtmlViewer : UserControl, IFileViewer
    {
        public HtmlViewer()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.webBrowser.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);

            if (this.ContextMenuActions != null)
            {
                ContextMenu cm = new ContextMenu();

                for (int i = 0; i < this.ContextMenuActions.Count; ++i)
                {
                    MenuActionInfo info = this.ContextMenuActions[i];
                    MenuItem item = new MenuItem();
                    item.Header = info.Header;
                    item.Click += (s, e) =>
                    {
                        info.DoOnClick(this, e);
                    };
                    cm.Items.Add(item);
                }

                this.ContextMenu = cm;
            }
        }

        private void webBrowser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            this.webBrowser.Document.ContextMenuShowing -= new System.Windows.Forms.HtmlElementEventHandler(Document_ContextMenuShowing);
            this.webBrowser.Document.ContextMenuShowing += new System.Windows.Forms.HtmlElementEventHandler(Document_ContextMenuShowing);
        }

        private void Document_ContextMenuShowing(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            e.ReturnValue = false;
            if (this.ContextMenu != null)
                this.ContextMenu.IsOpen = true;
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
            DependencyProperty.Register("ContextMenuActions", typeof(MenuActionInfoCollection), typeof(HtmlViewer), new UIPropertyMetadata(null));

        #endregion

        #region IFileViewer

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Open(string path)
        {
            this.webBrowser.Navigate(path);
            this.OpenArgs = path;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(System.IO.Stream stream, string extension)
        {
            string text = string.Empty;
            StreamReader sr = new StreamReader(stream);
            text = sr.ReadToEnd();
            if (Regex.IsMatch(text, @"([ᱻ�])\1"))
            {
                var match = Regex.Match(text, "(?<=(charset=))[^<]{3,}?(?=\\s*(>|/>))|(?<=(charset=\"))[^<]{3,}?(?=\"\\s*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (match.Success)
                {
                    var matchGroup = match.Groups[0].Value.Trim('\\', '\'', '\"', '/', ' ');

                    if (!matchGroup.IsNullOrEmptyOrWhiteSpace())
                    {
                        Encoding encoding = Encoding.GetEncoding(matchGroup);
                        stream.Seek(0, SeekOrigin.Begin);
                        StreamReader sr2 = new StreamReader(stream, encoding);
                        text = sr2.ReadToEnd();
                        this.webBrowser.DocumentText = text;
                    }
                    else
                    {
                        this.webBrowser.DocumentText = text;
                    }
                }
                else
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    StreamReader sr3 = new StreamReader(stream, Encoding.Default);
                    this.webBrowser.DocumentText = sr3.ReadToEnd();
                }
            }
            else
            {
                this.webBrowser.DocumentText = text;
            }
            stream.Position = 0;
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
            this.webBrowser.Navigate(string.Empty);

            if (null != this.OpenArgs && this.OpenArgs is Stream)
                (this.OpenArgs as Stream).Dispose();

            this.OpenArgs = null;
        }

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionString()
        {
            this.webBrowser.Document.ExecCommand("Copy", false, null);
            System.Windows.Forms.IDataObject ido = System.Windows.Forms.Clipboard.GetDataObject();
            string result = ido.GetData(DataFormats.Text).ToSafeString();
            System.Windows.Forms.Clipboard.Clear();
            return result;
        }

        /// <summary>
        /// 拷贝选中的内容
        /// </summary>
        public void Copy()
        {
            if (this.webBrowser != null && this.webBrowser.Document != null)
                this.webBrowser.Document.ExecCommand("Copy", false, null);
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            if (this.webBrowser != null && this.webBrowser.Document != null)
                this.webBrowser.Document.ExecCommand("SelectAll", false, null);
        }

        /// <summary>
        /// 是否支持该文件后缀
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns>是否支持该文件后缀</returns>
        public bool IsSupport(string extension)
        {
            return FileViewerConfig.Config.IsSupport(FileViewerType.Html, extension);
        }

        /// <summary>
        /// 视图类型
        /// </summary>
        public FileViewerType ViewerType
        {
            get { return FileViewerType.Html; }
        }

        #endregion
    }
}
