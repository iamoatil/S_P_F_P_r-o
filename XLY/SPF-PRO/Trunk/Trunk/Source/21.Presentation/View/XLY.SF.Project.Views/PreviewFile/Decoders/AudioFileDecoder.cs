using System;
using System.Windows;
using System.Windows.Controls;

namespace XLY.SF.Project.Views.PreviewFile.Decoders
{
    class AudioFileDecoder :IFileDecoder
    {
        public  FrameworkElement Element
        {
            get
            {
                return mediaElement;
            }
        }        

        MediaElement mediaElement = new MediaElement();        

        public void Decode(string path)
        {
            mediaElement.Source = new Uri(path);
            
        }        
    }
}
