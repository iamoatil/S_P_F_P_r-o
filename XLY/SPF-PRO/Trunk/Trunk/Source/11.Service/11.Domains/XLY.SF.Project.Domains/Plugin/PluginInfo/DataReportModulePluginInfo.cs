using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.DataReportModulePluginInfo
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/27 9:49:12
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 报表导出模板插件信息
    /// </summary>
    [Serializable]
    [XmlRoot("plugin")]
    public class DataReportModulePluginInfo : AbstractZipPluginInfo
    {
        /// <summary>
        /// 当前模板所属的报表插件ID
        /// </summary>
        [XmlElement("report")]
        public string ReportId { get; set; }

        /// <summary>
        /// 报表模板运行的主插件文件，比如“index.html"
        /// </summary>
        [XmlElement("start")]
        public string MainFile { get; set; }

        public override string ScriptFile => MainFile;
    }
}
