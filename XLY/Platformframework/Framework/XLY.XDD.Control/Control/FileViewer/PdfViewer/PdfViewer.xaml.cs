using System;
using System.Collections.Generic;
using System.IO;
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
using DevExpress.Xpf.Utils;
using MoonPdfLib.MuPdf;

namespace XLY.XDD.Control
{
    /// <summary>
    /// PdfViewer.xaml 的交互逻辑
    /// </summary>
    public partial class PdfViewer : UserControl, IFileViewer
    {
        public PdfViewer()
        {
            InitializeComponent();
        }

        #region IFileViewer

        //        /// <summary>
        //        /// 打开文件
        //        /// </summary>
        //        /// <param name="path">文件路径</param>
        //        public void Open(string path)
        //        {
        //            using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        //            {
        //                this.pdfViewerControl.DocumentSource = fs;
        //            }
        //            this.OpenArgs = path;
        //        }
        //
        //        /// <summary>
        //        /// 打开文件
        //        /// </summary>
        //        /// <param name="stream">文件流</param>
        //        /// <param name="extension">要打开的文件扩展名</param>
        //        public void Open(System.IO.Stream stream, string extension)
        //        {
        //            this.pdfViewerControl.DocumentSource = stream.CopyAllBytes();
        //            this.OpenArgs = stream;
        //        }
        //
        //        /// <summary>
        //        /// 打开文件
        //        /// </summary>
        //        /// <param name="buffer">文件Buffer</param>
        //        /// <param name="extension">要打开的文件扩展名</param>
        //        public void Open(byte[] buffer, string extension)
        //        {
        //            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
        //            {
        //                this.Open(ms, extension);
        //            }
        //            this.OpenArgs = buffer;
        //        }
        //
        //        /// <summary>
        //        /// 打开文件
        //        /// </summary>
        //        /// <param name="args">打开文件参数</param>
        //        public void Open(IFileViewerArgs args)
        //        {
        //            if (args.Type == FileViewerArgsType.Path)
        //                this.Open(args.Path);
        //            else if (args.Type == FileViewerArgsType.Stream)
        //                this.Open(args.Stream, args.Extension);
        //            else if (args.Type == FileViewerArgsType.Buffer)
        //                this.Open(args.Buffer, args.Extension);
        //            this.OpenArgs = args;
        //        }
        //
        //        /// <summary>
        //        /// 打开参数
        //        /// </summary>
        //        public object OpenArgs { get; set; }
        //
        //        /// <summary>
        //        /// 关闭文件
        //        /// </summary>
        //        public void Close()
        //        {
        //            this.pdfViewerControl.DocumentSource = null;
        //        }
        //
        //        /// <summary>
        //        /// 获取选中的文本内容
        //        /// </summary>
        //        /// <returns>当前选中的内容文本</returns>
        //        public string GetSelectionString()
        //        {
        //            return string.Empty;
        //        }   
        //        
        //        /// <summary>
        //        /// 拷贝选中的内容
        //        /// </summary>
        //        public void Copy()
        //        {
        //
        //        }
        //
        //        /// <summary>
        //        /// 全选
        //        /// </summary>
        //        public void SelectAll()
        //        {
        //
        //        }
        //
        //        /// <summary>
        //        /// 是否支持该文件后缀
        //        /// </summary>
        //        /// <param name="extension">文件后缀</param>
        //        /// <returns>是否支持该文件后缀</returns>
        //        public bool IsSupport(string extension)
        //        {
        //            return FileViewerConfig.Config.IsSupport(FileViewerType.Pdf, extension);
        //        }
        //
        //        /// <summary>
        //        /// 视图类型
        //        /// </summary>
        //        public FileViewerType ViewerType
        //        {
        //            get { return FileViewerType.Pdf; }
        //        }

        #endregion

        #region IFileViewerNew

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Open(string path)
        {
            try
            {
                var pdfTextBytes = File.ReadAllBytes(path);
                if (!IsPdfFile(pdfTextBytes))
                {
                    moonPdfPanel.Unload();
                    return;
                }
                moonPdfPanel.OpenFile(pdfTextBytes);
                this.OpenArgs = path;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(System.IO.Stream stream, string extension)
        {
            try
            {
                var pdfTextBytes = stream.CopyAllBytes();
                if (!IsPdfFile(pdfTextBytes))
                {
                    moonPdfPanel.Unload();
                    return;
                }
                moonPdfPanel.OpenFile(pdfTextBytes);
                this.OpenArgs = stream;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="buffer">文件Buffer</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(byte[] buffer, string extension)
        {
            try
            {
                if (IsPdfFile(buffer))
                {
                    moonPdfPanel.Unload();
                    return;
                }
                moonPdfPanel.OpenFile(buffer);
                this.OpenArgs = buffer;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
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
            moonPdfPanel.Unload();

            if (this.OpenArgs != null && this.OpenArgs is Stream)
                (this.OpenArgs as Stream).Dispose();

            this.OpenArgs = null;
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
            return FileViewerConfig.Config.IsSupport(FileViewerType.Pdf, extension);
        }

        /// <summary>
        /// 视图类型
        /// </summary>
        public FileViewerType ViewerType
        {
            get { return FileViewerType.Pdf; }
        }

        /// <summary>
        /// 是否是PDF文件
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private bool IsPdfFile(byte[] bytes)
        {
            if (bytes == null)
            {
                return false;
            }

            if (bytes.Length <= 4)
            {
                return false;
            }
            byte[] pdfFlag = { 0x25, 0x50, 0x44, 0x46 };
            return bytes[0] == pdfFlag[0] && bytes[1] == pdfFlag[1] && bytes[2] == pdfFlag[2] && bytes[3] == pdfFlag[3];
        }

        #endregion
    }
}