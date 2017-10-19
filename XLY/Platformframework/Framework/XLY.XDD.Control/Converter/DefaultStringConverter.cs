using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 默认字符串转化器
    /// </summary>
    public class DefaultStringConverter : IValueConverter
    {
        /// <summary>
        /// 默认的字符串
        /// </summary>
        public string DefaultString { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                if (value.ToSafeString().IsNullOrEmptyOrWhiteSpace())
                    return this.DefaultString;
                else
                    return value;
            }
            else
            {
                return this.DefaultString;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
