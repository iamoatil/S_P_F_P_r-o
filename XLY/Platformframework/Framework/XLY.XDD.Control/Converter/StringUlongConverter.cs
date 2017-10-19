using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 字符串Ulong类型转换器
    /// </summary>
    public class StringUlongConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToSafeString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 0u;
            ulong ul;
            if (!ulong.TryParse(value.ToString(), out ul))
                return Binding.DoNothing;
            else
                return ul;
        }
    }
}
