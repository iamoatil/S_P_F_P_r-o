using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 时间轴展开图片转换器
    /// </summary>
    public class TimelineItemExpandImageConverter : IValueConverter
    {
        private static ImageSource AddImage;
        private static ImageSource SubtractImage;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isexpanded = (bool)value;
            if (isexpanded)
            {
                if (SubtractImage == null) SubtractImage = System.Utility.Helper.Images.CreateImageSourceFromImage(Resource.subtract);
                return SubtractImage;
            }
            else
            {
                if (AddImage == null) AddImage = System.Utility.Helper.Images.CreateImageSourceFromImage(Resource.add);
                return AddImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}