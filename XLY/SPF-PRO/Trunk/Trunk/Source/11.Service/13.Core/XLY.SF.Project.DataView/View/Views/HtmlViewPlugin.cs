using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataView.View.Views.HtmlViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:10:56
* ==============================================================================*/

namespace XLY.SF.Project.DataView.View.Views
{
    /// <summary>
    /// Html数据视图插件，为通过脚本编辑器自定义的视图
    /// </summary>
    [ExportMetadata("PluginType", PluginType.SpfDataView)]
    [Export(PluginExportKeys.PluginScriptKey, typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HtmlViewPlugin : AbstractDataViewPlugin
    {
        public override Control GetControl(DataViewPluginArgument arg)
        {
            var plugin = PluginInfo as DataViewPluginInfo;
            HtmlViewControl ctrl = new HtmlViewControl(plugin.ScriptObject, arg.CurrentNode.Items);
            ctrl.OnSelectedDataChanged += OnSelectedDataChanged;
            return ctrl;
        }

        public override string ToString()
        {
            return $"自定义Html视图插件：{PluginInfo.Name}";
        }
    }
}
