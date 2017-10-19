using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Utility.Logger;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 图片路径到Image转化
    /// </summary>
    public class StringImageConverter : IValueConverter
    {
        public StringImageConverter()
        {
            this.IsWhenErrorReturnNull = true;
        }

        /// <summary>
        /// 默认图片路径
        /// </summary>
        public string DefaultImagePath { get; set; }

        /// <summary>
        /// 在转化失败的时候是否返回null
        /// </summary>
        public bool IsWhenErrorReturnNull { get; set; }

        /// <summary>
        /// 将一个路径转化为BitmapImage
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <returns></returns>
        public static BitmapImage Convert(string path)
        {
            try
            {
                if (!path.IsNullOrEmptyOrWhiteSpace())
                {
                    //如果是相对路径则转换为绝对路径
                    if (!System.IO.Path.IsPathRooted(path.TrimStart(' ', '/', '\\')))
                    {
                        path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                      path.TrimStart(' ', '/', '\\'));
                    }
                    BitmapImage bitmapImage;
                    bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(path);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    return bitmapImage;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message, ex);
                return null;
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                BitmapImage bi = StringImageConverter.Convert(value.ToSafeString());
                if (bi != null)
                    return bi;
                else if (this.IsWhenErrorReturnNull)
                    return null;
                else
                    return Binding.DoNothing;
            }
            else
            {
                if (this.IsWhenErrorReturnNull)
                    return null;
                else
                    return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
