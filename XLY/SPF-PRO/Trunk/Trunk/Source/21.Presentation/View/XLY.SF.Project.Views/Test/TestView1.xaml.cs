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
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Test
{
    /// <summary>
    /// TestView1.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.ModuleTestView1, typeof(UcViewBase))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class TestView1 : UcViewBase
    {
        public TestView1()
        {
            InitializeComponent();
        }

        [Import(ExportKeys.ModuleTestViewModel1, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return this.DataContext as ViewModelBase;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}
