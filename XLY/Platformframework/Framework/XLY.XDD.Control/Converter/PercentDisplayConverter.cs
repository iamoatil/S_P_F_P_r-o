using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 百分比显示转化器
    /// </summary>
    public class PercentDisplayConverter : IMultiValueConverter
    {
        /// <summary>
        /// 格式化
        /// </summary>
        public string Format { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values != null && values.Length == 2)
                {
                    string percent = ((100d * System.Convert.ToDouble(values[0])) / System.Convert.ToDouble(values[1])).ToString();
                    int place = percent.IndexOf('.');
                    if (place == -1)
                    {
                        return percent;
                    }
                    return percent.Substring(0, place + 2);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
