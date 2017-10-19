// ***********************************************************************
// Assembly:XLY.SF.Project.Plugin.Adapter
// Author:Songbing
// Created:2017-04-11 14:04:20
// Description:
// ***********************************************************************
// Last Modified By:You Min
// Last Modified On:2017/5/23
// ***********************************************************************

using System.Collections.Generic;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;
using System.Linq;


namespace XLY.SF.Project.Plugin.Adapter
{
    /// <summary>
    /// C#插件加载器
    /// </summary>
    [Export(PluginExportKeys.PluginLoaderKey, typeof(IPluginLoader))]
    public class NetPluginLoader : AbstractPluginLoader
    {
        protected override void LoadPlugin(IAsyncProgress asyn)
        {
            var plugins = IocManagerSingle.Instance.GetParts<IPlugin>(PluginExportKeys.PluginKey);
            Plugins = plugins.ToList().ConvertAll(p => p.Value);
        }
    }
}
