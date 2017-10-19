using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace XLY.XDD.Control
{
    /// <summary>
    /// Bool类型和字符串的转化器
    /// </summary>
    public class BoolStringConverter : IValueConverter
    {
        public BoolStringConverter()
        {
            this.TrueString = "True";
            this.FalseString = "False";
            this.NullString = "Null";
        }

        /// <summary>
        /// 值为True时的字符串
        /// </summary>
        public string TrueString { get; set; }

        /// <summary>
        /// 值为False时的字符串
        /// </summary>
        public string FalseString { get; set; }

        /// <summary>
        /// 值为Null时的字符串
        /// </summary>
        public string NullString { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool?)
            {
                bool? v = (bool?)value;
                if (v == true)
                {
                    return this.TrueString;
                }
                else if (v == false)
                {
                    return this.FalseString;
                }
                else
                {
                    return this.NullString;
                }
            }
            if (value is bool)
            {
                return ((bool)value) ? this.TrueString : this.FalseString;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
