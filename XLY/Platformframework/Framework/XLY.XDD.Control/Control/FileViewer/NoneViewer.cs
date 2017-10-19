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
    /// 空视图预览
    /// </summary>
    public class NoneViewer : System.Windows.Controls.Control, IFileViewer
    {
        static NoneViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NoneViewer), new FrameworkPropertyMetadata(typeof(NoneViewer)));
        }

        #region IFileViewer

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Open(string path)
        {
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
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionString()
        {
            //this.pdfViewerControl.GetText(this.pdfViewerControl.
            return string.Empty;
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
            get { return FileViewerType.None; }
        }

        #endregion

    }
}
