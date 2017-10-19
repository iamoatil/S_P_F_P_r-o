using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Data.Linq;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 用于比较两个对象的对象比较器（该比较器用于比较列表列相关属性）
    /// </summary>
    public class GridViewRowDataEqualityComparer : IEqualityComparer<object>
    {
        /// <summary>
        /// 列表列信息
        /// </summary>
        public IEnumerable<IGridViewColumn> Columns { get; set; }

        private readonly Regex reg = new Regex("^ProxyFileX\\.(?<FileName>.+)$");

        public bool Equals(object x, object y)
        {
            if (this.Columns == null)
                return false;

            Type x_type = x.GetType();
            Type y_type = y.GetType();
            object xValue = null;
            object yValue = null;
            var xpop = x_type.GetProperty("ProxyFileX");
            if (xpop != null)
            {
                xValue = xpop.GetValue(x);
                var ypop = y_type.GetProperty("ProxyFileX");
                yValue = ypop.GetValue(x);
                x_type = xValue.GetType();
                y_type = yValue.GetType();

            }
            foreach (IGridViewColumn c in Columns)
            {
                if (c.FieldName.IsNullOrEmptyOrWhiteSpace() || !c.IsDistinct)
                    continue;
                var FileName = string.Empty;
                FileName = c.FieldName.StartsWith("ProxyFileX") ?
                            reg.Match(c.FieldName).Groups["FileName"].Value :
                            c.FieldName;
                System.Reflection.PropertyInfo x_propertyinfo = x_type.GetProperty(FileName);
                System.Reflection.PropertyInfo y_propertyinfo = y_type.GetProperty(FileName);
                object x_value = null;
                object y_value = null;
                if (x_propertyinfo != null)
                    x_value = xpop == null ? x_propertyinfo.GetValue(x) : x_propertyinfo.GetValue(xValue);
                if (y_propertyinfo != null)
                    y_value = xpop == null ? y_propertyinfo.GetValue(y) : y_propertyinfo.GetValue(yValue);
                if (x_value == null && y_value == null)
                    continue;
                if (x_value == null && y_value != null || x_value != null && y_value == null || !x_value.Equals(y_value))
                    return false;
            }

            return true;
        }

        public int GetHashCode(object obj)
        {

            if (obj == null)
                return 0;

            Type type = obj.GetType();
            int hashCode = 0;

            object objValue = null;
            var xpop = type.GetProperty("ProxyFileX");
            if (xpop != null)
            {
                objValue = xpop.GetValue(obj);
                type = objValue.GetType();

            }
            foreach (IGridViewColumn c in Columns)
            {
                if (c.FieldName.IsNullOrEmptyOrWhiteSpace() || !c.IsDistinct)
                    continue;
                var FileName = string.Empty;
                FileName = c.FieldName.StartsWith("ProxyFileX") ?
                            reg.Match(c.FieldName).Groups["FileName"].Value :
                            c.FieldName;
                System.Reflection.PropertyInfo x_propertyinfo = type.GetProperty(FileName);
                object value = null;
                if (x_propertyinfo != null)
                    value = xpop == null ? x_propertyinfo.GetValue(obj) : x_propertyinfo.GetValue(objValue);
                if (value == null)
                    continue;
                hashCode ^= value.GetHashCode();
            }
            return hashCode;
        }
    }
}
