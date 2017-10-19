using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 导出模块定义
    /// </summary>
    public class PluginExportKeys
    {
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
    }
}
