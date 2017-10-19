using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataView.AbstractDataViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:05:22
* ==============================================================================*/

namespace XLY.SF.Project.DataView
{
    /// <summary>
    /// 数据展示插件接口定义
    /// </summary>
    public abstract class AbstractDataViewPlugin : IPlugin
    {
        public IPluginInfo PluginInfo { get; set; }

        public object Execute(object arg, IAsyncProgress progress)
        {
            return GetControl(arg as DataViewPluginArgument);
        }

        public void Dispose()
        {

        }

        public event DelgateDataViewSelectedItemChanged SelectedDataChanged;

        /// <summary>
        /// 触发数据选择事件
        /// </summary>
        /// <param name="data"></param>
        protected void OnSelectedDataChanged(object data)
        {
            SelectedDataChanged?.Invoke(data);
        }

        public abstract Control GetControl(DataViewPluginArgument arg);
    }

    /// <summary>
    /// 数据展示空控件当前选择项改变事件（主要用于数据预览窗口更新）
    /// </summary>
    /// <param name="data"></param>
    public delegate void DelgateDataViewSelectedItemChanged(object data);

    public class DataViewPluginArgument
    {
        public SPFTask Task { get; set; }
        public IDataSource DataSource { get; set; }
        public TreeNode CurrentNode { get; set; }
    }
}
