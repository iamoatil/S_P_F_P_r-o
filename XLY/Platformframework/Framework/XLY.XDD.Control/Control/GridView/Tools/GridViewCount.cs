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
    /// 列表统计记录总数
    /// </summary>
    public class GridViewCount : System.Windows.Controls.Control, IGridViewTool
    {
        static GridViewCount()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridViewCount), new FrameworkPropertyMetadata(typeof(GridViewCount)));
        }

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
            DependencyProperty.Register("GridView", typeof(XDD.Control.GridView), typeof(GridViewCount), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                GridViewCount count = s as GridViewCount;
                GridView grid = e.NewValue as GridView;
                if (grid != null)
                {
                    grid.grid.ItemsSourceChanged -= new DevExpress.Xpf.Grid.ItemsSourceChangedEventHandler(count.grid_ItemsSourceChanged);
                    grid.grid.ItemsSourceChanged += new DevExpress.Xpf.Grid.ItemsSourceChangedEventHandler(count.grid_ItemsSourceChanged);
                }
            })));

        private void grid_ItemsSourceChanged(object sender, DevExpress.Xpf.Grid.ItemsSourceChangedEventArgs e)
        {
            if (e.NewItemsSource is IList)
            {
                this.Count = ((IList)e.NewItemsSource).Count;
                return;
            }
            if (e.NewItemsSource is IEnumerable)
            {
                int count = 0;
                foreach (object i in (IEnumerable)e.NewItemsSource)
                {
                    ++count;
                }
                this.Count = count;
            }
        }

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
            DependencyProperty.Register("TreeView", typeof(TreeView), typeof(GridViewCount), new UIPropertyMetadata(null, new PropertyChangedCallback((s, e) =>
            {
                GridViewCount count = s as GridViewCount;
                TreeView tree = e.NewValue as TreeView;
            })));

        #endregion

        #region Count -- 统计记录

        /// <summary>
        /// 统计记录
        /// </summary>
        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(GridViewCount), new UIPropertyMetadata(0));

        #endregion
    }
}
