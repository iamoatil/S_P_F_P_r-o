using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.SystemKeys;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.Models;
using XLY.SF.Project.ProxyService;
using XLY.SF.Project.ViewDomain.MefKeys;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 17:35:02
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.ViewModels.Loading
{
    [Export(ExportKeys.ModuleLoadingViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class LoadingViewModel : ViewModelBase
    {
        /// <summary>
        /// 消息服务
        /// </summary>
        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox { get; set; }

        protected override void LoadCore(object parameters)
        {
            //执行加载
            ExecuteSysLoad(parameters.ToString());
        }

        public override void ViewClosed()
        {

        }

        private void ExecuteChangeLanguageCommand()
        {

        }

        #region 执行系统加载

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="solutionContentFromXml">推荐方案内容，XML文件格式</param>
        /// <returns></returns>
        private bool ConcreateLoadOfTask(object solutionContentFromXml)
        {
            //初始化系统上下文
            bool opertionResult = SystemContext.Instance.InitSysInfo() &&
                SystemContext.Instance.LoadProposedSolution(solutionContentFromXml.ToString());

            //开启设备监听服务
            ProxyFactory.DeviceMonitor.OpenDeviceService();

            return opertionResult;
        }

        private async void ExecuteSysLoad(string solutionContentFromXml)
        {
            var opertionResult = await Task<bool>.Factory.StartNew(ConcreateLoadOfTask, solutionContentFromXml);

            if (!opertionResult)
            {
                //加载信息失败，关闭程序
                MessageBox.ShowDialogErrorMsg("系统加载失败，即将关闭程序。");
                SysCommonMsgArgs args = new SysCommonMsgArgs(SystemKeys.ShutdownProgram);
                base.MessageAggregation.SendSysMsg(args);
            }

            //完成加载，进入登录界面
            base.CloseView();
        }

        #endregion
    }
}
