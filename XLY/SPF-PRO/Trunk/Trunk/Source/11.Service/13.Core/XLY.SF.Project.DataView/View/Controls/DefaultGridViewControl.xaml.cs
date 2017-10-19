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

namespace XLY.SF.Project.DataView.View.Controls
{
    /// <summary>
    /// DefaultGridViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class DefaultGridViewControl : UserControl
    {
        public DefaultGridViewControl()
        {
            InitializeComponent();
        }

        public event DelgateDataViewSelectedItemChanged OnSelectedDataChanged;

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectedDataChanged?.Invoke((sender as DataGrid).SelectedItem);
        }
    }
}
