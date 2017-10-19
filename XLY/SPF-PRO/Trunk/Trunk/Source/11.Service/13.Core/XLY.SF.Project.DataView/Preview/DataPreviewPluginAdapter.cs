using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataView.Preview.DataPreviewPluginAdapter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 11:26:24
* ==============================================================================*/

namespace XLY.SF.Project.DataView.Preview
{
    /// <summary>
    /// 数据预览插件适配器
    /// </summary>
    public class DataPreviewPluginAdapter
    {
        public DataPreviewPluginAdapter()
        {
            Plugins = PluginAdapter.Instance.GetPluginsByType<DataPreviewPluginInfo>(PluginType.SpfDataPreview).ToList().ConvertAll(p=>(AbstracDataPreviewPlugin)p.Value);
        }
        public static DataPreviewPluginAdapter Instance => SingleWrapperHelper<DataPreviewPluginAdapter>.Instance;

        public IEnumerable<AbstracDataPreviewPlugin> Plugins { get; set; }

        public IEnumerable<AbstracDataPreviewPlugin> GetView(string pluginId, object type)
        {
            if (type == null)
            {
                return new List<AbstracDataPreviewPlugin>();
            }
            string typeName = (type is Type) ? ((Type)type).Name : type.ToSafeString();
            return Plugins.Where(p =>
                ((DataPreviewPluginInfo)p.PluginInfo).ViewType.Any(v => (v.PluginId.Equals(pluginId) || v.PluginId == "*") && (v.TypeName.Equals(typeName) || v.TypeName == "*")))
                .OrderByDescending(iv => iv.PluginInfo.OrderIndex);
        }
    }
}
