using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 类型Bool转换器
    /// </summary>
    public class TypeBoolConverter : IValueConverter
    {
        /// <summary>
        /// 项
        /// </summary>
        public TypeBoolConverterCollection Items { get; set; }

        /// <summary>
        /// 默认返回值
        /// </summary>
        public bool? DefaultResult { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (this.Items == null)
                return this.DefaultResult;

            foreach (TypeBoolConverterItem item in this.Items)
            {
                if (item.Type == null && value == null || item.Type != null && value != null && item.Type.Equals(value.GetType()))
                    return item.Value;
            }

            return this.DefaultResult;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 类型Bool转换器项
    /// </summary>
    public class TypeBoolConverterItem
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public bool Value { get; set; }
    }

    /// <summary>
    /// 类型Bool转换器项列表
    /// </summary>
    public class TypeBoolConverterCollection : List<TypeBoolConverterItem>
    {

    }
}
