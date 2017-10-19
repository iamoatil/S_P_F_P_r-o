using System.Collections.Generic;
using System.Data;
using System.Dynamic;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 14:07:49
 * 类功能说明：
 *
 *************************************************/

namespace X86DllServer.Service
{
    public static partial class BaseTypeExtension
    {
        #region ToDynamicCollection
        /// <summary>
        /// 转换为动态类型集合
        /// </summary>
        public static List<dynamic> ToDynamicCollection(this DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0 || dt.Columns.Count <= 0)
            {
                return new List<dynamic>();
            }
            var items = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
                items.Add(dyn);
            }
            return items;
        }
        #endregion

    }
}
