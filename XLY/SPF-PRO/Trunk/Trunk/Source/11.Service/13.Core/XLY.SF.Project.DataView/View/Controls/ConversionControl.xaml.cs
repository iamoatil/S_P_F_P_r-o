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
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataView
{
    /// <summary>
    /// ConversionControl.xaml 的交互逻辑
    /// </summary>
    public partial class ConversionControl : UserControl
    {
        public ConversionControl()
        {
            InitializeComponent();
        }
        public event DelgateDataViewSelectedItemChanged OnSelectedDataChanged;
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectedDataChanged?.Invoke(lsb1.SelectedValue);
        }
    }

    public class ConversionTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var fe = container as FrameworkElement;
            var message = item as MessageCore;
            DataTemplate dt = null;
            if (message != null && fe != null)
            {
                if (message.SendState == EnumSendState.Send)
                {
                    if (message.Type == EnumColumnType.String)
                    {
                        dt = fe.FindResource("left_txt") as DataTemplate;
                    }
                    else if (message.Type == EnumColumnType.Image)
                    {
                        dt = fe.FindResource("left_txt") as DataTemplate;
                    }
                }
                else if (message.SendState == EnumSendState.Receive)
                {
                    if (message.Type == EnumColumnType.String)
                    {
                        dt = fe.FindResource("right_txt") as DataTemplate;
                    }
                    else if (message.Type == EnumColumnType.Image)
                    {
                        dt = fe.FindResource("right_txt") as DataTemplate;
                    }
                }
            }
            return dt;
        }
    }

    public class ConversionStyleSelector: StyleSelector
    {
        public Style StyleLeft { get; set; }
        public Style StyleRight { get; set; }
        public override Style SelectStyle(object item, DependencyObject container)
        {
            Style re = null;
            var fe = container as FrameworkElement;
            var message = item as MessageCore;
            if (message != null && fe != null)
            {
                if (message.SendState == EnumSendState.Send)
                {
                    return StyleLeft;
                }
                else if (message.SendState == EnumSendState.Receive)
                {
                    return StyleRight;
                }
            }
            return re;
        }
    }
}
