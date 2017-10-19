using System;
using System.Globalization;
using System.Windows.Data;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 时间轴数据项色块宽度转换器
    /// </summary>
    public class TimelineItemWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int max = parameter.ToSafeString().ToSafeInt();
            max = max == 0 ? 80 : max;
            var v = value.ToSafeString().ToSafeInt();
            if (v >= max) return max;
            return (int)(v / (double)max * max);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}