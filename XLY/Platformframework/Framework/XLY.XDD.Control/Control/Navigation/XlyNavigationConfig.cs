using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 导航控件配置信息
    /// </summary>
    public class XlyNavigationConfig
    {
        #region NavigationKey -- 导航键值

        private string _NavigationKey;
        /// <summary>
        /// 导航键值
        /// </summary>
        public string NavigationKey
        {
            get { return _NavigationKey; }
            set
            {
                if (value == null || value.Trim().Equals(string.Empty))
                    throw new ArgumentNullException();
                _NavigationKey = value;
            }
        }

        #endregion

        #region View -- 视图

        /// <summary>
        /// 视图类型
        /// </summary>
        public Type ViewType { get; set; }

        private FrameworkElement _View;

        /// <summary>
        /// 视图
        /// </summary>
        public FrameworkElement View
        {
            set { this._View = value; }
            get { return this._View; }
        }

        #endregion

        #region ViewModel -- 视图模型

        /// <summary>
        /// 视图模型
        /// </summary>
        public object ViewModel { get; set; }

        /// <summary>
        /// 视图模型类型
        /// </summary>
        public Type ViewModelType { get; set; }

        #endregion

        /// <summary>
        /// 导航参数
        /// </summary>
        public object NavigationArgs { get; set; }

        /// <summary>
        /// 导航之后的回掉
        /// </summary>
        internal Action<FrameworkElement> OnLoad { get; set; }

        /// <summary>
        /// 创建视图模型的方法
        /// </summary>
        public Func<XlyNavigationConfig, object> FunCreateViewModel;

        /// <summary>
        /// 创建视图的方法
        /// </summary>
        public Func<XlyNavigationConfig, FrameworkElement> FunCreateView;
    }
}
