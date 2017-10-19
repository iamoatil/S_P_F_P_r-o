using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.DataView.View.Controls;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataView.View.Views.DefaultGridViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:07:41
* ==============================================================================*/

namespace XLY.SF.Project.DataView.View.Views
{
    /// <summary>
    /// 默认表格视图
    /// </summary>
    [Export(PluginExportKeys.PluginKey, typeof(IPlugin))]
    public class DefaultGridViewPlugin : AbstractDataViewPlugin
    {
        public DefaultGridViewPlugin()
        {
            var p = new DataViewPluginInfo() { Guid = "7B51FA8D-F7F6-4EE3-B3B9-780C29B9B778", Name = "表格视图", ViewType = new List<DataViewSupportItem>(), OrderIndex = 0, PluginType = PluginType.SpfDataView };
            p.ViewType.Add(new DataViewSupportItem() { PluginId = "*", TypeName = "*" });
            PluginInfo = p;
        }

        public override Control GetControl(DataViewPluginArgument arg)
        {
            DefaultGridViewControl grid = new DefaultGridViewControl();
            grid.DataContext = arg.CurrentNode;
            grid.OnSelectedDataChanged += OnSelectedDataChanged;
            return grid;
        }
    }
}
