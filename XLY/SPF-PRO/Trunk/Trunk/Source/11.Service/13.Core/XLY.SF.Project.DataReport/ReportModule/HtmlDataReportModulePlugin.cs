using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.HtmlDataReportModulePlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/28 14:25:57
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// HtmlDataReportModulePlugin
    /// </summary>
    //[Export(PluginExportKeys.PluginKey, typeof(IPlugin))]
    [ExportMetadata("PluginType", PluginType.SpfReportModule)]
    [Export(PluginExportKeys.PluginScriptKey, typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HtmlDataReportModulePlugin : AbstractDataReportModulePlugin
    {
        public HtmlDataReportModulePlugin()
        {
            //PluginInfo = new DataReportModulePluginInfo()
            //{
            //    Name = "Html模板(Bootstrap)",
            //    Description = "Bootstrap风格的Html报表模板",
            //    Guid = "557A0A88-692D-4087-9EAF-383FA21D9A01",
            //    OrderIndex = 0,
            //    PluginType = PluginType.SpfReportModule,
            //    ReportId = "DC22CD47-D05D-49D1-8B4E-98039FCB2BB0",
            //    MainFile = "index.html",
            //    ZipTempDirectory = @"C:\Users\fhjun\Desktop\html模板\"
            //};
        }
    }
}
