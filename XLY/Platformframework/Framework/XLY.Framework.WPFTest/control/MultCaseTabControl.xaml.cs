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
using XLY.Framework.Controls;

namespace XLY.Framework.WPFTest
{
    /// <summary>
    /// MultCaseTabControl.xaml 的交互逻辑
    /// </summary>
    public partial class MultCaseTabControl : UserControl
    {
        public MultCaseTabControl()
        {
            InitializeComponent();
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as FrameworkElement;
            var tab = btn.TemplatedParent as TabItem;
            if (MessageBoxX.Question(string.Format("你确定要关闭[{0}]吗？", tab.Header.ToString())))
            {
                this.tabControl.Items.Remove(tab);
                e.Handled = true;
                return;
            }
            e.Handled = true;
        }
    }
}
