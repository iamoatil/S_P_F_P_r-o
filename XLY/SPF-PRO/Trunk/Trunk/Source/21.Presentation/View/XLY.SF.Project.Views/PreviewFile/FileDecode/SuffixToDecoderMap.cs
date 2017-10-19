using System.Collections.Generic;
using XLY.SF.Project.Views.PreviewFile.Decoders;

namespace XLY.SF.Project.Views.PreviewFile.FileDecode
{
   /// <summary>
   /// 后缀和文件解码器映射类
   /// 功能：建立后缀与特定文件解码器的映射关系
   /// </summary>
    class SuffixToFileDecoderMap
    {
        /// <summary>
        /// 后缀与解码器的映射关系
        /// </summary>
        private readonly Dictionary<string, IFileDecoder> _suffixToDecoderDic = new Dictionary<string, IFileDecoder>();

        public SuffixToFileDecoderMap(FileDecoderCollection manager)
        {       
            MapPathToFile(".mp3", manager.AudioFile);
            MapPathToFile(".bin", manager.BinaryFile);
            MapPathToFile(".html|.Xml", manager.HtmlFile);
            MapPathToFile(".jpg", manager.PictureFile);
            MapPathToFile(".txt|.ini", manager.TextFile);
            MapPathToFile(".avi|.rmvb|.mp4", manager.VideoFile);
        }

        private void MapPathToFile(string suffixExp,IFileDecoder file)
        {
            string[] suffixes = suffixExp.Split('|');
            foreach (var suffix in suffixes)
            {
                _suffixToDecoderDic.Add(suffix,file);
            }
        }

        /// <summary>
        /// 根据后缀返回一个文件解码器
        /// </summary>
        /// <returns></returns>
        public IFileDecoder GetDecoder(string suffix)
        {
            if (_suffixToDecoderDic.ContainsKey(suffix))
            {
                return _suffixToDecoderDic[suffix];
            }
            return null;
        }
    }    
}
