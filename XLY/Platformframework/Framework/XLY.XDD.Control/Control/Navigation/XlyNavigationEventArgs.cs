using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 导航事件参数
    /// </summary>
    public class XlyNavigationEventArgs : EventArgs
    {
        /// <summary>
        /// 导航配置
        /// </summary>
        public XlyNavigationConfig Config { get; set; }
    }
}
