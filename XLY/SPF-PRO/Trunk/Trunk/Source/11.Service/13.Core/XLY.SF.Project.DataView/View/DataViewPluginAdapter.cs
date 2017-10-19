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
* Assembly   ：	XLY.SF.Project.DataView.DataViewPluginAdapter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:06:18
* ==============================================================================*/

namespace XLY.SF.Project.DataView
{
    /// <summary>
    /// 数据展示插件适配器，用于加载插件和适配插件
    /// </summary>
    public class DataViewPluginAdapter
    {
        public DataViewPluginAdapter()
        {
            //var pl = IocManagerSingle.Instance.GetParts<IDataViewPlugin>(null);
            //Plugins = pl.ToList().ConvertAll(p => p.Value);
            Plugins = PluginAdapter.Instance.GetPluginsByType<DataViewPluginInfo>(PluginType.SpfDataView).ToList().ConvertAll(p => (AbstractDataViewPlugin)p.Value);
        }

        public static DataViewPluginAdapter Instance => SingleWrapperHelper<DataViewPluginAdapter>.Instance;

        /// <summary>
        /// 当前支持的所有数据视图插件
        /// </summary>
        public IEnumerable<AbstractDataViewPlugin> Plugins { get; set; }

        /// <summary>
        /// 根据当前选择的数据获取视图列表
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<AbstractDataViewPlugin> GetView(string pluginId, object type)
        {
            if (type == null)
            {
                return new List<AbstractDataViewPlugin>();
            }
            string typeName = (type is Type) ? ((Type)type).Name : type.ToSafeString();
            return Plugins.Where(p =>
                ((DataViewPluginInfo)p.PluginInfo).ViewType.Any(v => (v.PluginId.Equals(pluginId) || v.PluginId == "*") && (v.TypeName.Equals(typeName) || v.TypeName == "*")))
                .OrderByDescending(iv => iv.PluginInfo.OrderIndex);
        }
    }
}
