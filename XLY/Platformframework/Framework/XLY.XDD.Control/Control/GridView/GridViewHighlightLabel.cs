using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 高亮标签扩展，使用该标签将支持GridView的SearchText属性
    /// </summary>
    public class GridViewHighlightLabel : XlyHighlightLabel
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DevExpress.Xpf.Grid.EditGridCellData data = this.DataContext as DevExpress.Xpf.Grid.EditGridCellData;
            if (data != null)
            {
                System.Windows.Controls.Control control = null;
                control = data.View.GetAncestorByType<GridView>();
                if (control == null)
                {
                    //control = data.View.GetAncestorByType<TreeView>();
                }
                if (control != null)
                {
                    Binding b = new Binding();
                    b.Source = control;
                    b.Path = new System.Windows.PropertyPath("SearchText");
                    this.SetBinding(GridViewHighlightLabel.HighlightTextProperty, b);
                }
            }
        }
    }
}
