using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 导航视图模型
    /// </summary>
    public class XlyNavigationViewModelBase : ViewModelBase
    {
        /// <summary>
        /// 所属导航控件
        /// </summary>
        public XlyNavigation Navigation { get; set; }

        /// <summary>
        /// 所属配置信息
        /// </summary>
        public XlyNavigationConfig Config { get; set; }
    }
}
