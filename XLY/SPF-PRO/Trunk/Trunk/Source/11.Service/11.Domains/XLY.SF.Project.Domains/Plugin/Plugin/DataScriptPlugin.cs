using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Description：DataScriptPlugin  
* Author     ：Fhjun
* Create Date：2017/4/12 18:17:51
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 脚本插件
    /// </summary>
    [ExportMetadata("PluginType", PluginType.SpfDataParse)]
    [Export(PluginExportKeys.PluginScriptKey, typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataScriptPlugin : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public override object Execute(object arg, IAsyncProgress progress)
        {
            var context = GetContext();
            if(context == null)
            {
                throw new System.Exception("Dont find context!");
            }
            DataParsePluginInfo p = PluginInfo as DataParsePluginInfo;
            return context?.Execute(p.ScriptObject, progress);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private IScriptContext GetContext()
        {
            var plugins = IocManagerSingle.Instance.GetMetaParts<IScriptContext, IMetaScriptLanguage>("ScriptContext");
            foreach (var loader in plugins)
            {
                if (loader.Metadata.PluginLanguage == PluginLanguage.Python36)
                {
                    return loader.Value;
                }
            }
            return null;
        }
    }
}
