using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ViewDomain.MefKeys
{
    public class ExportKeys
    {
        #region 其他

        #region 单例包装器
        /// <summary>
        /// 单例包装器
        /// </summary>
        public const string OtherSingleWrapper = "ExportKeys_OtherSingleWrapper";

        #endregion

        #region 配置文件Helper
        /// <summary>
        /// 配置文件Helper
        /// </summary>
        public const string OtherSysConfigHelper = "ExportKey_OtherSysConfigHelper";

        #endregion

        #region 消息框
        /// <summary>
        /// 消息框
        /// </summary>
        public const string OtherMessageBox = "ExportKey_OtherMessageBox";

        #endregion

        #region 模块接口
        /// <summary>
        /// 模块接口
        /// </summary>
        public const string OtherLoadModule = "ExportKey_OtherLoadModule";

        #endregion

        #endregion

        #region 模块

        #region 登录

        /// <summary>
        /// 登录View
        /// </summary>
        public const string ModuleLoginView = "ExportKey_ModuleLoginView";
        /// <summary>
        /// 登录ViewModel
        /// </summary>
        public const string ModuleLoginViewModel = "ExportKey_ModuleLoginViewModel";

        #endregion

        #region 加载

        /// <summary>
        /// 加载View
        /// </summary>
        public const string ModuleLoadingView = "ExportKey_ModuleLoadingView";
        /// <summary>
        /// 加载ViewModel
        /// </summary>
        public const string ModuleLoadingViewModel = "ExportKey_ModuleLoadingViewModel";

        #endregion

        #region 主界面

        /// <summary>
        /// 主界面View
        /// </summary>
        public const string ModuleMainUcView = "ExportKey_ModuleMainUcView";
        /// <summary>
        /// 主界面ViewModel
        /// </summary>
        public const string ModuleMainViewModel = "ExportKey_ModuleMainViewModel";

        #endregion

        #region 首页

        /// <summary>
        /// 首页View
        /// </summary>
        public const string HomePageView = "ExportKey_HomePageView";
        /// <summary>
        /// 首页ViewModel
        /// </summary>
        public const string HomePageViewModel = "ExportKey_HomePageViewModel";

        #endregion

        #region 模版

        /// <summary>
        /// 模版
        /// </summary>
        public const string ViewTemplateView = "ExportKey_ViewTemplateView";
        /// <summary>
        /// 模版
        /// </summary>
        public const string ViewTemplateViewModel = "ExportKey_ViewTemplateViewModel";

        #endregion

        #region 案例

        public const string CaseCreationView = "ExportKey_CaseCreationView";

        public const string CaseCreationViewModel = "ExportKey_CaseCreationViewModel";

        public const string CaseListView = "ExportKey_CaseListView";

        public const string CaseListViewModel = "ExportKey_CaseListViewModel";

        #endregion

        #region 数据源选择
        public const string DeviceSelectView = "DeviceSelectView";
        public const string DeviceSelectViewModel = "DeviceSelectViewModel";
        #endregion
        #region 设备提取首页
        public const string DeviceHomeView = "DeviceHomeView";
        public const string DeviceHomeViewModel = "DeviceHomeViewModel";
        #endregion
        #endregion

        #region 插件
        /// <summary>
        /// 表示导出为插件(如数据解析插件）
        /// </summary>
        public const string PluginKey = "plugin";
        /// <summary>
        /// 表示导出为脚本插件(如python插件）
        /// </summary>
        public const string PluginScriptKey = "PluginScriptKey";
        /// <summary>
        /// 表示导出为插件加载器
        /// </summary>
        public const string PluginLoaderKey = "PluginLoader";
        /// <summary>
        /// 表示导出为插件适配器
        /// </summary>
        public const string PluginAdapterKey = "PluginAdapter";
        /// <summary>
        /// 表示导出为脚本执行上下文
        /// </summary>
        public const string ScriptContextKey = "ScriptContext";
        #endregion
    }
}
