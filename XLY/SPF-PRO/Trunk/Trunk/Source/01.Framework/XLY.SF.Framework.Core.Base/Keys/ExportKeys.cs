using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:44:41
 * 类功能说明：
 * 1.作为导出标记使用
 * 2.不能重复定义，避免导出不正确数据
 * 3.分模块定义，方便查看
 * 4.命名规范：目前分为两大类，其他：以Other开头命名。模块：以Module开头命名
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.Keys
{
    /// <summary>
    /// 导出模块定义
    /// </summary>
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

        #region 测试

        public const string ModuleTestView1 = "ExportKey_ModuleTestView1";
        public const string ModuleTestViewModel1 = "ExportKey_ModuleTestViewModel1";

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
