using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.AbstractDataReportPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/27 10:17:01
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// 报表插件基类
    /// </summary>
    public class AbstractDataReportPlugin : IPlugin
    {
        public IPluginInfo PluginInfo { get ; set ; }

        public virtual void Dispose()
        {
        }

        public object Execute(object arg, IAsyncProgress progress)
        {
            var p = arg as DataReportPluginArgument;
            Initialize(p, progress);
            ExportData(p, progress);
            ExportFile(p, progress);
            return ExportCompleted(p, progress);
        }

        /// <summary>
        /// 导出前初始化操作
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="progress"></param>
        protected virtual void Initialize(DataReportPluginArgument arg, IAsyncProgress progress) { }
        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="progress"></param>
        protected virtual void ExportData(DataReportPluginArgument arg, IAsyncProgress progress) { }
        /// <summary>
        /// 导出文件
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="progress"></param>
        protected virtual void ExportFile(DataReportPluginArgument arg, IAsyncProgress progress) { }
        /// <summary>
        /// 导出完成，返回导出后的文件或文件夹路径
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="progress"></param>
        protected virtual string ExportCompleted(DataReportPluginArgument arg, IAsyncProgress progress)
        {
            return null;
        }
    }

    /// <summary>
    /// 导出参数
    /// </summary>
    public class DataReportPluginArgument
    {
        /// <summary>
        /// 导出的目标文件夹或文件名
        /// </summary>
        public string ReportPath { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public IList<IDataSource> DataPool { get; set; }
        /// <summary>
        /// 设备信息
        /// </summary>
        public ExportDeviceInfo DeviceInfo { get; set; }
        /// <summary>
        /// 采集信息
        /// </summary>
        public ExportCollectionInfo CollectionInfo { get; set; }
        /// <summary>
        /// 导出时选择的报表模板名称，如果为null，则默认选择第一个
        /// </summary>
        public string ReportModuleName { get; set; }
        /// <summary>
        /// 是否导出资源文件
        /// </summary>
        public bool IsExportSourceFile { get; set; } = false;
    }
}
