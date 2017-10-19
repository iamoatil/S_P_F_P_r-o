using System.Collections.Generic;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Adapter
{
    /// <summary>
    /// 插件加载器接口
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// 加载插件
        /// </summary>
        /// <param name="asyn"></param>
        /// <returns></returns>
        IEnumerable<IPlugin> Load(IAsyncProgress asyn);
    }
}
