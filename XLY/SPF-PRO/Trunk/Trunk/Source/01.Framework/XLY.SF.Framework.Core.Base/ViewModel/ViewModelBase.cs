using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.SystemKeys;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:23:37
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    public class ViewModelBase : NotifyPropertyBase
    {
        #region 公有属性定义

        /// <summary>
        /// ViewModel标识
        /// </summary>
        public Guid ViewModelID { get; private set; }

        /// <summary>
        /// 是否加载过
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// 事件聚合器（广播消息）
        /// </summary>
        public MsgAggregation MessageAggregation
        {
            get
            {
                return MsgAggregation.Instance;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建ViewModel
        /// </summary>
        protected ViewModelBase()
        {
            ViewModelID = Guid.NewGuid();
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 关闭视图，并注销所有消息监听（当界面关闭时会自动调用，无须手动执行）
        /// </summary>
        public virtual void ViewClosed()
        {
        }

        #endregion

        #region 关闭View

        /// <summary>
        /// 通过消息关闭View
        /// </summary>
        public void CloseView()
        {
            NavigationArgs args = new NavigationArgs(this.ViewModelID);
            //发送关闭View消息
            MsgAggregation.Instance.SendNavigationMsgForCloseWindow(args);
            //注销所有消息
            MsgAggregation.Instance.UnRegisterMsgAll(this);
        }

        #endregion

        #region 虚方法【加载ViewModel】

        /// <summary>
        /// 加载ViewModel，只加载一次
        /// </summary>
        public void LoadViewModel(object parameters)
        {
            if (IsLoaded) return;
            IsLoaded = true;
            LoadCore(parameters);
        }

        protected virtual void LoadCore(object parameters)
        {

        }

        #endregion

        #region 导航

        /// <summary>
        /// 主界面导航
        /// </summary>
        /// <param name="exportKey"></param>
        public void NavigationForMainWindow(string exportKey, object param = null)
        {
            NavigationArgs args = new NavigationArgs(exportKey, param);
            MsgAggregation.Instance.SendNavigationMsgForMainView(args);
        }

        /// <summary>
        /// 打开新窗体
        /// </summary>
        /// <param name="exportKey"></param>
        public void NavigationForNewWindow(string exportKey, object param = null)
        {
            NavigationArgs args = new NavigationArgs(exportKey, param);
            MsgAggregation.Instance.SendNavigationMsgForWindow(args);
        }

        /// <summary>
        /// 打开新窗体【模式对话框】
        /// </summary>
        /// <param name="exportKey"></param>
        public void NavigationForNewDislogWindow(string exportKey, object param = null)
        {
            NavigationArgs args = new NavigationArgs(exportKey, param);
            MsgAggregation.Instance.SendNavigationMsgForDialogWindow(args);
        }

        #endregion

        #region ViewContainer

        /// <summary>
        /// 当前View的容器【目前主要考虑用于屏幕截图所用】
        /// </summary>
        private object ViewContainer { get; set; }

        /// <summary>
        /// 设置ViewContainer
        /// </summary>
        /// <param name="container"></param>
        public void SetViewContainer(object container)
        {
            ViewContainer = container;
        }

        /// <summary>
        /// 获取ViewContainer
        /// </summary>
        /// <returns></returns>
        protected object GetViewContainer()
        {
            return ViewContainer;
        }

        #endregion
    }
}
