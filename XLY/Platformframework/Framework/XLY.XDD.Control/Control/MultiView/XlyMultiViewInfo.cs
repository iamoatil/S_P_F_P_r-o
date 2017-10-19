using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 多视图信息
    /// </summary>
    public class XlyMultiViewInfo
    {
        /// <summary>
        /// 页签图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 导航键值
        /// </summary>
        internal string NavigationKey { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 视图类型
        /// </summary>
        public Type ViewType { get; set; }

        /// <summary>
        /// 视图样式
        /// </summary>
        public Style ViewStyle { get; set; }
    }
}
