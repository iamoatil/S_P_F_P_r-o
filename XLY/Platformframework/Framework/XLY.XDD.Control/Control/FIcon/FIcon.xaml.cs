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
    /// 字体图标组件，设置Text为字符编码，即可显示对应图标字符。使用字体查看软件（或XLY.SF.WPFTest-字体图标）可以查看字符编码
    /// xaml实例：Text="&#61904;"  Text="&#xf115;"
    /// c#示例：Text=""\uf000""
    /// </summary>
    public partial class FIcon : TextBlock
    {
       static  FIcon()
       {
           DefaultStyleKeyProperty.OverrideMetadata(typeof(FIcon), new FrameworkPropertyMetadata(typeof(FIcon)));
       }
    }
}
