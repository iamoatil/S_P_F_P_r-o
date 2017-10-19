using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 更新滚动条参数
    /// </summary>
    public class UpdateVScrollEventArgs : EventArgs
    {
        public UpdateVScrollEventArgs()
        {
            this.IsEnabled = true;
        }

        /// <summary>
        /// 最小值
        /// </summary>
        public double? Minimum { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public double? Maximum { get; set; }

        /// <summary>
        /// 当前值
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        /// 控件是否可用
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
