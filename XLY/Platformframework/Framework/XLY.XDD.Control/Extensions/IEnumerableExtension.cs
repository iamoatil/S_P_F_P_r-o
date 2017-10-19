using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 列表类型扩展
    /// </summary>
    public static class IEnumerableExtension
    {
        /// <summary>
        /// 获取第一个节点的数据类型
        /// </summary>
        /// <param name="items">要获取的数据集合</param>
        /// <returns></returns>
        public static Type GetFirstItemType(this IEnumerable items)
        {
            if (items == null)
                return null;
            Type type = null;
            foreach (object i in items)
            {
                type = i.GetType();
                break;
            }
            return type;
        }
    }
}
