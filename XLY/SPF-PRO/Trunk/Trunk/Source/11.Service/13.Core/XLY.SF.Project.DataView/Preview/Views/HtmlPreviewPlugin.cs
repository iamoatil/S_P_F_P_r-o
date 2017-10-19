using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataView.Preview.Views.HtmlPreviewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 11:30:42
* ==============================================================================*/

namespace XLY.SF.Project.DataView.Preview.Views
{
    /// <summary>
    /// HtmlPreviewPlugin
    /// </summary>
    [ExportMetadata("PluginType", PluginType.SpfDataPreview)]
    [Export(PluginExportKeys.PluginScriptKey, typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HtmlPreviewPlugin : AbstracDataPreviewPlugin
    {
        public override Control GetControl(DataPreviewPluginArgument arg)
        {
            var plugin = PluginInfo as DataPreviewPluginInfo;
            HtmlViewControl ctrl = new HtmlViewControl(plugin.ScriptObject, arg.Item);
            return ctrl;
        }
    }
}
