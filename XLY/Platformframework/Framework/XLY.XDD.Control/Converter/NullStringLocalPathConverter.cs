using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 空字符串本地路径转换器
    /// </summary>
    public class NullStringLocalPathConverter : IValueConverter
    {
        /// <summary>
        /// 默认的相对路径
        /// </summary>
        public string DefaultRelativePath { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                if (value.ToSafeString().IsNullOrEmptyOrWhiteSpace())
                    return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.DefaultRelativePath);
                else
                    return value;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
