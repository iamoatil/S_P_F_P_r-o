using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Main
{
    [Export(ExportKeys.HomePageViewModel, typeof(ViewModelBase))]
    public class HomePageViewModel : ViewModelBase
    {
        #region Constructors

        public HomePageViewModel()
        {
            CreateCaseCommand = new ProxyRelayCommand(ExecuteCreateCaseCommand);
            OpenAllCaseCommand = new ProxyRelayCommand(ExecuteOpenAllCaseCommand);
            OpenLocalCaseCommand = new ProxyRelayCommand(ExecuteOpenLocalCaseCommand);
        }

        #endregion

        #region Commands

        /// <summary>
        /// 新建案例
        /// </summary>
        public ProxyRelayCommand CreateCaseCommand { get; set; }
        /// <summary>
        /// 打开所有案例
        /// </summary>
        public ProxyRelayCommand OpenAllCaseCommand { get; set; }
        /// <summary>
        /// 打开所有案例
        /// </summary>
        public ProxyRelayCommand OpenLocalCaseCommand { get; set; }

        #endregion

        public override void ViewClosed()
        {

        }

        #region ExcuteCommands

        private string ExecuteCreateCaseCommand()
        {
            base.NavigationForMainWindow(ExportKeys.CaseCreationView);
            //通知主界面可以打开创建案例界面
            GeneralArgs args = new GeneralArgs(GeneralKeys.AllowShowCaseName);
            base.MessageAggregation.SendGeneralMsg(args);

            return "新建案例";
        }
        private string ExecuteOpenAllCaseCommand()
        {
            base.NavigationForNewDislogWindow(ExportKeys.CaseListView);

            GeneralArgs args = new GeneralArgs(GeneralKeys.AllowShowDeviceList);
            base.MessageAggregation.SendGeneralMsg(args);

            return "打开所有案例";
        }

        private string ExecuteOpenLocalCaseCommand()
        {
            return "打开本地案例";
        }

        #endregion
    }
}
