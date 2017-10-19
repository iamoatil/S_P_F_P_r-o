using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文本读取参数
    /// </summary>
    public class TextViewerEventArgs : EventArgs
    {
        /// <summary>
        /// 是否读取成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 读取的内容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 要读取的文本路径
        /// </summary>
        public string Path { get; set; }
    }
}
