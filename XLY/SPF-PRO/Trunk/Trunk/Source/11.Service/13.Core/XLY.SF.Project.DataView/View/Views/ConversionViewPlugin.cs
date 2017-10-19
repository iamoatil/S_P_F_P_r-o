using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataView.View.Views.ConversionViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:10:16
* ==============================================================================*/

namespace XLY.SF.Project.DataView.View.Views
{
    /// <summary>
    /// ConversionViewPlugin
    /// </summary>
    [Export(PluginExportKeys.PluginKey, typeof(IPlugin))]
    public class ConversionViewPlugin : AbstractDataViewPlugin
    {
        public ConversionViewPlugin()
        {
            var p = new DataViewPluginInfo() { Guid = "03987975-D89C-48B5-86D5-ABFE44EA3E71", Name = "对话模式", ViewType = new List<DataViewSupportItem>(), OrderIndex = 1, PluginType = PluginType.SpfDataView };
            p.ViewType.Add(new DataViewSupportItem() { PluginId = "*", TypeName = "MessageCore" });
            PluginInfo = p;
        }

        public override Control GetControl(DataViewPluginArgument arg)
        {
            ConversionControl ctrl = new ConversionControl();
            ctrl.DataContext = arg.CurrentNode;
            ctrl.OnSelectedDataChanged += OnSelectedDataChanged;
            return ctrl;
        }
    }
}
