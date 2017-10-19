using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace XLY.XDD.Control
{
    public class NullVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 值为空时的转化类型
        /// </summary>
        public System.Windows.Visibility NullVisibility { get; set; }

        /// <summary>
        /// 值不为空时的转化类型
        /// </summary>
        public System.Windows.Visibility NotNullVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return this.NullVisibility;
            }
            else
            {
                return this.NotNullVisibility;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
