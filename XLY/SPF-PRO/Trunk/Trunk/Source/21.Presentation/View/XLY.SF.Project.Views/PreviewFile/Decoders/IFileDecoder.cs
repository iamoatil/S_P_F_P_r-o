using System.Windows;

namespace XLY.SF.Project.Views.PreviewFile.Decoders
{
    /// <summary>
    /// 文件解码器接口
    /// </summary>
    interface IFileDecoder
    {
        /// <summary>
        /// 解码生成的界面元素
        /// </summary>
        FrameworkElement Element { get; }

        /// <summary>
        /// 解码一个文件
        /// </summary>
        /// <param name="path">文件路径</param>
        void Decode(string path);
    
    }  
}
