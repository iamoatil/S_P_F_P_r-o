using System;
using System.Collections.Generic;
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
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.DataView.Preview.Controls
{
    /// <summary>
    /// DefaultPreviewControl.xaml 的交互逻辑
    /// </summary>
    public partial class DefaultPreviewControl : UserControl
    {
        public DefaultPreviewControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.DataContext == null)
            {
                return;
            }
            var obj = this.DataContext;
            int row = 0;
            foreach(var pro in obj.GetType().GetProperties())
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });

                TextBlock tb = new TextBlock() { Text = pro.Name + " :" };
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Right;
                tb.Margin = new Thickness(3);
                grid.Children.Add(tb);
                tb.SetValue(Grid.RowProperty, row);

                TextBlock tb2 = new TextBlock() { Text = pro.GetValue(obj).ToSafeString() };
                tb2.VerticalAlignment = VerticalAlignment.Center;
                tb2.HorizontalAlignment = HorizontalAlignment.Left;
                tb2.Margin = new Thickness(3);
                grid.Children.Add(tb2);
                tb2.SetValue(Grid.RowProperty, row);
                tb2.SetValue(Grid.ColumnProperty, 1);
                row++;
            }
        }
    }
}
