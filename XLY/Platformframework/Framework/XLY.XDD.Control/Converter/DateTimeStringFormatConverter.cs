using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 日期时间格式化转化器
    /// </summary>
    public class DateTimeStringFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && (values[0] is DateTime || values[0] is DateTime?) && values[1] is string)
            {
                if (values[0] is DateTime)
                {
                    DateTime dt = (DateTime)values[0];
                    string format = values[1].ToString();
                    return dt.ToString(format);
                }
                else
                {
                    DateTime? dt = values[0] as DateTime?;
                    string format = values[1].ToString();
                    return dt.Value.ToString(format);
                }
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
