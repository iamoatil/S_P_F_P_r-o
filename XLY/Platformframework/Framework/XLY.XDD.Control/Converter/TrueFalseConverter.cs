using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace XLY.XDD.Control
{
    /// <summary>
    /// Bool类型值转化器
    /// </summary>
    public class TrueFalseConverter : IValueConverter
    {
        /// <summary>
        /// 如果是Bool?类型那么在空值的时候返回False，否则返回True
        /// </summary>
        public bool IsNullValueRtruenFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool?)
            {
                bool? b = (bool?)value;
                if (b == true)
                    return false;
                else if (b == false)
                    return true;
                else if (this.IsNullValueRtruenFalse)
                    return false;
                else
                    return true;
            }
            else if (value is bool)
            {
                return !(bool)value;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
