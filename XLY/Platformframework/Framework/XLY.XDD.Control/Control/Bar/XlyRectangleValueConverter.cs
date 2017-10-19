using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace XLY.XDD.Control
{
    /// <summary>
    /// Xly矩形值转换器
    /// </summary>
    public class XlyRectangleValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                double value = (double)values[0];
                double min = (double)values[1];
                double max = (double)values[2];
                double a_value = (double)values[3];

                double result = value * (a_value / (max - min));
                return result > a_value ? a_value : result;
            }
            catch
            {
                return 0d;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
