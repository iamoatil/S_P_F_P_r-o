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

namespace XLY.XDD.Control.Control
{
    /// <summary>
    /// 文件选择对话框
    /// </summary>
    [TemplatePart(Name = "PART_TreeView", Type = typeof(TreeView))]
    [TemplatePart(Name = "PART_GridView", Type = typeof(GridView))]
    [TemplatePart(Name = "PART_Enter", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Cancel", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Create", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Delete", Type = typeof(Button))]
    public class XlyFileDialog : System.Windows.Controls.Control
    {
        static XlyFileDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyFileDialog), new FrameworkPropertyMetadata(typeof(XlyFileDialog)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_TreeView = this.Template.FindName("PART_TreeView", this) as TreeView;
            this.PART_GridView = this.Template.FindName("PART_GridView", this) as GridView;
            this.PART_Enter = this.Template.FindName("PART_Enter", this) as Button;
            this.PART_Cancel = this.Template.FindName("PART_Cancel", this) as Button;
            this.PART_Create = this.Template.FindName("PART_Create", this) as Button;
            this.PART_Cancel = this.Template.FindName("PART_Cancel", this) as Button;

            
        }

        #region PART

        private TreeView PART_TreeView;
        private GridView PART_GridView;
        private Button PART_Create;
        private Button PART_Delete;
        private Button PART_Enter;
        private Button PART_Cancel;

        #endregion


    }
}
