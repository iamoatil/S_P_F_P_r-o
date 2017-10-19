using System.Text;

/* ==============================================================================
* Description：PresentBuffer  
* Author     ：litao
* Create Date：2017/10/16 11:40:14
* ==============================================================================*/


namespace XLY.SF.Project.Views.PreviewFile.LargeFileLoad
{
    public class PresentBuffer
    {
        public PresentBuffer(CacheBuffer cacheBuffer)
        {
            _cacheBuffer = cacheBuffer;
        }

        private readonly CacheBuffer _cacheBuffer;

        private IdentifyEncoding identifyEncoding = new IdentifyEncoding();

        /// <summary>
        /// 当前显示内容的起始偏置
        /// </summary>
        public int StartedOffset { get; private set; }

        /// <summary>
        /// 当前显示内容的所代表的长度
        /// </summary>
        public int TotalLen { get; private set; }

        //todo 此处认为界面一次显示不了10kb的数据，若发生显示应该会出问题。
        public const int OneTimeRead = 10 * 1024;

        /// <summary>
        /// 界面显示的内容
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// 读取下一段数据
        /// </summary>
        public void ReadNext()
        {
            StartedOffset += OneTimeRead;
            Read(StartedOffset);
        }

        /// <summary>
        /// 读取上一段数据
        /// </summary>
        public void ReadLast()
        {
            StartedOffset -= OneTimeRead;
            if (StartedOffset  < 0)
            {
                StartedOffset = 0;
            }
            Read(StartedOffset);
        }


        /// <summary>
        /// 从指定位置开始读取数据,直到接收到停止消息。
        /// </summary>
        public void Read(int offset)
        {
            //下面这句话保证，每次都从OneTimeRead的倍数位置读取数据
            offset=offset / OneTimeRead * OneTimeRead;

            byte[] bytes = new byte[OneTimeRead];
            TotalLen = _cacheBuffer.Read(offset, bytes);
            StartedOffset = offset;
           
            string encodingName = identifyEncoding.GetEncodingName(IdentifyEncoding.ToSByteArray(bytes));
            string text = Encoding.GetEncoding(encodingName).GetString(bytes, 0, TotalLen);

            Text = text;
        }
    }
}
