using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.Log4NetService.LoggerEnum;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:36:08
 * 类功能说明：
 * 1.用于发送导航消息
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.MessageBase
{
    /// <summary>
    /// 导航消息
    /// </summary>
    public class NavigationArgs : ArgsBase
    {
        /// <summary>
        /// 目前待定
        /// </summary>
        public Guid ViewModelID { get; private set; }

        /// <summary>
        /// 导航目标视图
        /// </summary>
        public UcViewBase View { get; private set; }

        ///// <summary>
        ///// 导航到的界面View的ExportKey
        ///// </summary>
        //public string ViewExportKey { get; private set; }

        /// <summary>
        /// 是否创建成功
        /// </summary>
        public bool IsSuccess => View != null;

        /// <summary>
        /// 主界面内导航
        /// </summary>
        //public bool InMainWindow { get; private set; }

        /// <summary>
        /// View关闭消息
        /// </summary>
        /// <param name="openedViewModelID">打开的ViewModelID</param>
        public NavigationArgs(Guid openedViewModelID)
        {
            this.ViewModelID = openedViewModelID;
        }

        /// <summary>
        /// 不带参数的导航消息
        /// </summary>
        /// <param name="exportViewkey">显示的View</param>
        public NavigationArgs(string exportViewkey)
        {
            CreateParameter(exportViewkey);
        }

        /// <summary>
        /// 带参数的导航消息
        /// </summary>
        /// <param name="exportViewkey">显示的View</param>
        /// <param name="parameter">创建View时的参数</param>
        public NavigationArgs(string exportViewkey, object parameter)
        {
            CreateParameter(exportViewkey, parameter);
        }

        /// <summary>
        /// 创建消息参数
        /// </summary>
        /// <param name="exportViewkey"></param>
        /// <param name="parameter">传递的参数</param>
        private void CreateParameter(string exportViewkey, object parameter = null)
        {
            if (!string.IsNullOrEmpty(exportViewkey))
            {
                var view = IocManagerSingle.Instance.GetViewPart(exportViewkey);
                if (view != null)
                {
                    //传递参数
                    if (!view.DataSource.IsLoaded)
                    {
                        try
                        {
                            view.DataSource.LoadViewModel(parameter);
                        }
                        catch (Exception ex)
                        {
                            LoggerManagerSingle.Instance.Error(ex, string.Format("加载模块Key【{0}】失败", exportViewkey));
                            return;
                        }
                    }
                    this.ViewModelID = view.DataSource.ViewModelID;
                    //加载ViewContainer
                    view.DataSource.SetViewContainer(view);
                    this.View = view;
                    base.MsgToken = exportViewkey;
                }
                else
                    LoggerManagerSingle.Instance.Error(string.Format("导入模块Key【{0}】失败", exportViewkey));
            }
        }
    }
}
