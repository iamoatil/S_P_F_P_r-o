using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 毫秒时间格式转化器
    /// </summary>
    public class MillisecondsTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                return TimeSpan.FromMilliseconds((double)value).ToString().Substring(0, 8);
            }
            else
            {
                return TimeSpan.MinValue.ToString().Substring(0, 8);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
