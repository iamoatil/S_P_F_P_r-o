using System.Collections.Generic;


/* ==============================================================================
* Assembly：	XLY.SF.Project.Domains.DataViewPluginInfo
* Description：	DataScriptPlugin  
* Author     ：	Fhjun
* Create Date：	2017/7/3 17:14:45
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 应用数据自定义视图插件配置信息
    /// </summary>
    public class DataViewPluginInfo : AbstractZipPluginInfo
    {
        /// <summary>
        /// 该数据视图对应的类型列表，格式为:"插件ID.类名;插件ID.类名;..."
        /// </summary>
        public List<DataViewSupportItem> ViewType { get; set; }

        /// <summary>
        /// 插件文件名，相对路径，表示解析器调用的主插件
        /// </summary>
        public override string ScriptFile => "index.html";
    }

    /// <summary>
    /// 该数据视图对应的类型列表，格式为:"插件ID.类名;插件ID.类名;..."
    /// </summary>
    public class DataViewSupportItem
    {
        /// <summary>
        /// 该数据视图对应的插件ID
        /// </summary>
        public string PluginId { get; set; }

        /// <summary>
        /// 该数据视图对应的类名
        /// </summary>
        public string TypeName { get; set; }

        public override string ToString()
        {
            return $"{PluginId}.{TypeName}";
        }
    }
}
