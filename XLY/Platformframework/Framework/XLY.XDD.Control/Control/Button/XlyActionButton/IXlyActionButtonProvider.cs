using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 激活按钮解析器
    /// </summary>
    public interface IXlyActionButtonProvider
    {
        /// <summary>
        /// 执行激活
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="args">参数</param>
        void Action(object type, object args);
    }
}
