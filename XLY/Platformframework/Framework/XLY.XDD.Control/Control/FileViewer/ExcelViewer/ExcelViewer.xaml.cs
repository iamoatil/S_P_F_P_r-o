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
using DevExpress.Xpf.Spreadsheet.Menu.Internal;

namespace XLY.XDD.Control
{
    /// <summary>
    /// ExcelViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ExcelViewer : UserControl, IFileViewer
    {
        public ExcelViewer()
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
            DependencyProperty.Register("ContextMenuActions", typeof(MenuActionInfoCollection), typeof(ExcelViewer), new UIPropertyMetadata(null));

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
            this.spreadsheetControl.LoadDocument(path);
            this.OpenArgs = path;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(System.IO.Stream stream, string extension)
        {
            this.spreadsheetControl.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Undefined);
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
            this.spreadsheetControl.GetCommand(DevExpress.XtraSpreadsheet.Commands.SpreadsheetCommandId.FileNew).Execute(null);
            if (null!= OpenArgs&& OpenArgs is System.IO.Stream)
                (OpenArgs as System.IO.Stream).Dispose();

            OpenArgs = null;
        }

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionString()
        {
            if (this.spreadsheetControl == null || this.spreadsheetControl.Selection == null)
            {
                return string.Empty;
            }

            ICommand icommand = this.spreadsheetControl.GetCommand(DevExpress.XtraSpreadsheet.Commands.SpreadsheetCommandId.CopySelection);
            icommand.Execute(null);

            System.Windows.Forms.IDataObject ido = System.Windows.Forms.Clipboard.GetDataObject();
            string text = ido.GetData(DataFormats.Text).ToSafeString();
            System.Windows.Forms.Clipboard.Clear();

            return text;
        }

        /// <summary>
        /// 拷贝选中的内容
        /// </summary>
        public void Copy()
        {
            if (this.spreadsheetControl == null || this.spreadsheetControl.Selection == null)
                return;

            ICommand icommand = this.spreadsheetControl.GetCommand(DevExpress.XtraSpreadsheet.Commands.SpreadsheetCommandId.CopySelection);
            icommand.Execute(null);
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            if (this.spreadsheetControl == null || this.spreadsheetControl.Selection == null)
                return;

            ICommand icommand = this.spreadsheetControl.GetCommand(DevExpress.XtraSpreadsheet.Commands.SpreadsheetCommandId.SelectAll);
            icommand.Execute(null);
        }

        /// <summary>
        /// 是否支持该文件后缀
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns>是否支持该文件后缀</returns>
        public bool IsSupport(string extension)
        {
            return FileViewerConfig.Config.IsSupport(FileViewerType.Excel, extension);
        }

        /// <summary>
        /// 视图类型
        /// </summary>
        public FileViewerType ViewerType
        {
            get { return FileViewerType.Excel; }
        }

        #endregion



        private void spreadsheetControl_PopupMenuShowing(object sender, DevExpress.Xpf.Spreadsheet.Menu.PopupMenuShowingEventArgs e)
        {
            if (this.ContextMenuActions == null)
                return;

            if (e.MenuType == DevExpress.Xpf.Spreadsheet.SpreadsheetMenuType.Cell)
            {
                e.Menu.ItemLinks.Clear();

                if (this.ContextMenu != null)
                {
                    this.ContextMenu.IsOpen = true;
                }
            }
        }
    }
}
