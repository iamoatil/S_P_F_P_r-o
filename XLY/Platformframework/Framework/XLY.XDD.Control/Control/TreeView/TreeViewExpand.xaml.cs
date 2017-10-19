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
using DevExpress.Xpf.Grid;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 树展开按钮
    /// </summary>
    public class TreeViewExpand : System.Windows.Controls.Control
    {
        static TreeViewExpand()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewExpand), new FrameworkPropertyMetadata(typeof(TreeViewExpand)));
        }

        public TreeViewExpand()
        {
            this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TreeViewExpand_PreviewMouseLeftButtonDown);
        }

        #region IsExpand -- 是否展开

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpand
        {
            get { return (bool)GetValue(IsExpandProperty); }
            set { SetValue(IsExpandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandProperty =
            DependencyProperty.Register("IsExpand", typeof(bool), typeof(TreeViewExpand), new UIPropertyMetadata(false));

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DevExpress.Xpf.Grid.EditGridCellData data = this.DataContext as DevExpress.Xpf.Grid.EditGridCellData;

            data.ContentChanged -= new EventHandler(data_ContentChanged);
            data.ContentChanged += new EventHandler(data_ContentChanged);

            if (this.treeListView == null)
                this.treeListView = data.View as DevExpress.Xpf.Grid.TreeListView;
            if (this.treeView == null)
                this.treeView = this.treeListView.GetAncestorByType<TreeView>();
            this._CheckItems(data.RowData.Row);
        }

        private DevExpress.Xpf.Grid.TreeListView treeListView;
        private TreeView treeView;

        private void data_ContentChanged(object sender, EventArgs e)
        {
            DevExpress.Xpf.Grid.EditGridCellData data = this.DataContext as DevExpress.Xpf.Grid.EditGridCellData;
            TreeListNode node = this.treeListView.GetNodeByContent(data.RowData.Row);
            this.IsExpand = node.IsExpanded;
            this._CheckItems(data.RowData.Row);
        }

        private void _CheckItems(object row)
        {
            if (!this.treeView.IsHideCustormExpandWhenHasNoetChildren)
                return;
            if (this.treeView.ChildNodesSelector != null)
            {
                System.Collections.IEnumerable items = this.treeView.ChildNodesSelector.SelectChildren(row);
                if (items == null)
                {
                    this.Visibility = System.Windows.Visibility.Hidden;
                    return;
                }
                bool hasItems = false;
                foreach (var i in items)
                {
                    hasItems = true;
                    break;
                }
                if (!hasItems)
                {
                    this.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    this.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void TreeViewExpand_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DevExpress.Xpf.Grid.EditGridCellData data = this.DataContext as DevExpress.Xpf.Grid.EditGridCellData;
            if (this.IsExpand)
            {
                (data.RowData.View as DevExpress.Xpf.Grid.TreeListView).CollapseNode(data.RowData.RowHandle.Value);
                this.IsExpand = false;
            }
            else
            {
                (data.RowData.View as DevExpress.Xpf.Grid.TreeListView).ExpandNode(data.RowData.RowHandle.Value);
                this.IsExpand = true;
            }
            e.Handled = true;
        }
    }
}
