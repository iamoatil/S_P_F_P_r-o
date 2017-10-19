using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Main
{
    /// <summary>
    /// HomePageView.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.HomePageView, typeof(UcViewBase))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class HomePageView : UcViewBase
    {
        public HomePageView()
        {
            InitializeComponent();
        }

        [Import(ExportKeys.HomePageViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return DataContext as ViewModelBase;
            }
            set
            {
                value.SetViewContainer(this);
                DataContext = value;
            }
        }
    }
}
