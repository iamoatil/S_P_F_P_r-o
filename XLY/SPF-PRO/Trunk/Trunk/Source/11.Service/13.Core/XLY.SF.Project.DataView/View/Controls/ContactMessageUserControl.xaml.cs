using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataView
{
    /// <summary>
    /// ContactMessageUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ContactMessageUserControl : UserControl
    {
        public ContactMessageUserControl()
        {
            InitializeComponent();
        }

        public event DelgateDataViewSelectedItemChanged OnSelectedDataChanged;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void lsb1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WeChatFriendShow accout = lsb1.SelectedValue as WeChatFriendShow;
            TreeNode nodes = lsb1.DataContext as TreeNode;
            if (accout == null || nodes == null)
            {
                return ;
            }
            var selNode = nodes.TreeNodes.FirstOrDefault(t => t.Text == accout.Nick);
            var views = DataViewPluginAdapter.Instance.GetView("微信", selNode.Type);
            tbdetail.Items.Clear();
            foreach (var v in views)
            {
                v.SelectedDataChanged += OnSelectedDataChanged;
                TabItem ti = new TabItem() { Header = v.PluginInfo.Name };
                ti.Content = v.GetControl(new DataViewPluginArgument() { CurrentNode = selNode });
                tbdetail.Items.Add(ti);
            }

            OnSelectedDataChanged?.Invoke(lsb1.SelectedValue);
        }
    }

    public class TreeNodeSelectMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            WeChatFriendShow accout = values[0] as WeChatFriendShow;
            TreeNode nodes = values[1] as TreeNode;
            if(accout == null || nodes == null)
            {
                return null;
            }
            return nodes.TreeNodes.FirstOrDefault(t => t.Text == accout.Nick);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
