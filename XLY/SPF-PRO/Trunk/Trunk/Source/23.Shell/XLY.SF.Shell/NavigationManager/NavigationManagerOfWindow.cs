using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.SystemKeys;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.Themes;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Shell.CommWindow;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 17:19:23
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Shell.NavigationManager
{
    public class NavigationManagerOfWindow
    {
        /// <summary>
        /// 当前打开的所有窗口
        /// </summary>
        private Dictionary<Guid, OpenWindowElement> _curOpenWindows;

        public NavigationManagerOfWindow()
        {
            _curOpenWindows = new Dictionary<Guid, OpenWindowElement>();
        }

        /// <summary>
        /// 注册导航消息
        /// </summary>
        public void RegisterNavigation()
        {
            //注册监听消息
            MsgAggregation.Instance.RegisterSysMsg(this, SystemKeys.LoginComplete, LoginSuccessCallback);

            //注册打开新窗口事件
            MsgAggregation.Instance.RegisterNaviagtionMsg(this, SystemKeys.OpenNewWindow, OpenNewWindowCallback);
            //注册打开新窗口事件
            MsgAggregation.Instance.RegisterNaviagtionMsg(this, SystemKeys.OpenNewDialogWindow, OpenNewDialogWindowCallback);
            //注册关闭窗口事件
            MsgAggregation.Instance.RegisterNaviagtionMsg(this, SystemKeys.CloseWindow, CloseWindowCallback);
        }

        #region 事件处理

        #region 显示新窗口

        //关闭窗口
        private void CloseWindowCallback(NavigationArgs args)
        {
            if (_curOpenWindows.ContainsKey(args.ViewModelID))
            {
                _curOpenWindows[args.ViewModelID].CurOpenWindow.Close();
            }
        }

        private void OpenNewDialogWindowCallback(NavigationArgs args)
        {
            var newWindow = CreateNewWindow(args.View, Application.Current.MainWindow);
            _curOpenWindows.Add(args.ViewModelID, new OpenWindowElement(newWindow));
            newWindow.ShowDialog();
        }

        //打开新窗口
        private void OpenNewWindowCallback(NavigationArgs args)
        {
            var newWindow = CreateNewWindow(args.View);
            _curOpenWindows.Add(args.ViewModelID, new OpenWindowElement(newWindow));
            newWindow.Show();
        }

        #endregion

        #region 登录成功

        private void LoginSuccessCallback(SysCommonMsgArgs args)
        {
            //注销登录界面消息监听
            MsgAggregation.Instance.UnRegisterMsg<SysCommonMsgArgs>(this, SystemKeys.LoginComplete, LoginSuccessCallback);

            //注册主界面，正式启动程序
            var view = IocManagerSingle.Instance.GetViewPart(ExportKeys.ModuleMainUcView);
            Application.Current.MainWindow.Content = view;
            view.DataSource.LoadViewModel(null);
            Application.Current.MainWindow.Show();
        }

        #endregion

        #region 执行程序初始化
        /* 由于初始化流程不便 
         * 此处独立处理
         * 等待加载页面关闭后（关闭动画完成）
         * 再进入登录界面
         */

        /// <summary>
        /// 执行初始化程序
        /// </summary>
        public void ExecuteProgramInitialise()
        {
            //加载界面
            var loadingView = IocManagerSingle.Instance.GetViewPart(ExportKeys.ModuleLoadingView);
            loadingView.DataSource.LoadViewModel(XLY.SF.Shell.Properties.Resources.ProposedSolutionConfig);         //传递推荐配置内容
            var loadingWindow = CreateNewWindow(loadingView);

            //登录界面
            //var loginView = IocManagerSingle.Instance.GetViewPart(ExportKeys.ModuleLoginView);
            //var loginWindow = CreateNewWindow(loginView);
            var loginView = IocManagerSingle.Instance.GetViewPart("TestPage");
            var loginWindow = CreateNewWindow(loginView);
            loginWindow.ShowInTaskbar = true;

            //设置前置窗口视图模型ID
            loginWindow.ParentViewModelID = loadingView.DataSource.ViewModelID;

            //添加加载界面与登录界面（这两个界面是固定顺序所以使用独特于其他的添加方式到当前打开的所有窗口）
            _curOpenWindows.Add(loadingView.DataSource.ViewModelID, new OpenWindowElement(loadingWindow, loginWindow));
            _curOpenWindows.Add(loginView.DataSource.ViewModelID, new OpenWindowElement(loginWindow));

            loadingWindow.Show();
        }

        #endregion

        #endregion

        #region 创建新窗口

        /// <summary>
        /// 创建新窗口
        /// </summary>
        /// <param name="view">显示内容</param>
        /// <returns></returns>
        private Shell CreateNewWindow(UcViewBase view, Window owner = null)
        {
            Shell newWindow = new Shell();
            if (view != null)
            {
                newWindow.WindowState = view.IsMaxView ? WindowState.Maximized : WindowState.Normal;
                //newWindow.SizeToContent = view.IsMaxView ? SizeToContent.WidthAndHeight: SizeToContent.Manual;
                newWindow.Title = view.Title ?? "";
                newWindow.Content = view;
                newWindow.Owner = owner;
                newWindow.AllowsTransparency = true;
                newWindow.WindowStyle = WindowStyle.None;
                newWindow.ResizeMode = ResizeMode.NoResize;
                newWindow.ShowInTaskbar = false;
                newWindow.Closed += newWindow_Closed;
            }
            return newWindow;
        }

        //窗口关闭（启动程序后的弹出框）
        private void newWindow_Closed(object sender, EventArgs e)
        {
            var curWin = sender as Shell;
            if (curWin != null)
            {
                var viewBase = curWin.Content as XLY.SF.Framework.Core.Base.ViewModel.UcViewBase;
                if (viewBase != null)
                {
                    if (_curOpenWindows.ContainsKey(viewBase.DataSource.ViewModelID))
                    {
                        //窗体关闭后执行
                        viewBase.DataSource.ViewClosed();
                        //查看是否有后续窗口
                        if (_curOpenWindows[viewBase.DataSource.ViewModelID].NavigationTargetWindow != null)
                        {
                            _curOpenWindows[viewBase.DataSource.ViewModelID].NavigationTargetWindow.Content.DataSource.LoadViewModel(null);
                            _curOpenWindows[viewBase.DataSource.ViewModelID].NavigationTargetWindow.Show();
                        }
                        else
                            //没有后续窗体，移除窗体
                            _curOpenWindows.Remove(viewBase.DataSource.ViewModelID);
                    }
                }
            }
        }

        #endregion
    }
}
