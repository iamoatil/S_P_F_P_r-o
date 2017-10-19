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

namespace XLY.XDD.Control
{
    /// <summary>
    /// 树视图缩进扩展按钮
    /// </summary>
    public class TreeViewIndent : System.Windows.Controls.Control
    {
        static TreeViewIndent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewIndent), new FrameworkPropertyMetadata(typeof(TreeViewIndent)));
        }

        #region RowIndent -- 行缩进值

        /// <summary>
        /// 行缩进值
        /// </summary>
        public double RowIndent
        {
            get { return (double)GetValue(RowIndentProperty); }
            set { SetValue(RowIndentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowIndent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowIndentProperty =
            DependencyProperty.Register("RowIndent", typeof(double), typeof(TreeViewIndent), new UIPropertyMetadata(20d));

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DevExpress.Xpf.Grid.EditGridCellData data = this.DataContext as DevExpress.Xpf.Grid.EditGridCellData;

            data.ContentChanged -= new EventHandler(data_ContentChanged);
            data.ContentChanged += new EventHandler(data_ContentChanged);

            this.updateWidth(data);
        }

        private void updateWidth(DevExpress.Xpf.Grid.EditGridCellData data)
        {
            this.Width = data.RowData.Level * this.RowIndent;
            //(data.RowData.View as DevExpress.Xpf.Grid.TreeListView).ExpandNode(data.RowData.RowHandle.Value);
        }

        private void data_ContentChanged(object sender, EventArgs e)
        {
            DevExpress.Xpf.Grid.EditGridCellData data = this.DataContext as DevExpress.Xpf.Grid.EditGridCellData;

            this.updateWidth(data);
        }
    }
}
