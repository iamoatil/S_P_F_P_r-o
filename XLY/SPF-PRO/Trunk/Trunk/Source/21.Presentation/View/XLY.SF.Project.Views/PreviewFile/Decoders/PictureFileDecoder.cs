using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace XLY.SF.Project.Views.PreviewFile.Decoders
{
    class PictureFileDecoder : IFileDecoder
    {
        public FrameworkElement Element
        {
            get
            {
                return _image;
            }
        }

        private Image _image = new Image();

        public void Decode(string path)
        {
            _image.Source = new BitmapImage(new Uri(path));
        }
    }
}
