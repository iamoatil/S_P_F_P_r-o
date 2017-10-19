using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/7 11:01:08
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.ViewModels.Test
{
    [Export(ExportKeys.ModuleTestViewModel1, typeof(ViewModelBase))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class TestViewModel1 : ViewModelBase
    {
        public TestViewModel1()
        {

        }

        public ProxyRelayCommand CloseMeCommand { get; set; }

        protected override void InitViewModel()
        {
            CloseMeCommand = new ProxyRelayCommand(ExecuteCloseMeCommand, base.GetViewContainer, null);
        }
        public override void LoadViewModel(object parameters)
        {
            base.LoadViewModel(parameters);
        }

        public override void ViewClosed()
        {

        }

        private string ExecuteCloseMeCommand()
        {

            return "测试操作日志";
        }
    }
}
