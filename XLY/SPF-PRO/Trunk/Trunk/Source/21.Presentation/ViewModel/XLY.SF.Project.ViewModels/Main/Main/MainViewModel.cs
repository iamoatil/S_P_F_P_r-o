using GalaSoft.MvvmLight.Command;
using ProjectExtend.Context;
using System;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.SystemKeys;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.VModel.Main;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 17:38:50
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.ViewModels.Main
{
    [Export(ExportKeys.ModuleMainViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MainViewModel : ViewModelBase
    {
        #region 属性

        #region private

        /// <summary>
        /// 数据库服务
        /// </summary>
        private IDatabaseContext _dbService;

        /// <summary>
        /// 消息服务
        /// </summary>
        private IMessageBox _messageBox { get; set; }

        #endregion

        /// <summary>
        /// 主界面导航管理器
        /// </summary>
        public MainNavigationManager MainNavigation { get; set; }

        #region Model

        private MainModel _mainInfo;
        /// <summary>
        /// 主界面展示信息
        /// </summary>
        public MainModel MainInfo
        {
            get
            {
                return this._mainInfo;
            }

            set
            {
                this._mainInfo = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// 结束程序
        /// </summary>
        public ProxyRelayCommand ShutdownProgramCommand { get; set; }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        public ProxyRelayCommand CloseCaseCommand { get; set; }

        #endregion

        #endregion

        #region Constructors

        [ImportingConstructor]
        public MainViewModel(IDatabaseContext dbService, IMessageBox messageBox)
        {
            _dbService = dbService;
            _messageBox = messageBox;

            MainNavigation = new MainNavigationManager();

            //事件注册
            ShutdownProgramCommand = new ProxyRelayCommand(ExecuteShutdownProgramCommand);
            CloseCaseCommand = new ProxyRelayCommand(ExecuteCloseCaseCommand);
        }

        #endregion

        #region 重载

        public override void ViewClosed()
        {

        }

        protected override void LoadCore(object parameters)
        {
            MainInfo = new MainModel()
            {
                CurUserName = SystemContext.Instance.CurUserInfo?.UserName,
                CurSysTime = DateTime.Now
            };
            //加载首页
            base.NavigationForMainWindow(ExportKeys.HomePageView);
            //加载子页面【创建案例】
            //NavigationArgs args = new NavigationArgs(ExportKeys.CaseCreationView);
            //GeneralArgs args2 = new GeneralArgs(GeneralKeys.NavigationForSubView);
            //args2.Parameters = args.View;
            //MsgAggregation.Instance.SendGeneralMsg(args2);
        }

        #endregion

        #region ExecuteCommand

        //关程序
        private string ExecuteShutdownProgramCommand()
        {
            SysCommonMsgArgs args = new SysCommonMsgArgs(SystemKeys.ShutdownProgram);
            base.MessageAggregation.SendSysMsg(args);
            return "关闭程序";
        }

        //关闭按钮
        private string ExecuteCloseCaseCommand()
        {


            return "关闭案例";
        }

        #endregion
    }
}
