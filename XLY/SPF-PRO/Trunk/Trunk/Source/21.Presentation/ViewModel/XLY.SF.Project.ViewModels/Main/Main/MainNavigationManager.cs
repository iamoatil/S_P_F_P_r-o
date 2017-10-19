using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.SystemKeys;
using XLY.SF.Project.ViewDomain.MefKeys;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/5/10 15:53:45
 * 类功能说明：
 * 1.主要管理主界面内的导航
 * 
 *************************************************/

namespace XLY.SF.Project.ViewModels.Main
{
    public class MainNavigationManager : XLY.SF.Framework.Core.Base.ViewModel.NotifyPropertyBase
    {
        public MainNavigationManager()
        {
            //注册主界面导航消息
            MsgAggregation.Instance.RegisterNaviagtionMsg(this, SystemKeys.MainUcNavigation, MainNavigationCallback);

            //获取附属界面【创建案例界面】
            //SubView = IocManagerSingle.Instance.GetViewPart(ExportKeys.CaseCreationView);
        }

        #region 当前界面显示的内容

        private object _curMainView;
        /// <summary>
        /// 当前显示的视图
        /// </summary>
        public object CurMainView
        {
            get { return _curMainView; }
            set
            {
                _curMainView = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 附属界面

        private object _subView;
        /// <summary>
        /// 附属界面
        /// </summary>
        public object SubView
        {
            get
            {
                return this._subView;
            }

            set
            {
                this._subView = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 主界面导航

        //主界面导航回调
        private void MainNavigationCallback(NavigationArgs args)
        {
            //首页需要重置界面状态
            if (args.MsgToken == ExportKeys.HomePageView)
                CollapsedCaseNameRow();
            else if (args.MsgToken == ExportKeys.CaseCreationView && !IsShowCurCaseNameRow)
                IsShowCurCaseNameRow = true;
            else if (args.MsgToken == ExportKeys.DeviceSelectView && !IsShowDeviceListRow)
                IsShowDeviceListRow = true;            

            CurMainView = args.View;            
        }

        /// <summary>
        /// 影藏案例名称行
        /// </summary>
        private void CollapsedCaseNameRow()
        {
            IsShowCurCaseNameRow = false;
            IsShowDeviceListRow = false;
        }

        #endregion

        #region 界面显示调整

        private bool _isShowCurCaseNameRow;
        /// <summary>
        /// 是否显示当前案例名称行
        /// </summary>
        public bool IsShowCurCaseNameRow
        {
            get
            {
                return this._isShowCurCaseNameRow;
            }

            set
            {
                this._isShowCurCaseNameRow = value;
                base.OnPropertyChanged();
            }
        }


        private bool _isShowDeviceListRow;
        /// <summary>
        /// 是否显示设备列表行
        /// </summary>
        public bool IsShowDeviceListRow
        {
            get
            {
                return this._isShowDeviceListRow;
            }

            set
            {
                this._isShowDeviceListRow = value;
                base.OnPropertyChanged();
            }
        }

        #endregion
        
    }
}
