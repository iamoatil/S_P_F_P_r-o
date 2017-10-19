using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 空字符串显示与否转换器
    /// </summary>
    public class StringNullVisibilityConverter : IValueConverter
    {
        public StringNullVisibilityConverter()
        {
            this.IsNullOrEmptyOrWhiteSpaceVisibility = System.Windows.Visibility.Collapsed;
            this.NormalVisibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// 如果是空字符串时的可见性
        /// </summary>
        public System.Windows.Visibility IsNullOrEmptyOrWhiteSpaceVisibility { get; set; }

        /// <summary>
        /// 正常字符串的可见性
        /// </summary>
        public System.Windows.Visibility NormalVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                if (value.ToSafeString().IsNullOrEmptyOrWhiteSpace())
                    return this.IsNullOrEmptyOrWhiteSpaceVisibility;
                else
                    return this.NormalVisibility;
            }
            else
            {
                return this.IsNullOrEmptyOrWhiteSpaceVisibility;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
