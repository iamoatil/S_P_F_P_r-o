using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.SystemKeys;
using XLY.SF.Framework.Log4NetService;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 17:01:14
 * 类功能说明：
 * 1.消息聚合器（用于发送、接收和注销消息）
 * 2.目前消息聚合器已经添加自动识别线程调用功能（除普通消息外）
 *      既：使用UIDispatcher发送消息，会自动判断当前发送者是否在UI线程上。
 *          目前此功能为考虑性能问题，有必要时可以根据选择来判断是否使用UIDispatcher
 * 3.监听消息和注销消息，目前未做线程判断
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.MessageAggregation
{
    public class MsgAggregation
    {
        #region Single

        private static object _objLock = new object();

        private MsgAggregation()
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.Initialize();
        }

        private static MsgAggregation _instance;

        public static MsgAggregation Instance
        {
            get
            {
                if (_instance == null)
                    lock (_objLock)
                        if (_instance == null)
                            _instance = new MsgAggregation();
                return _instance;
            }
        }

        #endregion

        #region 消息发送

        #region 导航（UI线程）

        /// <summary>
        /// 导航到新窗口【模式对话框】
        /// </summary>
        /// <param name="args">参数</param>
        public void SendNavigationMsgForDialogWindow(NavigationArgs args)
        {
            if (args.IsSuccess)
            {
                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<NavigationArgs>(args, SystemKeys.SystemKeys.OpenNewDialogWindow);
                });
            }
        }

        /// <summary>
        /// 导航到新窗口
        /// </summary>
        /// <param name="args">参数</param>
        public void SendNavigationMsgForWindow(NavigationArgs args)
        {
            if (args.IsSuccess)
            {
                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<NavigationArgs>(args, SystemKeys.SystemKeys.OpenNewWindow);
                });
            }
        }

        /// <summary>
        /// 主界面导航
        /// </summary>
        /// <param name="args">参数</param>
        public void SendNavigationMsgForMainView(NavigationArgs args)
        {
            if (args.IsSuccess)
            {
                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<NavigationArgs>(args, SystemKeys.SystemKeys.MainUcNavigation);
                });
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="args">参数</param>
        public void SendNavigationMsgForCloseWindow(NavigationArgs args)
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<NavigationArgs>(args, SystemKeys.SystemKeys.CloseWindow);
            });
        }

        #endregion

        #region 普通消息发送

        /// <summary>
        /// 发送普通消息（任意线程）
        /// </summary>
        /// <param name="msgData"></param>
        public void SendGeneralMsg(GeneralArgs msgData)
        {
            try
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<GeneralArgs>(msgData, msgData.GeneralKey);
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
        }

        /// <summary>
        /// 发送普通消息（UI线程）
        /// </summary>
        /// <param name="msgData"></param>
        public void SendGeneralMsgToUI(GeneralArgs msgData)
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<GeneralArgs>(msgData, msgData.GeneralKey);
            });
        }

        #endregion

        #region 系统消息发送（UI线程）

        /// <summary>
        /// 发送系统级消息
        /// </summary>
        /// <param name="args">消息</param>
        public void SendSysMsg(SysCommonMsgArgs args)
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<SysCommonMsgArgs>(args, args.MsgToken);
            });
        }

        #endregion

        #endregion

        #region 监听消息

        #region 系统消息

        /// <summary>
        /// 注册系统消息
        /// </summary>
        /// <param name="recipient">消息的接受者</param>
        /// <param name="token">消息标识</param>
        /// <param name="msgCallback">消息回调</param>
        public void RegisterSysMsg(object recipient, object token, Action<SysCommonMsgArgs> msgCallback)
        {
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<SysCommonMsgArgs>(recipient, token, msgCallback);
            RegisterMsg<SysCommonMsgArgs>(recipient, token, msgCallback);
        }

        /// <summary>
        /// 注册系统消息
        /// </summary>
        /// <param name="recipient">消息的接受者</param>
        /// <param name="token">消息标识</param>
        /// <param name="msgCallback">消息回调</param>
        public void RegisterSysMsg<TParam>(object recipient, object token, Action<SysCommonMsgArgs<TParam>> msgCallback)
        {
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<SysCommonMsgArgs<TParam>>(recipient, token, msgCallback);
            RegisterMsg<SysCommonMsgArgs<TParam>>(recipient, token, msgCallback);
        }

        #endregion

        #region 普通消息

        /// <summary>
        /// 注册普通消息
        /// </summary>
        /// <param name="recipient">消息的接受者</param>
        /// <param name="token">消息标识</param>
        /// <param name="msgCallback">消息回调</param>
        public void RegisterGeneralMsg(object recipient, string token, Action<GeneralArgs> msgCallback)
        {
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<GeneralArgs>(recipient, token, msgCallback);
            RegisterMsg<GeneralArgs>(recipient, token, msgCallback);
        }

        /// <summary>
        /// 注册普通消息
        /// </summary>
        /// <param name="recipient">消息的接受者</param>
        /// <param name="token">消息标识</param>
        /// <param name="msgCallback">消息回调</param>
        public void RegisterGeneralMsg<TParam>(object recipient, string token, Action<GeneralArgs<TParam>> msgCallback)
        {
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<GeneralArgs<TParam>>(recipient, token, msgCallback);
            RegisterMsg<GeneralArgs<TParam>>(recipient, token, msgCallback);
        }

        #endregion

        #region 导航消息

        /// <summary>
        /// 注册导航消息
        /// </summary>
        /// <param name="recipient">消息的接受者</param>
        /// <param name="token">消息标识</param>
        /// <param name="msgCallback">消息回调</param>
        public void RegisterNaviagtionMsg(object recipient, string token, Action<NavigationArgs> msgCallback)
        {
            RegisterMsg<NavigationArgs>(recipient, token, msgCallback);
        }

        #endregion

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="TArgs">要监听消息类型</typeparam>
        /// <param name="recipient">消息的接受者</param>
        /// <param name="token">消息标识</param>
        /// <param name="msgCallback">消息回调</param>
        private void RegisterMsg<TArgs>(object recipient, object token, Action<TArgs> msgCallback)
            where TArgs : ArgsBase
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<TArgs>(recipient, token, msgCallback);
        }

        #endregion

        #region 取消监听

        /// <summary>
        /// 取消消息监听
        /// </summary>
        /// <typeparam name="TArgs">要监听消息类型</typeparam>
        /// <param name="recipient">消息的接受者</param>
        /// <param name="token">消息标识</param>
        /// <param name="msgCallback">消息回调</param>
        public void UnRegisterMsg<TArgs>(object recipient, object token, Action<TArgs> msgCallback)
            where TArgs : ArgsBase
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister(recipient, token, msgCallback);
        }

        /// <summary>
        /// 取消消息接受者的所有消息监听
        /// </summary>
        /// <param name="recipient">消息的接受者</param>
        public void UnRegisterMsgAll(object recipient)
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister(recipient);
        }

        #endregion
    }
}
