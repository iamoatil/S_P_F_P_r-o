using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.IPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/12 10:27:37
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 所有插件定义的接口
    /// </summary>
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// 插件信息
        /// </summary>
        IPluginInfo PluginInfo { get; set; }

        /// <summary>
        /// 执行插件
        /// </summary>
        /// <returns></returns>
        object Execute(object arg, IAsyncProgress progress);
    }
}
