using System;
using System.IO;

/* ==============================================================================
* Description：LargeFile  
* Author     ：litao
* Create Date：2017/10/16 11:05:18
* ==============================================================================*/

namespace XLY.SF.Project.Views.PreviewFile.LargeFileLoad
{
    /// <summary>
    /// 大文件
    /// 解析：它代表Windows上的一个文件
    /// 目的：解决在TextBox中显示大文件卡的问题。
    /// 方法：实时动态装载，内容显示。
    /// </summary>
    public class LargeFile : IDisposable
    {
        public LargeFile(string path)
        {
            _fileInfo = new FileInfo(path);
            _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        readonly FileInfo _fileInfo;
        readonly FileStream _fileStream;

        /// <summary>
        /// 正在读取的文件的长度
        /// </summary>
        public int FileContentLength { get { return (int)_fileInfo.Length; } }

        /// <summary>
        /// 在位置offset处读取数据到bytes数组中,此位置为“当前位置”相对于“文件头”的偏置
        ///todo 这里面修改了offset的值，然后用一个新的offset来装载数据
        /// </summary>
        public int Read(ref int offset, byte[] bytes)
        {
            int fragmentLength = bytes.Length;
            //offset到文件结尾的距离大于等于FragmentLength。
            if (offset > FileContentLength - fragmentLength)
            {
                offset = FileContentLength - fragmentLength;
            }
            //offset不能为负数
            if (offset < 0)
            {
                offset = 0;
            }
            
            _fileStream.Seek(offset, SeekOrigin.Begin);
            int count=_fileStream.Read(bytes, 0, fragmentLength);
            return count;
        }

        public void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Dispose();
            }
        }
    }
}
