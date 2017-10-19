using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using RichTextBox = System.Windows.Forms.RichTextBox;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文本预览，辅助读取的方式由ExpandRead实现
    /// </summary>
    [TemplatePart(Name = "PART_TextBox", Type = typeof(XlyHighlightTextBox))]
    public class TextViewer : System.Windows.Controls.Control, IFileViewer
    {

        static TextViewer()
        {

            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextViewer), new FrameworkPropertyMetadata(typeof(TextViewer)));
        }

        #region PART

        private XlyHighlightTextBox PART_TextBox;

        #endregion


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_TextBox = this.Template.FindName("PART_TextBox", this) as XlyHighlightTextBox;

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

            if (this.PART_TextBox.PART_RichTextBox == null)
            {
                this.PART_TextBox.ApplyTemplate();
            }

            this.PART_TextBox.PART_RichTextBox.ContextMenu = menu;
        }

        #region ContextMenuActions -- 右键菜单

        /// <summary>
        /// 右键菜单
        /// </summary>
        public MenuActionInfoCollection ContextMenuActions
        {
            get { return (MenuActionInfoCollection)GetValue(ContextMenuActionsProperty); }
            set { SetValue(ContextMenuActionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContextMenuActions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextMenuActionsProperty =
            DependencyProperty.Register("ContextMenuActions", typeof(MenuActionInfoCollection), typeof(TextViewer), new UIPropertyMetadata(null));

        #endregion

        /// <summary>
        /// 当打开时触发
        /// </summary>
        public event EventHandler<TextViewerEventArgs> OnOpen;

        /// <summary>
        /// 清理高亮部分
        /// </summary>
        private void _ClearHighlight()
        {
            this.PART_TextBox.IsAutoUpdateHighlight = false;
            this.PART_TextBox.HighlightLength = 0;
            this.PART_TextBox.HighlightOffset = 0;
            this.PART_TextBox.HighlightText = string.Empty;
        }

        /// <summary>
        /// 尝试打开并获取文本部分
        /// </summary>
        /// <param name="path"></param>
        private string _Open_GetText(string path)
        {
            string extension = System.IO.Path.GetExtension(path);
            if (this.OnOpen != null)
            {
                if (Regex.IsMatch(extension, "^(?i).?rtf$"))
                {
                    try
                    {
                        RichTextBox _RichText = new RichTextBox();
                        _RichText.Rtf = File.ReadAllText(path);
                        return _RichText.Text;
                    }
                    catch (Exception)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    // 临时的外置文本读取
                    TextViewerEventArgs args = new TextViewerEventArgs();
                    args.Path = path;
                    this.OnOpen(this, args);
                    if (args.IsSuccess)
                    {
                        return args.Context;
                    }
                    else
                    {
                        if (ReadToTxt.ReadToTxtHelper.IsSupport(extension))
                        {
                            return ReadToTxt.ReadToTxtHelper.GetContent(path);
                        }
                        else
                        {
                            using (StreamReader sr = new StreamReader(path))
                            {
                                string text = sr.ReadToEnd();
                                return text;
                            }
                        }
                    }
                }

            }
            else
            {
                if (Regex.IsMatch(extension, "^(?i).?rtf$"))
                {
                    try
                    {
                        RichTextBox _RichText = new RichTextBox();
                        _RichText.Rtf = File.ReadAllText(path);
                        return _RichText.Text;
                    }
                    catch (Exception)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    if (ReadToTxt.ReadToTxtHelper.IsSupport(extension))
                    {
                        return ReadToTxt.ReadToTxtHelper.GetContent(path);
                    }
                    else
                    {
                        using (StreamReader sr = new StreamReader(path))
                        {
                            string text = sr.ReadToEnd();
                            return text;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 执行文件打开
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        private void DoOpen(string path, Encoding encoding)
        {
            this._ClearHighlight();
            string text = this._Open_GetText(path);
            if (encoding == null)
                this.PART_TextBox.Text = text;
            else
                this.PART_TextBox.Text = encoding.GetString(System.Text.Encoding.Default.GetBytes(text).Skip(encoding.GetPreamble().Length).ToArray());
        }

        #region IFileViewer

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Open(string path)
        {
            this.DoOpen(path, null);
            this.PART_TextBox.UpdateHighlight();
            this.OpenArgs = path;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(System.IO.Stream stream, string extension)
        {
            this._ClearHighlight();
            StreamReader sr = new StreamReader(stream);
            string text = sr.ReadToEnd();
            this.PART_TextBox.Text = text;
            this.PART_TextBox.UpdateHighlight();
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
            this._ClearHighlight();
            using (MemoryStream ms = new MemoryStream(buffer))
            using (StreamReader sr = new StreamReader(ms))
            {
                string text = sr.ReadToEnd();
                this.PART_TextBox.Text = text;
                this.PART_TextBox.UpdateHighlight();
            }
            this.OpenArgs = buffer;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="args">打开文件参数</param>
        public void Open(IFileViewerArgs args)
        {
            if (args is IFileViewerHighlightArgs)
            {
                IFileViewerHighlightArgs ifvha = args as IFileViewerHighlightArgs;
                this.PART_TextBox.IsAutoUpdateHighlight = false;
                if (args.Encoding != null)
                {
                    this.DoOpen(args.Path, args.Encoding);
                    this.PART_TextBox.HighlightMode = ifvha.HighlightMode;
                    this.PART_TextBox.HighlightText = ifvha.HighlightText;
                    this.PART_TextBox.HighlightOffset = ifvha.HighlightOffset;
                    this.PART_TextBox.HighlightLength = ifvha.HighlightLength;
                    this.PART_TextBox.UpdateHighlight();
                    return;
                }
            }
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
            if (null != OpenArgs && OpenArgs is Stream)
                (OpenArgs as Stream).Dispose();

            this.OpenArgs = null;
            if (this.PART_TextBox != null)
            {
                this.PART_TextBox.Text = string.Empty;
                this.PART_TextBox.HighlightLength = 0;
                this.PART_TextBox.HighlightText = string.Empty;
                this.PART_TextBox.HighlightOffset = -1;
                this.PART_TextBox.UpdateHighlight();
            }
        }

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionString()
        {
            if (this.PART_TextBox == null)
            {
                return string.Empty;
            }
            return this.PART_TextBox.GetSelectionText();
        }

        /// <summary>
        /// 拷贝选中的内容
        /// </summary>
        public void Copy()
        {
            if (this.PART_TextBox != null)
                this.PART_TextBox.Copy();
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            if (this.PART_TextBox != null)
            {
                this.PART_TextBox.SelectAll();
            }
        }

        /// <summary>
        /// 是否支持该文件后缀
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns>是否支持该文件后缀</returns>
        public bool IsSupport(string extension)
        {
            return FileViewerConfig.Config.IsSupport(FileViewerType.Text, extension);
        }

        /// <summary>
        /// 视图类型
        /// </summary>
        public FileViewerType ViewerType
        {
            get { return FileViewerType.Text; }
        }

        #endregion
    }
}
