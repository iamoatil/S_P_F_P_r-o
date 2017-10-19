using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.DataView.Preview.Controls;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataView.Preview.Views.DefaultPreviewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 11:27:28
* ==============================================================================*/

namespace XLY.SF.Project.DataView.Preview.Views
{
    /// <summary>
    /// 默认预览插件，展示数据的详细信息
    /// </summary>
    [Export(PluginExportKeys.PluginKey, typeof(IPlugin))]
    public class DefaultPreviewPlugin : AbstracDataPreviewPlugin
    {
        public DefaultPreviewPlugin()
        {
            var plugin = new DataPreviewPluginInfo() { Guid = "2F801B47-09F5-498C-966C-B06EFFB378A1", Name = "详细信息", ViewType = new List<DataViewSupportItem>(), OrderIndex = 0, PluginType = PluginType.SpfDataPreview };
            plugin.ViewType.Add(new DataViewSupportItem() { PluginId = "*", TypeName = "*" });
            PluginInfo = plugin;
        }

        public override Control GetControl(DataPreviewPluginArgument arg)
        {
            DefaultPreviewControl ctrl = new DefaultPreviewControl();
            ctrl.DataContext = arg.Item;
            return ctrl;
        }
    }
}
