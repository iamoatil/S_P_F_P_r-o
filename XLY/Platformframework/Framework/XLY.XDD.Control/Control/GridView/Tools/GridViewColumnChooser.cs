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
    /// 列选择器
    /// </summary>
    public class GridViewColumnChooser : System.Windows.Controls.Control, IGridViewTool
    {
        static GridViewColumnChooser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridViewColumnChooser), new FrameworkPropertyMetadata(typeof(GridViewColumnChooser)));
        }

        #region ItemsSource -- 列数据源

        /// <summary>
        /// 列数据源
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(GridViewColumnChooser), new UIPropertyMetadata(null));

        #endregion

        #region GridView -- 列表视图

        /// <summary>
        /// 列表视图
        /// </summary>
        public XDD.Control.GridView GridView
        {
            get { return (XDD.Control.GridView)GetValue(GridViewProperty); }
            set { SetValue(GridViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridViewProperty =
            DependencyProperty.Register("GridView", typeof(XDD.Control.GridView), typeof(GridViewColumnChooser), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                GridViewColumnChooser chooser = s as GridViewColumnChooser;
                XDD.Control.GridView grid = e.NewValue as XDD.Control.GridView;
                if (grid == null)
                {
                    chooser.ItemsSource = null;
                    return;
                }
                Binding binding = new Binding();
                binding.Source = grid;
                binding.Path = new PropertyPath("ColumnsSource");
                chooser.SetBinding(GridView.ItemsSourceProperty, binding);
            })));

        #endregion

        #region TreeView -- 树视图

        /// <summary>
        /// 树视图
        /// </summary>
        public TreeView TreeView
        {
            get { return (TreeView)GetValue(TreeViewProperty); }
            set { SetValue(TreeViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TreeView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TreeViewProperty =
            DependencyProperty.Register("TreeView", typeof(TreeView), typeof(GridViewColumnChooser), new UIPropertyMetadata(null));

        #endregion
    }
}
