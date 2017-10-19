using System;

namespace XLY.SF.Project.Views.PreviewFile.LargeFileLoad
{
    /// <summary>
    /// 内存文件
    /// 目的：解决在TextBox中显示大文件卡的问题。
    /// 方法：每次从本地文件只读取MemoryFile大小的数据作为缓存
    /// </summary>
    public class CacheBuffer
    {
        public CacheBuffer(LargeFile largeFile)
        {
            _largeFile = largeFile;
        }

        private readonly LargeFile _largeFile;

        /// <summary>
        /// 当前缓存的数据
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// _buffer中的有效长度
        /// </summary>
        private int _bufferValidLength = 0;

        /// <summary>
        /// 段的长度。段：从大文件中选取的一个小段，每次只读取这个小段的内容，然后显示。
        /// </summary>
        public const int FragmentLength = 1024*2024;

        /// <summary>
        /// 此Buffer开始的偏置位置
        /// </summary>
        public int StartedPos { get; private set; }

        /// <summary>
        /// 装载Buffer中的数据。用指定位置的数据来设置Buffer
        /// </summary>
        public void LoadBuffer(int offset)
        {
            _buffer = new byte[FragmentLength];
            _bufferValidLength = _largeFile.Read(ref offset, _buffer);
            StartedPos = offset;
        }

        /// <summary>
        /// 读取指定位置的数据
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="bytes">存放读取的内容的字节数组</param>
        /// <returns>读取的字节个数</returns>
        public int Read(int offset, byte[] bytes)
        {
            int bytesLen = bytes.Length;
           
            //读取的位置不在Buffer范围内，就重新装载一下数据
            if (offset + bytesLen > StartedPos + _bufferValidLength
                || offset < StartedPos)
            {
                LoadBuffer(offset - FragmentLength/2);
            }

            //装载Buffer中的数据
            if (offset >= StartedPos)
            {
                if (offset + bytesLen <= StartedPos + _bufferValidLength)
                {
                    Array.Copy(_buffer, offset - StartedPos, bytes, 0, bytesLen);
                }
                else
                {
                    bytesLen = StartedPos + _bufferValidLength - offset;
                    Array.Copy(_buffer, offset - StartedPos, bytes, 0, bytesLen);
                }
                return bytesLen;
            }
            
            return 0;
        }
    }
}
