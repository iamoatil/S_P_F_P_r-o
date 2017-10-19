using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 资源管理器筛选项
    /// </summary>
    public class ResourceBrowserFilterItem
    {
        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 显示标签
        /// </summary>
        public string DisplayLabel { get; set; }

        /// <summary>
        /// 筛选项
        /// </summary>
        public List<string> Filters { get; set; }
    }
}
