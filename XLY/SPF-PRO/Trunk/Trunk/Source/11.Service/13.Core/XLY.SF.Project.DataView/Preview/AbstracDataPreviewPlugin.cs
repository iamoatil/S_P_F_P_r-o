using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataView.IDataPreviewPlugin
* Description： XLY.SF.Project.DataView	  
* Author     ：	fhjun
* Create Date：	2017/8/24 16:47:32
* ==============================================================================*/

namespace XLY.SF.Project.DataView
{
    /// <summary>
    /// 数据预览展示插件接口定义
    /// </summary>
    public abstract class AbstracDataPreviewPlugin : IPlugin
    {
        public IPluginInfo PluginInfo { get; set; }

        public void Dispose()
        {
            
        }

        public object Execute(object arg, IAsyncProgress progress)
        {
            return GetControl(arg as DataPreviewPluginArgument);
        }

        public abstract Control GetControl(DataPreviewPluginArgument arg);
    }

    public class DataPreviewPluginArgument
    {
        public SPFTask Task { get; set; }
        public IDataSource DataSource { get; set; }
        public TreeNode CurrentNode { get; set; }
        public object Item { get; set; }
    }
}
