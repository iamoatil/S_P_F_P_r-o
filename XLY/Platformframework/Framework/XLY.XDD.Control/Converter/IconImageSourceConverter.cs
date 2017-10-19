using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Drawing;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 将Icon转化为ImageSource
    /// </summary>
    public class IconImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                Icon img = value as Icon;

                Bitmap bitmap = img.ToBitmap();
                IntPtr hBitmap = bitmap.GetHbitmap();

                ImageSource wpfBitmap =
                      System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                           hBitmap, IntPtr.Zero, Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());

                return wpfBitmap;
            }
            catch
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
