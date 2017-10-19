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
    /// 列表为序控件
    /// </summary>
    public class GridViewIndex : System.Windows.Controls.Control
    {
        static GridViewIndex()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridViewIndex), new FrameworkPropertyMetadata(typeof(GridViewIndex)));
        }

        #region RowIndex -- 行号

        /// <summary>
        /// 行号
        /// </summary>
        public string RowIndex
        {
            get { return (string)GetValue(RowIndexProperty); }
            set { SetValue(RowIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowIndexProperty =
            DependencyProperty.Register("RowIndex", typeof(string), typeof(GridViewIndex), new UIPropertyMetadata(null));

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DevExpress.Xpf.Grid.EditGridCellData data = this.DataContext as DevExpress.Xpf.Grid.EditGridCellData;
            if (data != null)
            {
                data.ContentChanged -= new EventHandler(data_ContentChanged);
                data.ContentChanged += new EventHandler(data_ContentChanged);
                this.RowIndex = (data.RowData.ControllerVisibleIndex + 1).ToString();
            }
        }

        private void data_ContentChanged(object sender, EventArgs e)
        {
            DevExpress.Xpf.Grid.EditGridCellData data = this.DataContext as DevExpress.Xpf.Grid.EditGridCellData;
            if (data != null)
            {
                this.RowIndex = (data.RowData.ControllerVisibleIndex + 1).ToString();
            }
        }
    }
}
