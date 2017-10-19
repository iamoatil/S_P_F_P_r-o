using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/4/20 11:39:24
 * 类功能说明：
 * 1. 增加实现是否完成动画再执行导航
 * 
 *************************************************/

namespace XLY.SF.Shell.NavigationManager
{
    /// <summary>
    /// 打开窗口元素
    /// </summary>
    public class OpenWindowElement
    {
        /// <summary>
        /// 当前打开的窗口
        /// </summary>
        public Shell CurOpenWindow { get; private set; }

        /// <summary>
        /// 导航目标窗口
        /// </summary>
        public Shell NavigationTargetWindow { get; private set; }

        /// <summary>
        /// 创建需要等待关闭动画的窗口元素（目前登录专用）
        /// </summary>
        /// <param name="curOpenWindow">当前打开的窗口</param>
        /// <param name="navigationTargetWindow">关闭打开窗口后，导航的目标窗口</param>
        public OpenWindowElement(Shell curOpenWindow, Shell navigationTargetWindow)
        {
            this.CurOpenWindow = curOpenWindow;
            this.NavigationTargetWindow = navigationTargetWindow;
        }

        /// <summary>
        /// 创建不需要等待关闭动画的窗口元素
        /// </summary>
        /// <param name="curOpenWindow"></param>
        public OpenWindowElement(Shell curOpenWindow)
        {
            this.CurOpenWindow = curOpenWindow;
        }
    }
}
