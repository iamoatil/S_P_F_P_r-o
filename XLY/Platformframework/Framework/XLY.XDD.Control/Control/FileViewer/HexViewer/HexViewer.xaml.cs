using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XLY.XDD.Control.EncodingChecker.Ude;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using Be.Windows.Forms;
using Be.Windows.Forms;
using Path = System.Windows.Shapes.Path;

namespace XLY.XDD.Control
{
    /// <summary>
    /// HexPreviewControl.xaml 的交互逻辑
    /// </summary>
    public partial class HexViewer : UserControl, IFileViewer
    {
        public HexViewer()
        {
            InitializeComponent();
            this.hexBox.EncodingPain += this._DoOpenFile;
            this.hexBox.EncodingBase64Pain += BindBase64Data;
            this.hexBox.EncodingPain_Stream += this._DoOpenFile;
            this.hexBox.EncodingBase64Pain_Stream += this._DoOpenFile;
            this.hexBox.OnUpdateVScroll += new EventHandler<UpdateVScrollEventArgs>(hexBox_OnUpdateVScroll);

            this.hexBox.Init(this);
        }

        private void vScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            System.Windows.Forms.ScrollEventType type;
            switch (e.ScrollEventType)
            {
                case System.Windows.Controls.Primitives.ScrollEventType.Last:
                    type = System.Windows.Forms.ScrollEventType.Last;
                    break;
                case System.Windows.Controls.Primitives.ScrollEventType.EndScroll:
                    type = System.Windows.Forms.ScrollEventType.EndScroll;
                    break;
                case System.Windows.Controls.Primitives.ScrollEventType.SmallIncrement:
                    type = System.Windows.Forms.ScrollEventType.SmallIncrement;
                    break;
                case System.Windows.Controls.Primitives.ScrollEventType.SmallDecrement:
                    type = System.Windows.Forms.ScrollEventType.SmallDecrement;
                    break;
                case System.Windows.Controls.Primitives.ScrollEventType.LargeIncrement:
                    type = System.Windows.Forms.ScrollEventType.LargeIncrement;
                    break;
                case System.Windows.Controls.Primitives.ScrollEventType.LargeDecrement:
                    type = System.Windows.Forms.ScrollEventType.LargeDecrement;
                    break;
                case System.Windows.Controls.Primitives.ScrollEventType.ThumbPosition:
                    type = System.Windows.Forms.ScrollEventType.ThumbPosition;
                    break;
                case System.Windows.Controls.Primitives.ScrollEventType.ThumbTrack:
                    type = System.Windows.Forms.ScrollEventType.ThumbTrack;
                    break;
                case System.Windows.Controls.Primitives.ScrollEventType.First:
                    type = System.Windows.Forms.ScrollEventType.First;
                    break;
                default:
                    type = System.Windows.Forms.ScrollEventType.First;
                    break;
            }

            System.Windows.Forms.ScrollEventArgs args = new System.Windows.Forms.ScrollEventArgs(type, (int)e.NewValue);

            this.hexBox._vScrollBar_Scroll(this.vScrollBar, args);
        }

        #region IsOnlyStringView -- 是否只有字符视图

        /// <summary>
        /// 是否只有字符视图
        /// </summary>
        public bool IsOnlyStringView
        {
            get { return (bool)GetValue(IsOnlyStringViewProperty); }
            set { SetValue(IsOnlyStringViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOnlyStringView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOnlyStringViewProperty =
            DependencyProperty.Register("IsOnlyStringView", typeof(bool), typeof(HexViewer), new UIPropertyMetadata(false, new PropertyChangedCallback((s, e) =>
            {
                HexViewer viewer = s as HexViewer;
                viewer.hexBox.OnlyStringView = (bool)e.NewValue;
            })));

        #endregion

        #region BlackColor -- 背景色

        /// <summary>
        /// 背景色
        /// </summary>
        public SolidColorBrush BlackColor
        {
            get { return (SolidColorBrush)GetValue(BlackColorProperty); }
            set { SetValue(BlackColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlackColorProperty =
            DependencyProperty.Register("BlackColor", typeof(SolidColorBrush), typeof(HexViewer), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                HexViewer viewer = s as HexViewer;

                SolidColorBrush brush = e.NewValue as SolidColorBrush;
                System.Drawing.ColorConverter converter = new System.Drawing.ColorConverter();
                viewer.hexBox.BackColor = (System.Drawing.Color)converter.ConvertFromString(brush.ToString());
            })));

        #endregion

        #region ForeColor -- 前景色

        /// <summary>
        /// 前景色
        /// </summary>
        public SolidColorBrush ForeColor
        {
            get { return (SolidColorBrush)GetValue(ForeColorProperty); }
            set { SetValue(ForeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForeColorProperty =
            DependencyProperty.Register("ForeColor", typeof(SolidColorBrush), typeof(HexViewer), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                HexViewer viewer = s as HexViewer;

                SolidColorBrush brush = e.NewValue as SolidColorBrush;
                System.Drawing.ColorConverter converter = new System.Drawing.ColorConverter();
                viewer.hexBox.ForeColor = (System.Drawing.Color)converter.ConvertFromString(brush.ToString());
            })));

        #endregion

        #region StatusBarBlackColor -- 状态栏背景色

        /// <summary>
        /// 状态栏背景色
        /// </summary>
        public Brush StatusBarBlackColor
        {
            get { return (Brush)GetValue(StatusBarBlackColorProperty); }
            set { SetValue(StatusBarBlackColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusBarBlackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarBlackColorProperty =
            DependencyProperty.Register("StatusBarBlackColor", typeof(Brush), typeof(HexViewer), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));

        #endregion

        #region StatusBarForeColor -- 状态栏前景色

        /// <summary>
        /// 状态栏前景色
        /// </summary>
        public Brush StatusBarForeColor
        {
            get { return (Brush)GetValue(StatusBarForeColorProperty); }
            set { SetValue(StatusBarForeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusBarForeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarForeColorProperty =
            DependencyProperty.Register("StatusBarForeColor", typeof(Brush), typeof(HexViewer), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region IsUserContextMenuOverride -- 是否使用用户右键菜单覆盖默认的右键菜单

        /// <summary>
        /// 是否使用用户右键菜单覆盖默认的右键菜单
        /// </summary>
        public bool IsUserContextMenuOverride
        {
            get { return (bool)GetValue(IsUserContextMenuOverrideProperty); }
            set { SetValue(IsUserContextMenuOverrideProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUserContextMenuOverride.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUserContextMenuOverrideProperty =
            DependencyProperty.Register("IsUserContextMenuOverride", typeof(bool), typeof(HexViewer), new UIPropertyMetadata(false));

        #endregion

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
            DependencyProperty.Register("ContextMenuActions", typeof(MenuActionInfoCollection), typeof(HexViewer), new UIPropertyMetadata(null));

        #endregion

        /// <summary>
        /// 更新垂直滚动条
        /// </summary>
        private void hexBox_OnUpdateVScroll(object sender, UpdateVScrollEventArgs e)
        {
            if (e.Maximum.HasValue)
                this.vScrollBar.Maximum = e.Maximum.Value;
            if (e.Minimum.HasValue)
                this.vScrollBar.Minimum = e.Minimum.Value;
            if (e.Value.HasValue)
                this.vScrollBar.Value = e.Value.Value;
            this.vScrollBar.IsEnabled = e.IsEnabled;
        }

        #region Hex

        private string _fileName;

        private byte[] _findBuffer = new byte[0];

        public bool IsGoToGetDefaultCharset;

        /// <summary>
        /// 使用流打开文件
        /// </summary>
        /// <param name="stream">文件流</param>
        private void _DoOpenFile(Stream stream)
        {
            hexBox.fileName = null;
            DynamicFileByteProvider dynamicFileByteProvider;
            try
            {
                var bugFix = stream.ReadByte();
                stream.Position = 0;
                this.hexBox.stream = stream;
                try
                {

                    try
                    {
                        Encoding _encoding = this.hexBox._encoding;
                        if (this.hexBox.IsUseBase64 && stream.Length < 1024 * 1024 * 10)
                        {
                            this.hexBox.IsUseEncoding = true;
                            // 读成自己的解码后的流
                            try
                            {
                                byte[] data = new byte[4 * 1024]; //Chunk size is 4k
                                stream.Seek(0, SeekOrigin.Begin);
                                int read = stream.Read(data, 0, data.Length);

                                byte[] chunk = new byte[4 * 1024];
                                List<byte> list = new List<byte>();
                                while (read > 0)
                                {
                                    chunk = Convert.FromBase64String(_encoding.GetString(data, 0, read));

                                    list.AddRange(chunk);
                                    read = stream.Read(data, 0, data.Length);

                                }

                                var decodeBytes = list.ToArray();

                                // 放入流中
                                Stream ms = new MemoryStream(decodeBytes);

                                stream.Close();
                                //dynamicFileByteProvider = new DynamicFileByteProvider(fileName);
                                dynamicFileByteProvider = new DynamicFileByteProvider(ms, _encoding);
                            }
                            catch (Exception ex)
                            {
                                var tmp = @"Base64编码无效";
                                this.hexBox._encoding = Encoding.UTF8;
                                byte[] bytes = this.hexBox._encoding.GetBytes(tmp);
                                Stream tmp_stream = new MemoryStream(bytes);

                                // try to open in read-only mode
                                dynamicFileByteProvider = new DynamicFileByteProvider(tmp_stream);

                                this.hexBox.IsUseBase64 = false;
                            }

                        }
                        else
                        {
                            // try to open in write mode
                            dynamicFileByteProvider = new DynamicFileByteProvider(stream);

                        }
                    }
                    catch (IOException) // write mode failed
                    {
                        try
                        {
                            // try to open in read-only mode
                            dynamicFileByteProvider = new DynamicFileByteProvider(stream);

                        }
                        catch (IOException) // read-only also failed
                        {
                            // file cannot be opened
                            return;
                        }
                    }
                    if (this.IsGoToGetDefaultCharset)
                    {
                        hexBox._encoding = dynamicFileByteProvider.GetEncoding();
                        IsGoToGetDefaultCharset = false;
                    }
                    hexBox.ByteProvider = dynamicFileByteProvider;
                    this.fileSizeToolStripStatusLabel.Text = Util.GetDisplayBytes(this.hexBox.ByteProvider.Length);
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                byte[] data = new byte[0];
                Stream stream1 = new MemoryStream(data);
                dynamicFileByteProvider = new DynamicFileByteProvider(stream1);
                hexBox.ByteProvider = dynamicFileByteProvider;
                this.fileSizeToolStripStatusLabel.Text = Util.GetDisplayBytes(this.hexBox.ByteProvider.Length);
            }

        }

        /// <summary>
        /// 执行打开文件
        /// </summary>
        /// <param name="fileName"></param>
        private void _DoOpenFile(string fileName)
        {
            try
            {
                hexBox.stream = null;

                DynamicFileByteProvider dynamicFileByteProvider;
                try
                {
                    Encoding _encoding = this.hexBox._encoding;
                    if (this.hexBox.IsUseBase64)
                    {
                        this.hexBox.IsUseEncoding = true;
                        // 读成自己的解码后的流
                        try
                        {
                            FileStream inputStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                            byte[] data = new byte[4 * 1024]; //Chunk size is 4k
                            inputStream.Seek(0, SeekOrigin.Begin);
                            int read = inputStream.Read(data, 0, data.Length);

                            byte[] chunk = new byte[4 * 1024];
                            List<byte> list = new List<byte>();
                            while (read > 0)
                            {
                                chunk = Convert.FromBase64String(_encoding.GetString(data, 0, read));

                                list.AddRange(chunk);
                                read = inputStream.Read(data, 0, data.Length);

                            }

                            var decodeBytes = list.ToArray();

                            // 放入流中
                            Stream stream = new MemoryStream(decodeBytes);

                            inputStream.Close();
                            //dynamicFileByteProvider = new DynamicFileByteProvider(fileName);
                            dynamicFileByteProvider = new DynamicFileByteProvider(stream, _encoding);
                        }
                        catch (Exception ex)
                        {
                            var tmp = @"Base64编码无效";
                            this.hexBox._encoding = Encoding.UTF8;
                            byte[] bytes = this.hexBox._encoding.GetBytes(tmp);
                            Stream stream = new MemoryStream(bytes);

                            // try to open in read-only mode
                            dynamicFileByteProvider = new DynamicFileByteProvider(stream);

                            this.hexBox.IsUseBase64 = false;
                        }

                    }
                    else
                    {
                        if (!System.Utility.Helper.File.IsValid(fileName))
                        {
                            byte[] data = new byte[0];
                            Stream stream = new MemoryStream(data);
                            dynamicFileByteProvider = new DynamicFileByteProvider(stream);
                            hexBox.ByteProvider = dynamicFileByteProvider;
                            _fileName = fileName;
                            this.fileSizeToolStripStatusLabel.Text = Util.GetDisplayBytes(this.hexBox.ByteProvider.Length);
                            return;
                        }
                        // try to open in write mode
                        dynamicFileByteProvider = new DynamicFileByteProvider(fileName, true,
                                                                              this.IsGoToGetDefaultCharset);

                    }
                }
                catch (IOException) // write mode failed
                {
                    try
                    {
                        if (!System.Utility.Helper.File.IsValid(fileName))
                        {
                            return;
                        }
                        // try to open in read-only mode
                        dynamicFileByteProvider = new DynamicFileByteProvider(fileName, true);

                    }
                    catch (IOException) // read-only also failed
                    {
                        // file cannot be opened
                        return;
                    }
                }
                if (!System.Utility.Helper.File.IsValid(fileName))
                {
                    return;
                }
                if (this.IsGoToGetDefaultCharset)
                {
                    hexBox._encoding = dynamicFileByteProvider.GetEncoding();
                    IsGoToGetDefaultCharset = false;
                }
                hexBox.ByteProvider = dynamicFileByteProvider;
                _fileName = fileName;
                this.fileSizeToolStripStatusLabel.Text = Util.GetDisplayBytes(this.hexBox.ByteProvider.Length);
            }
            catch
            {
                byte[] data = new byte[0];
                Stream stream = new MemoryStream(data);
                var dynamicFileByteProvider = new DynamicFileByteProvider(stream);
                hexBox.ByteProvider = dynamicFileByteProvider;
                _fileName = fileName;
                this.fileSizeToolStripStatusLabel.Text = Util.GetDisplayBytes(this.hexBox.ByteProvider.Length);
                return;
            }
        }

        #endregion

        public readonly string[] officearr =
            {
                ".txt",
                ".log",
                ".ini",
                ".html",
                ".htm",
                ".xml",
                ".xaml",
                ".js"
            };

        public void BindBase64Data(string filePath)
        {
            this.hexBox.IsUseBase64 = true;
            this.hexBox._encoding = Encoding.UTF8;
            this._DoOpenFile(filePath);
        }

        #region IFileViewer

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Open(string path)
        {
            string extension = System.IO.Path.GetExtension(path).Trim().ToLower();
            if (officearr.Contains(extension))
            {
                IsGoToGetDefaultCharset = true;
                this.hexBox.IsUseEncoding = true;
            }
            else
            {
                IsGoToGetDefaultCharset = false;
                this.hexBox.IsUseEncoding = false;
            }

            if (this.hexBox.IsUseBase64)
            {
                this.hexBox.IsUseBase64 = false;
            }

            this._DoOpenFile(path);
            this.OpenArgs = path;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(System.IO.Stream stream, string extension)
        {

            if (officearr.Contains(extension))
            {
                IsGoToGetDefaultCharset = true;
                this.hexBox.IsUseEncoding = true;
            }
            else
            {
                IsGoToGetDefaultCharset = false;
                this.hexBox.IsUseEncoding = false;
            }

            if (this.hexBox.IsUseBase64)
            {
                this.hexBox.IsUseBase64 = false;
            }

            this._DoOpenFile(stream);
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
            if (this.OpenArgs != null)
            {
                if (this.OpenArgs is Stream)
                    (this.OpenArgs as Stream).Dispose();

                this.OpenArgs = null;
                this.hexBox.stream = null;
                this.hexBox.ByteProvider = null;
            }
        }

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionString()
        {
            if (this.hexBox != null)
                return this.hexBox.GetSelectionString();
            else
                return string.Empty;
        }

        /// <summary>
        /// 拷贝选中的内容
        /// </summary>
        public void Copy()
        {
            if (this.hexBox != null)
            {
                this.hexBox.Copy();
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            if (this.hexBox != null)
            {
                this.hexBox.SelectAll();
            }
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
            get { return FileViewerType.Hex; }
        }

        #endregion

    }
}
