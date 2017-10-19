using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.SystemKeys;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.ViewDomain.MefKeys;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/2 10:05:11
 * 类功能说明：
 * 1.监听系统未捕获错误，以及线程未捕获错误。并记录日志
 *
 *************************************************/

namespace XLY.SF.Shell.SpfException
{
    /// <summary>
    /// 异常服务
    /// </summary>
    public class ExceptionHelper
    {
        [Import(typeof(IMessageBox))]
        private IMessageBox _msgBox;

        public ExceptionHelper()
        {
            //_msgBox = IocManagerSingle.Instance.GetPart<IMessageBox>();
        }

        /// <summary>
        /// 初始化异常监听
        /// </summary>
        public void Init()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var a = e.Exception is ApplicationException;

            e.Handled = true;
            LoggerManagerSingle.Instance.Error(e.Exception);
            _msgBox.ShowDialogErrorMsg("程序出现严重错误，即将关闭");
            Application.Current.Shutdown();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LoggerManagerSingle.Instance.Error(e.ExceptionObject.ToString());
            if (e.IsTerminating)
            {
                Application.Current.Shutdown();
                _msgBox.ShowDialogErrorMsg("程序出现严重错误，即将关闭");
            }
        }
    }
}
