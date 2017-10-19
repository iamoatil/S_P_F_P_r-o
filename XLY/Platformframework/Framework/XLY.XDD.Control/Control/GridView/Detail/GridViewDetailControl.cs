using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 列表详细信息控件
    /// </summary>
    public class GridViewDetailControl : System.Windows.Controls.Control
    {
        static GridViewDetailControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridViewDetailControl), new FrameworkPropertyMetadata(typeof(GridViewDetailControl)));
        }

        #region ItemsSource -- 数据源

        /// <summary>
        /// 数据源
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(GridViewDetailControl), new UIPropertyMetadata(null));

        #endregion
    }
}
