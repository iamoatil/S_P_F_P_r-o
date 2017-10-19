using System.Windows;
using System.Windows.Controls;
using XLY.SF.Project.Views.PreviewFile.FileDecode;

namespace XLY.SF.Project.Views.PreviewFile
{
    /// <summary>
    /// PreViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class PreViewControl : UserControl
    {
        public PreViewControl()
        {
            InitializeComponent();
        }

        FileDecoderCollection _decoderCollection = new FileDecoderCollection();

        public void ReplaceContent(string filePath)
        {
            FrameworkElement element= _decoderCollection.Decode(filePath);
            Preview.Content = element;
            _decoderCollection.BinaryFile.Decode(filePath);
            element = _decoderCollection.BinaryFile.Element;
            HexPreview.Content = element;
        }
    }    
}
