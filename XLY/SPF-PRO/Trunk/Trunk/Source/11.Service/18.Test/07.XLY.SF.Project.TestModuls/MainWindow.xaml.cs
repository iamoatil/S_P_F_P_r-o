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
using XLY.SF.Framework.Language;

namespace _07.XLY.SF.Project.TestModuls
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            LanguageHelperSingle.Instance.SwitchLanguage();
            var a = LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.DeviceLanguage_DeviceUseableValidation_InUse);
        }
    }
}
