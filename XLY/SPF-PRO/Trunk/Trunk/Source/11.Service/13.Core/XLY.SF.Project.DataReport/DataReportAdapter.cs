using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.DataReportAdapter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:06:18
* ==============================================================================*/


namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// 数据报表插件适配器，用于加载插件和适配插件
    /// </summary>
    public class DataReportAdapter
    {
        public DataReportAdapter()
        {
            var pluginModules = PluginAdapter.Instance.GetPluginsByType<DataReportModulePluginInfo>(PluginType.SpfReportModule).ToList().ConvertAll(p => (AbstractDataReportModulePlugin)p.Value)
                .ConvertAll(m => m.PluginInfo as DataReportModulePluginInfo).OrderBy(m => m.OrderIndex);
            Plugins = PluginAdapter.Instance.GetPluginsByType<DataReportPluginInfo>(PluginType.SpfReport).ToList().ConvertAll(p => (AbstractDataReportPlugin)p.Value);
            foreach(var p in Plugins)   //添加报表模板信息
            {
                if(p.PluginInfo is DataReportPluginInfo rp)
                {
                    rp.Modules = pluginModules.Where(m=> m != null && m.ReportId == rp.Guid).ToList();
                }
            }
        }

        public static DataReportAdapter Instance => SingleWrapperHelper<DataReportAdapter>.Instance;

        /// <summary>
        /// 当前支持的所有报表插件
        /// </summary>
        public IEnumerable<AbstractDataReportPlugin> Plugins { get; set; }

    }
}
