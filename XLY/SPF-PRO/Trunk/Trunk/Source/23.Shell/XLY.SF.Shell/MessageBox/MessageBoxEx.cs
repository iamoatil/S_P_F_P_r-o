using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.SystemKeys;
using XLY.SF.Framework.Language;
using XLY.SF.Shell.CommWindow;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 17:17:41
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Shell.MessageBox
{
    [Export(typeof(IMessageBox))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public class MessageBoxEx : IMessageBox
    {
        /// <summary>
        /// 创建消息窗口
        /// </summary>
        /// <param name="title">消息窗口标题</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        private MessageBoxWin CreateMsgWindow(string title, string msg)
        {
            MessageBoxWin winResult = new MessageBoxWin();
            winResult.SetMsgBox(title, msg);
            if (Application.Current.MainWindow.GetType() == typeof(Shell)&&
                Application.Current.MainWindow.IsVisible)
                winResult.Owner = Application.Current.MainWindow;
            winResult.Topmost = true;
            return winResult;
        }

        /// <summary>
        /// 显示普通消息
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="msg">内容</param>
        public void ShowOtherMsg(string title, string msg)
        {
            var msgWin = CreateMsgWindow(title, msg);
            msgWin.Show();
        }

        /// <summary>
        /// 显示交互信息（需要用于确认，模式对话框）
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="msg">内容</param>
        /// <returns></returns>
        public bool ShowMutualMsg(string title, string msg)
        {
            var msgWin = CreateMsgWindow(title, msg);
            var result = msgWin.ShowDialog(); 
            return msgWin.DialogResultEx;
        }

        /// <summary>
        /// 显示错误消息（默认标题：错误信息）
        /// </summary>
        /// <param name="errorText">内容</param>
        /// <param name="isDialog">是否为模式对话框</param>
        public void ShowErrorMsg(string errorText)
        {
            var msgWin = CreateMsgWindow(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.ViewLanguage_View_MessageBox_Error), errorText);
            msgWin.Show();
        }

        /// <summary>
        /// 显示错误消息（模式对话框）
        /// </summary>
        public bool ShowDialogErrorMsg(string errorText)
        {
            return ShowMutualMsg(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.ViewLanguage_View_MessageBox_Error), errorText);
        }

        /// <summary>
        /// 显示提示消息（默认标题：提示信息）
        /// </summary>
        /// <param name="msg">内容</param>
        /// <param name="isDialog">是否为模式对话框</param>
        public void ShowNoticeMsg(string msg)
        {
            var msgWin = CreateMsgWindow(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.ViewLanguage_View_MessageBox_Notice), msg);
            msgWin.Show();
        }

        /// <summary>
        /// 显示通知信息（模式对话框）
        /// </summary>
        /// <param name="text">消息内容</param>
        public bool ShowDialogNoticeMsg(string text)
        {
            return ShowMutualMsg(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.ViewLanguage_View_MessageBox_Notice), text);
        }
    }
}
