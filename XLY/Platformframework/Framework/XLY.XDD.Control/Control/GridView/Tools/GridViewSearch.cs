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
    /// 列表搜索控件
    /// </summary>
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    public class GridViewSearch : System.Windows.Controls.Control, IGridViewTool
    {
        static GridViewSearch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridViewSearch), new FrameworkPropertyMetadata(typeof(GridViewSearch)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_TextBox = this.Template.FindName("PART_TextBox", this) as TextBox;
            this.PART_TextBox.PreviewKeyUp -= new KeyEventHandler(PART_TextBox_PreviewKeyUp);
            this.PART_TextBox.PreviewKeyUp += new KeyEventHandler(PART_TextBox_PreviewKeyUp);
            this.PART_TextBox.LostFocus -= new RoutedEventHandler(PART_TextBox_LostFocus);
            this.PART_TextBox.LostFocus += new RoutedEventHandler(PART_TextBox_LostFocus);
            this.PART_Button = this.Template.FindName("PART_Button", this) as Button;
            this.PART_Button.Click -= new RoutedEventHandler(PART_Button_Click);
            this.PART_Button.Click += new RoutedEventHandler(PART_Button_Click);
        }

        private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.GridView != null)
            {
                this.GridView.SearchText = this.PART_TextBox.Text;
            }
            if (this.TreeView != null)
            {

            }
        }

        private void PART_TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (this.GridView != null)
                {
                    this.GridView.SearchText = this.PART_TextBox.Text;
                }
                if (this.TreeView != null)
                {

                }
            }
        }

        private void PART_Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.GridView != null)
            {
                this.GridView.SearchText = this.PART_TextBox.Text;
            }
            if (this.TreeView != null)
            {

            }
        }

        #region PART

        private TextBox PART_TextBox;
        private Button PART_Button;

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
            DependencyProperty.Register("GridView", typeof(XDD.Control.GridView), typeof(GridViewSearch), new UIPropertyMetadata(null));

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
            DependencyProperty.Register("TreeView", typeof(TreeView), typeof(GridViewSearch), new UIPropertyMetadata(null));

        #endregion
    }
}
