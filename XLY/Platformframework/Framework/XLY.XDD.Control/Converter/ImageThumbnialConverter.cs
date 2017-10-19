using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 图片缩略图转换器
    /// </summary>
    public class ImageThumbnialConverter : IValueConverter
    {
        /// <summary>
        /// value为图片路径，parameter为大小的参数，格式为width,height，如：100_100
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string fileName = value.ToString();
                return this.CreateImageSourceThumbnia(fileName, parameter);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private ImageSource CreateImageSourceThumbnia(string fileName, object pars)
        {
            if (fileName.IsInvalid() || !System.IO.File.Exists(fileName)) return null;
            var paras = pars.ToSafeString().Split('_');
            double width = 100D, height = 100D;
            if (paras.Length == 2)
            {
                width = paras[0].ToDouble();
                height = paras[1].ToDouble();
            }
            return System.Utility.Helper.Images.CreateImageSourceThumbnia(fileName, width, height);
        }
    }
}