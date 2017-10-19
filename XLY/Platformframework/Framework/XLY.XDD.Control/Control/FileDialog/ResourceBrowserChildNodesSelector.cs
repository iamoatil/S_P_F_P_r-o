using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Grid;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 资源管理器子节点选择器
    /// </summary>
    public class ResourceBrowserChildNodesSelector : IChildNodesSelector
    {
        public System.Collections.IEnumerable SelectChildren(object item)
        {
            ResourceBrowserItem i = item as ResourceBrowserItem;
            if (i == null || i.IsFile)
                return null;

            if (i.Items == null)
            {
                i.Items = ResourceBrowserHelper.GetDirectorys(i.FullPath);
                if (i.Items == null)
                    return null;
                foreach (ResourceBrowserItem v in i.Items)
                {
                    v.Parent = i;
                }
            }
            return i.Items;
        }
    }
}
