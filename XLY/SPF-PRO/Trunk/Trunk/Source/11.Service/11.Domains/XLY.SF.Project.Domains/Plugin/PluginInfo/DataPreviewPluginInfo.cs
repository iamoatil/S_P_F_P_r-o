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
    /// 应用数据自定义预览视图插件配置信息
    /// </summary>
    public class DataPreviewPluginInfo : AbstractZipPluginInfo
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
}
