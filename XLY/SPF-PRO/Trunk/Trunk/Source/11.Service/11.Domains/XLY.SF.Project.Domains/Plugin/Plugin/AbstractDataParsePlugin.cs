// ***********************************************************************
// Assembly:XLY.SF.Framework.Core.CommonInterfaces.PluginInterface
// Author:Songbing
// Created:2017-04-13 11:43:41
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Security;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// C#数据解析插件虚基类
    /// </summary>
    public abstract class AbstractDataParsePlugin:IPlugin
    {
        public abstract IPluginInfo PluginInfo { get; set; }

        /// <summary>
        /// 插件执行的起始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 插件执行完成的时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        public abstract object Execute(object arg, IAsyncProgress progress);

        public virtual void Dispose()
        {

        }
    }
}
