using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace XLY.XDD.Control
{
    /// <summary>
    /// Bool类型和图片的转换器
    /// </summary>
    public class BoolImageConverter : IValueConverter
    {
        /// <summary>
        /// 当值为True时的图片路径
        /// </summary>
        public string TrueImagePath { get; set; }

        /// <summary>
        /// 当值为False时的图片路径
        /// </summary>
        public string FalseImagePath { get; set; }

        /// <summary>
        /// 当值为Null时的图片路径
        /// </summary>
        public string NullImagePath { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
