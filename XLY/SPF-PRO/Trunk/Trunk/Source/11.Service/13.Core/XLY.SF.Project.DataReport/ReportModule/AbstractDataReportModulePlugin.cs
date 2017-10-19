using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.AbstractDataReportModulePlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/27 10:24:10
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// AbstractDataReportModulePlugin
    /// </summary>
    public class AbstractDataReportModulePlugin : IPlugin
    {
        public IPluginInfo PluginInfo { get ; set ; }

        public void Dispose()
        {
            
        }

        public object Execute(object arg, IAsyncProgress progress)
        {
            return null;
        }
    }
}
