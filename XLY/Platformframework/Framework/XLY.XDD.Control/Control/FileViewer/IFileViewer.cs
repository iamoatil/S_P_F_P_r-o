using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文件预览接口
    /// </summary>
    public interface IFileViewer
    {
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        void Open(string path);

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">要打开的文件扩展名</param>
        void Open(System.IO.Stream stream, string extension);

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="buffer">文件Buffer</param>
        /// <param name="extension">要打开的文件扩展名</param>
        void Open(byte[] buffer, string extension);

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="args">打开文件参数</param>
        void Open(IFileViewerArgs args);

        /// <summary>
        /// 打开参数
        /// </summary>
        object OpenArgs { get; set; }

        /// <summary>
        /// 关闭文件
        /// </summary>
        void Close();

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        string GetSelectionString();

        /// <summary>
        /// 拷贝选中的内容
        /// </summary>
        void Copy();

        /// <summary>
        /// 全选
        /// </summary>
        void SelectAll();

        /// <summary>
        /// 是否支持该文件后缀
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns>是否支持该文件后缀</returns>
        bool IsSupport(string extension);

        /// <summary>
        /// 视图类别
        /// </summary>
        FileViewerType ViewerType { get; }
    }
}
