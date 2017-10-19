using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace XLY.XDD.Control
{
    /// <summary>
    /// Bool类型到Visibility类型的转化器
    /// </summary>
    public class BoolVisibilityConverter : IValueConverter
    {
        public BoolVisibilityConverter()
        {
            this.TrueVisibility = System.Windows.Visibility.Visible;
            this.FalseVisilibity = System.Windows.Visibility.Collapsed;
            this.NullVisilibity = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// 为真时的显示状态
        /// </summary>
        public System.Windows.Visibility TrueVisibility { get; set; }

        /// <summary>
        /// 为假时的显示状态
        /// </summary>
        public System.Windows.Visibility FalseVisilibity { get; set; }

        /// <summary>
        /// 为空时的显示状态
        /// </summary>
        public System.Windows.Visibility NullVisilibity { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return this.NullVisilibity;
            }
            else
            {
                return ((bool)value) ? this.TrueVisibility : this.FalseVisilibity;
            }

            //return value.ToSafeString() == "1" ? this.TrueVisibility : this.FalseVisilibity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
