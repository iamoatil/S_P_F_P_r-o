using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 列表工具条
    /// </summary>
    public interface IGridViewTool
    {
        /// <summary>
        /// 关联的列表视图
        /// </summary>
        GridView GridView { get; set; }

        /// <summary>
        /// 关联的树视图
        /// </summary>
        TreeView TreeView { get; set; }
    }
}
