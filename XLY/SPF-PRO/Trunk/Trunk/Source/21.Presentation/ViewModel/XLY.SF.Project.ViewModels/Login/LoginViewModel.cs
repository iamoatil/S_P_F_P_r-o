using GalaSoft.MvvmLight.Command;
using ProjectExtend.Context;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.SystemKeys;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 17:36:58
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.ViewModels.Login
{
    [Export(ExportKeys.ModuleLoginViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class LoginViewModel : ViewModelBase
    {
        #region Constructors

        [ImportingConstructor]
        public LoginViewModel(IMessageBox messageBox)
        {
            //_dbService = dbService;
            MessageBox = messageBox;
            LoginCommand = new ProxyRelayCommand(ExecuteLoginCommand, GetViewContainer, null);
            ExitSysCommand = new RelayCommand(ExeucteExitSysCommand);
            CurLoginUser = new UserInfoEntityModel() { LoginUserName = "admin", LoginPassword = "123456" };
        }

        #endregion

        #region Commands

        /// <summary>
        /// 登录
        /// </summary>
        public ProxyRelayCommand LoginCommand { get; set; }

        /// <summary>
        /// 退出程序
        /// </summary>
        public ICommand ExitSysCommand { get; set; }

        #endregion

        #region Model

        public UserInfoEntityModel CurLoginUser { get; set; }

        #endregion

        #region 数据定义

        /// <summary>
        /// 数据库服务
        /// </summary>
        [Import(typeof(IDatabaseContext))]
        private IDatabaseContext _dbService { get; set; }

        /// <summary>
        /// 消息服务
        /// </summary>
        //[Import(typeof(IMessageBox))]
        private IMessageBox MessageBox { get; set; }

        #endregion

        public override void ViewClosed()
        {

        }

        #region 登录操作

        private string ExecuteLoginCommand()
        {
            //UserInfoEntityModel s = new UserInfoEntityModel()
            //{
            //    IdNumber = "548795462165789548",
            //    LoginPassword = "123456",
            //    LoginUserName = "admin",
            //    PhoneNumber = "15448798546",
            //    UserName = "大锤",
            //    WorkUnit = "月上之上"
            //};
            //UserInfoEntityModel b = new UserInfoEntityModel()
            //{
            //    IdNumber = "548795462165789548",
            //    LoginPassword = "123456",
            //    LoginUserName = "hzl",
            //    PhoneNumber = "15448798546",
            //    UserName = "大锤2",
            //    WorkUnit = "月上之上2"
            //};
            //var a = _dbService.AddNew<UserInfoEntityModel>(s, b);

            string operationLog = string.Empty;
            var loginUser = _dbService.UserInfos.FirstOrDefault((t) => t.LoginUserName == CurLoginUser.LoginUserName).ToModel<UserInfo, UserInfoEntityModel>();
            if (loginUser == default(UserInfoEntityModel))
            {
                //登录失败
                MessageBox.ShowDialogErrorMsg("登录失败，请重新登录");
            }
            else
            {
                SystemContext.Instance.SetLoginSuccessUser(loginUser);

                //关闭界面
                //由于此处导航是持续导航（第一个界面完成后，直接进入下个界面，无导航消息发送）
                //所以需要自己关闭
                base.CloseView();
                operationLog = "登录用户成功";

                //登录成功
                SysCommonMsgArgs sysArgs = new SysCommonMsgArgs(SystemKeys.LoginComplete);
                MsgAggregation.Instance.SendSysMsg(sysArgs);
            }

            return operationLog;
        }

        #endregion

        #region 退出程序

        private void ExeucteExitSysCommand()
        {
            SysCommonMsgArgs args = new SysCommonMsgArgs(SystemKeys.ShutdownProgram);
            base.MessageAggregation.SendSysMsg(args);
        }

        #endregion
    }
}
