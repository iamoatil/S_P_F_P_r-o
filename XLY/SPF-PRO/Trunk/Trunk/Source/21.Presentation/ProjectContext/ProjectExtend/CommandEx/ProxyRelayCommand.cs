using GalaSoft.MvvmLight.Command;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Project.ViewDomain.Model;

namespace GalaSoft.MvvmLight.Command
{
    #region 基类

    /// <summary>
    /// 代理Command基类
    /// </summary>
    public class ProxyRelayCommandBase
    {
        protected ProxyRelayCommandBase()
        {

        }

        /// <summary>
        /// 操作日志
        /// </summary>
        protected SysCommonMsgArgs logArgs;

        /// <summary>
        /// 获取截图目标回调
        /// </summary>
        protected Func<object> _getContainerCallback;

        /// <summary>
        /// 界面绑定命令
        /// </summary>
        public ICommand ViewExecuteCmd { get; protected set; }

        /// <summary>
        /// 写入操作日志
        /// </summary>
        /// <param name="operationResult">操作结果内容</param>
        public void WriteOperationLog(string operationResult)
        {
            if (!string.IsNullOrWhiteSpace(operationResult))
            {
                //屏幕截图
                string screenShotPath = string.Empty;
                if (_getContainerCallback != null)
                {
                    var container = _getContainerCallback();
                    if (container != null && container is FrameworkElement)
                        screenShotPath = SystemContext.Instance.SaveOperationImageByWindow(container as FrameworkElement);
                }

                //记录日志
                SystemContext.Instance.AddOperationLog(new OperationLogParamElement()
                {
                    FunctionModule = "",
                    OperationContent = operationResult,
                    ScreenShotPath = screenShotPath
                });
            }
        }
    }

    #endregion

    /// <summary>
    /// RelayCommand代理，增加日志的输出.如不需要日志输出，可直接使用RelayCommand类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProxyRelayCommand<T> : ProxyRelayCommandBase
    {
        /// <summary>
        /// 内部使用回调
        /// </summary>
        private Func<T, string> _callback;

        /// <summary>
        /// 操作完成后汇报操作信息，并记录操作日志
        /// </summary>
        /// <param name="cmdByReportOpInfo"></param>
        /// <param name="screenShotTarget">是否需要截图</param>
        public ProxyRelayCommand(Func<T, string> cmdByReportOpInfo, Func<T, bool> canExecute = null)
        {
            _callback = cmdByReportOpInfo ??
                throw new ArgumentNullException(string.Format("功能方法【cmdByReportOpInfo】不能为NULL"));

            logArgs = new SysCommonMsgArgs(XLY.SF.Framework.Core.Base.SystemKeys.SystemKeys.WirteOperationLog);
            if (canExecute != null)
                ViewExecuteCmd = new RelayCommand<T>(ConcreateExecute, canExecute);
            else
                ViewExecuteCmd = new RelayCommand<T>(ConcreateExecute);
        }

        /// <summary>
        /// 操作完成后汇报操作信息，并记录操作日志同时截图保存
        /// </summary>
        /// <param name="cmdByReportOpInfo"></param>
        /// <param name="getViewContainerCallback">获取ViewContainer回调</param>
        /// <param name="canExecute"></param>
        public ProxyRelayCommand(Func<T, string> cmdByReportOpInfo, Func<object> getViewContainerCallback, Func<T, bool> canExecute)
            : this(cmdByReportOpInfo, canExecute)
        {
            _getContainerCallback = getViewContainerCallback;
        }

        private void ConcreateExecute(T t)
        {
            string operationResult = _callback(t);
            if (!string.IsNullOrWhiteSpace(operationResult))
                base.WriteOperationLog(operationResult);
        }
    }

    /// <summary>
    /// RelayCommand代理，增加日志的输出.如不需要日志输出，可直接使用RelayCommand类
    /// </summary>
    public class ProxyRelayCommand : ProxyRelayCommandBase
    {
        /// <summary>
        /// 内部使用回调
        /// </summary>
        private Func<string> _callback;

        /// <summary>
        /// 操作完成后汇报操作信息，并记录操作日志
        /// </summary>
        /// <param name="cmdByReportOpInfo"></param>
        /// <param name="canExecute"></param>
        public ProxyRelayCommand(Func<string> cmdByReportOpInfo, Func<bool> canExecute = null)
        {
            _callback = cmdByReportOpInfo ??
                throw new ArgumentNullException(string.Format("功能方法【cmdByReportOpInfo】不能为NULL"));

            logArgs = new SysCommonMsgArgs(XLY.SF.Framework.Core.Base.SystemKeys.SystemKeys.WirteOperationLog);
            if (canExecute != null)
                ViewExecuteCmd = new RelayCommand(ConcreateExecute, canExecute);
            else
                ViewExecuteCmd = new RelayCommand(ConcreateExecute);
        }

        /// <summary>
        /// 操作完成后汇报操作信息，并记录操作日志同时截图保存
        /// </summary>
        /// <param name="cmdByReportOpInfo"></param>
        /// <param name="getViewContainerCallback">获取ViewContainer回调</param>
        /// <param name="canExecute"></param>
        public ProxyRelayCommand(Func<string> cmdByReportOpInfo, Func<object> getViewContainerCallback, Func<bool> canExecute)
            : this(cmdByReportOpInfo, canExecute)
        {
            _getContainerCallback = getViewContainerCallback;
        }

        private void ConcreateExecute()
        {
            string operationResult = _callback();
            if (!string.IsNullOrWhiteSpace(operationResult))
                base.WriteOperationLog(operationResult);
        }
    }
}
