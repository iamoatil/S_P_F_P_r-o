using System.Windows;
using XLY.SF.Project.Views.PreviewFile.LargeFileLoad;

namespace XLY.SF.Project.Views.PreviewFile.Decoders
{
    class TextFileDecoder : IFileDecoder
    {
        public FrameworkElement Element { get { return _textBox; } }
        private readonly TextBoxUserControl _textBox = new TextBoxUserControl();

        public void Decode(string path)
        {
            _textBox.OpenFile(path);
        }
    }
}
