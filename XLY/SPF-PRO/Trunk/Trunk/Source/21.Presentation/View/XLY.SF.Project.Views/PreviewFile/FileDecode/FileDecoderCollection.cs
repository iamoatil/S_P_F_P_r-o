using System.IO;
using System.Windows;
using XLY.SF.Project.Views.PreviewFile.Decoders;

namespace XLY.SF.Project.Views.PreviewFile.FileDecode
{
    /// <summary>
    /// 文件解码器集合
    /// 功能：其中包含多种可以解析文件的解码器的
    /// </summary>
    class FileDecoderCollection
    {
        /// <summary>
        /// 音频文件解码器
        /// </summary>
        public IFileDecoder AudioFile { get; private set; }

        /// <summary>
        /// 二进制文件解码器
        /// </summary>
        public IFileDecoder BinaryFile { get; private set; }

        /// <summary>
        /// Html文件解码器
        /// </summary>
        public IFileDecoder HtmlFile { get; private set; }
      
        /// <summary>
        /// 图片文件解码器
        /// </summary>
        public IFileDecoder PictureFile { get; private set; }

        /// <summary>
        /// 文本文件解码器
        /// </summary>
        public IFileDecoder TextFile { get; private set; }

        /// <summary>
        /// 视频文件解码器
        /// </summary>
        public IFileDecoder VideoFile { get; private set; }

        //后缀与文件解码器的关系
        private SuffixToFileDecoderMap _suffixToFileMap;

        public FileDecoderCollection()
        {
            //建立 各种不同类型文件解码器
            AudioFile = new AudioFileDecoder();
            BinaryFile = new BinaryFileDecoder();
            HtmlFile = new HtmlFileDecoder();
            PictureFile = new PictureFileDecoder();
            TextFile = new TextFileDecoder();
            VideoFile = new VideoFileDecoder();

            //建立后缀与文件解码器的关系，根据映射关系某一后缀就能被某种文件解码器默认打开。eg：.txt -->被TextFileEncoder打开
            _suffixToFileMap = new SuffixToFileDecoderMap(this);
        }

        /// <summary>
        /// 解析一个Path为控件
        /// </summary>
        /// <returns></returns>
        public FrameworkElement Decode(string filePath)
        {
            string suffix = Path.GetExtension(filePath);
            IFileDecoder decoder=_suffixToFileMap.GetDecoder(suffix);
            if(decoder == null)
            {
                decoder= this.BinaryFile;
            }
            decoder.Decode(filePath);
            return decoder.Element;
        }
    }
}
