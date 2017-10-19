using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 11:38:32
 * 接口功能说明：
 *      1. 框架核心接口
 *      2. 实现此接口用于弹出消息对话框
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.CoreInterface
{
    public interface IMessageBox
    {
        /// <summary>
        /// 显示其他消息
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="text">内容</param>
        void ShowOtherMsg(string title, string text);

        /// <summary>
        /// 显示交互消息（需要用户确认）
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="text">内容</param>
        /// <returns></returns>
        bool ShowMutualMsg(string title, string text);

        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="errorText">消息内容</param>
        void ShowErrorMsg(string errorText);

        /// <summary>
        /// 显示错误消息（模式对话框）
        /// </summary>
        bool ShowDialogErrorMsg(string errorText);

        /// <summary>
        /// 显示通知信息
        /// </summary>
        /// <param name="text">消息内容</param>
        void ShowNoticeMsg(string text);

        /// <summary>
        /// 显示通知信息（模式对话框）
        /// </summary>
        /// <param name="text">消息内容</param>
        bool ShowDialogNoticeMsg(string text);
    }
}
