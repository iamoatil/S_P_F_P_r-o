using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.CommonInterfaces.PluginInterface;
using XLY.SF.Project.Domains.DomainEntity.Plugin;

namespace XLY.SF.Project.Plugin.Adapter
{
    
    public interface IPluginAdapter
    {
        List<IPlugin> MatchPluginList(PluginType pType);
        List<IPlugin> GetPluginList();
        bool ExecutePlugin(PluginType plugin);
        bool ExecutePluginList(List<PluginType> pluginList);
    }
}
