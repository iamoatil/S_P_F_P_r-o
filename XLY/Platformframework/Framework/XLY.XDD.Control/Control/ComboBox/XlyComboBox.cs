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
using System.Windows.Controls.Primitives;
using System.Collections;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 高级复选框
    /// </summary>
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class XlyComboBox : System.Windows.Controls.ComboBox
    {
        static XlyComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XlyComboBox), new FrameworkPropertyMetadata(typeof(XlyComboBox)));
        }

        #region UpdateText -- 文本内容更新方式

        /// <summary>
        /// 文本内容更新方式
        /// </summary>
        public IUpdateXlyComboBoxText UpdateText
        {
            get { return (IUpdateXlyComboBoxText)GetValue(UpdateTextProperty); }
            set { SetValue(UpdateTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpdateText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateTextProperty =
            DependencyProperty.Register("UpdateText", typeof(IUpdateXlyComboBoxText), typeof(XlyComboBox), new UIPropertyMetadata(new DefaultUpdateXlyComboBoxText()));

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_TextBox = this.Template.FindName("PART_TextBox", this) as TextBox;
        }

        private object _Old;

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (this.PART_TextBox == null)
                this.ApplyTemplate();
            if(this.PART_TextBox==null)
                return;
            
            string temp = this.PART_TextBox.Text;

            base.OnSelectionChanged(e);

            if (this.PART_TextBox == null || e.RemovedItems.Count > 0 && e.AddedItems.Count == 0)
                return;

            if (this.UpdateText != null)
                this.Text = this.UpdateText.Update(temp, _Old, this.SelectedValue);
            else
                this.Text = this.SelectedValue.ToSafeString();
            _Old = this.SelectedValue;
            if (this.PART_TextBox != null && this.PART_TextBox.Text != null)
                this.PART_TextBox.Select(this.PART_TextBox.Text.Length, 0);
        }

        #region PART

        private TextBox PART_TextBox;

        #endregion
    }
}
