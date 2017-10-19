using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android.Location
{
    [Export(PluginExportKeys.PluginKey, typeof(IPlugin))]
    public class AndroidLocationDataParser:AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidLocationDataParser()
        {
            PluginInfo = new DataParsePluginInfo();
        }

        public override object Execute(object arg, IAsyncProgress progress)
        {
            return null;
        }
    }
}
