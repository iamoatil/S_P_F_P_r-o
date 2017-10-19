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
    /// 详细信息视图
    /// </summary>
    [TemplatePart(Name = "PART_RichTextBox", Type = typeof(RichTextBox))]
    public class DetailViewer : System.Windows.Controls.Control, IFileViewer
    {
        static DetailViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DetailViewer), new FrameworkPropertyMetadata(typeof(DetailViewer)));
        }

        #region PART

        private RichTextBox PART_RichTextBox;

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_RichTextBox = this.Template.FindName("PART_RichTextBox", this) as RichTextBox;

            if (this.ContextMenuActions == null)
                return;

            ContextMenu menu = new ContextMenu();

            for (int i = 0; i < this.ContextMenuActions.Count; ++i)
            {
                MenuActionInfo info = this.ContextMenuActions[i];
                MenuItem item = new MenuItem();
                item.Header = info.Header;
                if (!info.Icon.IsNullOrEmptyOrWhiteSpace())
                {
                    Border border = new Border();
                    border.Background = new ImageBrush(StringImageConverter.Convert(info.Icon));
                    item.Icon = border;
                }
                item.InputGestureText = info.KeyText;

                item.Click += (s, a) =>
                {
                    info.DoOnClick(this, a);
                };

                menu.Items.Add(item);
            }

            this.PART_RichTextBox.ContextMenu = menu;
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
            DependencyProperty.Register("ContextMenuActions", typeof(MenuActionInfoCollection), typeof(DetailViewer), new UIPropertyMetadata(null));

        #endregion

        /// <summary>
        /// 设置显示信息
        /// </summary>
        public Action<RichTextBox, FileDetailInfo> SetDisplayAction;

        #region IFileViewer


        public void OpenEx(string path, string displayPath)
        {
            FileDetailInfo info = new FileDetailInfo(path);
            if (this.SetDisplayAction != null)
                this.SetDisplayAction(this.PART_RichTextBox, info);
            else
            {
                string tmpPath = displayPath.IsNullOrEmptyOrWhiteSpace() ? info.FullFileName : displayPath;
                string tmpDir = string.Empty;
                if (tmpPath != null)
                {
                    var index = tmpPath.LastIndexOf(@"\");
                    if (index > -1)
                    {
                        tmpDir = tmpPath.Substring(0, tmpPath.LastIndexOf(@"\"));
                    }
                    else
                    {
                        tmpDir = info.FileDirectory;
                    }
                }

                string sizeStr;
                if (info.FileType == "文件夹")
                {
                    sizeStr = string.Empty;
                }
                else
                {
                    sizeStr = System.Utility.Helper.File.GetFileSize(info.FileLength);
                }

                string display = "文件名称：" + info.FileName + Environment.NewLine +
                                 "文件全称：" + tmpPath + Environment.NewLine +
                                 "文件大小：" + sizeStr + Environment.NewLine +
                                "文件目录：" + tmpDir + Environment.NewLine +
                                "创建时间：" + info.FileCreationDate + Environment.NewLine +
                                "修改时间：" + info.FileModification + Environment.NewLine +
                                "文件类型：" + info.FileType + Environment.NewLine +
                                "文件分类：" + info.FileCategory;

                this.PART_RichTextBox.Document.Blocks.Clear();
                Paragraph p = new Paragraph();
                p.LineHeight = this.FontSize * 1.5;
                p.Inlines.Add(display);
                this.PART_RichTextBox.Document.Blocks.Add(p);
            }
            this.OpenArgs = path;
        }

        public void OpenFileX(string display, string displayPath)
        {
            this.PART_RichTextBox.Document.Blocks.Clear();
            Paragraph p = new Paragraph();
            p.LineHeight = this.FontSize * 1.5;
            p.Inlines.Add(display);
            this.PART_RichTextBox.Document.Blocks.Add(p);
            this.OpenArgs = displayPath;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Open(string path)
        {
            FileDetailInfo info = new FileDetailInfo(path);
            if (this.SetDisplayAction != null)
                this.SetDisplayAction(this.PART_RichTextBox, info);
            else
            {
                double toMb = 1024d * 1024d;
                string display = "文件名称：" + info.FileName + Environment.NewLine +
                                                      "文件全称：" + info.FullFileName + Environment.NewLine +
                                                      "文件大小：" + Math.Round(info.FileLength / toMb, 2, MidpointRounding.AwayFromZero) +
                                                                     " MB ( " + info.FileLength + "  字节 )" + Environment.NewLine +
                                                      "文件目录：" + info.FileDirectory + Environment.NewLine +
                                                      "创建时间：" + info.FileCreationDate + Environment.NewLine +
                                                      "修改时间：" + info.FileModification + Environment.NewLine +
                                                      "文件类型：" + info.FileType + Environment.NewLine +
                                                      "文件分类：" + info.FileCategory;
                this.PART_RichTextBox.Document.Blocks.Clear();
                Paragraph p = new Paragraph();
                p.LineHeight = this.FontSize * 1.5;
                p.Inlines.Add(display);
                this.PART_RichTextBox.Document.Blocks.Add(p);
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
            this.PART_RichTextBox.Document.Blocks.Clear();
            if (OpenArgs != null && OpenArgs is System.IO.Stream)
                (OpenArgs as System.IO.Stream).Dispose();

            OpenArgs = null;
        }

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionString()
        {
            if (this.PART_RichTextBox == null || this.PART_RichTextBox.Selection == null)
            {
                return string.Empty;
            }
            return this.PART_RichTextBox.Selection.Text ?? string.Empty;
        }

        /// <summary>
        /// 拷贝选中的内容
        /// </summary>
        public void Copy()
        {
            if (this.PART_RichTextBox != null)
                this.PART_RichTextBox.Copy();
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            if (this.PART_RichTextBox != null)
                this.PART_RichTextBox.SelectAll();
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
            get { return FileViewerType.Detail; }
        }

        #endregion


    }
}
