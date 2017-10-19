using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 行数据事件参数
    /// </summary>
    public class GridViewRowEventArgs : EventArgs
    {
        /// <summary>
        /// 行数据
        /// </summary>
        public object RowData { get; set; }

        /// <summary>
        /// 所属列表
        /// </summary>
        public GridView GridView { get; set; }
    }
}
