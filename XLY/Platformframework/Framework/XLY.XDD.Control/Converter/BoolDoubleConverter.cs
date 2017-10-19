using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace XLY.XDD.Control
{
    /// <summary>
    /// Bool类型与Double类型的转化
    /// </summary>
    public class BoolDoubleConverter : IValueConverter
    {
        public BoolDoubleConverter()
        {
            this.TrueValue = 1;
            this.FalseValue = 0;
        } 

        /// <summary>
        /// 为True时的值
        /// </summary>
        public double TrueValue { get; set; }

        /// <summary>
        /// 为False时的值
        /// </summary>
        public double FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (bool)value ? this.TrueValue : this.FalseValue;
            }
            catch
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
