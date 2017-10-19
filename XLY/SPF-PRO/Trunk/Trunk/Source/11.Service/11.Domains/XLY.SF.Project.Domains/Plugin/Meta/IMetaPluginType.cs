using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.Plugin.IMetaPluginType
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/29 17:24:26
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 用于插件导出的元数据
    /// </summary>
    public interface IMetaPluginType
    {
        PluginType PluginType { get; }
    }
}
