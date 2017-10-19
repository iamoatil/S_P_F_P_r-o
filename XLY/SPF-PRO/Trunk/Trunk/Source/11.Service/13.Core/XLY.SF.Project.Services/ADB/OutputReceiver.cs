using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Services.ADB
{
    /// <summary>
    /// 返回数据接收器的抽象基类
    /// </summary>
    public abstract class AbstractOutputReceiver
    {
        protected const String NEWLINE = "\r\n";
        protected String UnfinishedLine { get; set; }

        /// <summary>
        /// 接收的行数据集合，换行分割
        /// </summary>
        public List<string> Lines { get; private set; }

        /// <summary>
        /// 接收的数据
        /// </summary>
        public string Data { get; internal set; }

        #region AbstractOutputReceiver-构造函数（初始化）

        /// <summary>
        ///  AbstractOutputReceiver-构造函数（初始化）
        /// </summary>
        public AbstractOutputReceiver()
        {
            this.Lines = new List<string>();
        }

        #endregion

        private StringBuilder stringBuilder = new StringBuilder();

        /// <summary>
        /// 添加数据流,add data to the reciver
        /// </summary>
        public virtual void Add(byte[] data, int offset = 0, int length = -1)
        {
            length = length < 0 ? data.Length : length;
            string s = null;
            try
            {
                s = Encoding.UTF8.GetString(data, offset, length);
            }
            catch (DecoderFallbackException)
            {
                s = Encoding.Default.GetString(data, offset, length);
            }

            //*  此处有一个非常诡异的Bug,休息一会儿它，自己就好了 */
            System.Threading.Thread.Sleep(15);

            if (String.IsNullOrWhiteSpace(s)) return;
            this.stringBuilder.Append(s);
        }

        /// <summary>
        /// 完成输出
        /// Flushes the output.
        /// </summary>
        public virtual void Flush()
        {
            this.Data = this.stringBuilder.ToString();
            this.stringBuilder.Clear();
            //now we split the data
            this.Lines = this.Data.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (IsValid(this.Lines))
            {
                this.DoResolver();
            }
        }

        /// <summary>
        /// 执行数据解析
        /// </summary>
        public virtual void DoResolver()
        {
        }
        public  bool IsValid<T>(IEnumerable<T> source)
        {
            return source != null && source.Any();
        }
    }
}
