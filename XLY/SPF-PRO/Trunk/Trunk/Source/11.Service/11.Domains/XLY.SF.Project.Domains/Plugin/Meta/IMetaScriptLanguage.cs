using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.Plugin.Meta.IMetaScriptLanguage
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/29 18:04:20
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 用于脚本执行上下文导出的元数据
    /// </summary>
    public interface IMetaScriptLanguage
    {
        PluginLanguage PluginLanguage { get; }
    }
}
