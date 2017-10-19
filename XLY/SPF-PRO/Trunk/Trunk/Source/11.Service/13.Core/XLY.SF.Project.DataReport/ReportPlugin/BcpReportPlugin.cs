using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.BcpReportPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/30 9:44:19
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// Bcp格式的数据导出插件
    /// </summary>
    [Export(PluginExportKeys.PluginKey, typeof(IPlugin))]
    public class BcpReportPlugin : AbstractDataReportPlugin
    {
        public BcpReportPlugin()
        {
            PluginInfo = new DataReportPluginInfo()
            {
                Guid = "8C7B20AF-B0B3-4932-9FCB-185FDC2AA9B7",
                Name = "BCP",
                ReportExtension = ".zip",
                Description = "导出为Bcp格式的数据包文件",
                OrderIndex = 0,
                Platform = "蛛网",
                PluginType = PluginType.SpfReport,
                Group = ""
            };
        }

        private string _bcpPath = "";

        protected override void Initialize(DataReportPluginArgument arg, IAsyncProgress progress)
        {
            if (!Directory.Exists(arg.ReportPath))
            {
                Directory.CreateDirectory(arg.ReportPath);
            }
        }

        protected override void ExportData(DataReportPluginArgument arg, IAsyncProgress progress)
        {
            _bcpPath = new DataSourceToBcpConverter().ConverterToBcpFile(arg, arg.ReportPath);   
        }

        protected override string ExportCompleted(DataReportPluginArgument arg, IAsyncProgress progress)
        {
            return _bcpPath;
        }
    }
}
