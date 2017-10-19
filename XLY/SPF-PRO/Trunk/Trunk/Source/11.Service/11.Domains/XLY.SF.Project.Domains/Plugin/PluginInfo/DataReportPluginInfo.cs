using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.DataReportPluginInfo
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/26 18:00:24
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据报表导出插件信息
    /// </summary>
    public class DataReportPluginInfo: AbstractZipPluginInfo
    {
        /// <summary>
        /// 平台名称，比如“蛛网”
        /// </summary>
        public string Platform { get; set; }
        /// <summary>
        /// 报表默认后缀名，比如“.bcp"
        /// </summary>
        public string ReportExtension { get; set; }
        /// <summary>
        /// 当前插件支持的模板列表
        /// </summary>
        public List<DataReportModulePluginInfo> Modules { get; set; }

        /// <summary>
        /// 如果大于0，则表示每页为多少行数据；
        /// 如果小于0，则表示每页显示所有数据；
        /// </summary>
        public int DataPageSize { get; set; }

        /// <summary>
        /// 插件文件名，相对路径，表示解析器调用的主插件
        /// </summary>
        public override string ScriptFile => "main.py";
    }
}
