using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace XLY.SF.Project.Views.PreviewFile.LargeFileLoad
{
    /// <summary>
    /// richTextBoxUserControl.xaml 的交互逻辑
    /// 
    /// </summary>
    public partial class TextBoxUserControl : UserControl
    {
        public TextBoxUserControl()
        {
            InitializeComponent();
            _textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            richTextBox.FontFamily = new FontFamily("Courier New"); //设置等宽字体。可以不用。

            this.scrollBar.ValueChanged += ScrollBar_ValueChanged;
            this.PreviewMouseWheel += RichTextBox_PreviewMouseWheel;
            this.PreviewKeyDown += RichTextBox_PreviewKeyDown;
        }

        void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_disableScrollBarValueEvent ==true)
            {
                _disableScrollBarValueEvent = false;
                return;
            }
            
            //通知ContentScaleValue改变刻度值
            int fileOffset=(int)(scrollBar.Value/ (scrollBar.Maximum-scrollBar.Minimum)*_largeFile.FileContentLength);
            if (_presentBuffer.StartedOffset <= fileOffset
                && fileOffset <= _presentBuffer.StartedOffset + _presentBuffer.TotalLen)
            {
                double scale = (fileOffset - _presentBuffer.StartedOffset) / (double)_presentBuffer.TotalLen;
               richTextBox.ScrollToVerticalOffset(scale*richTextBox.ExtentHeight); 
            }
            else
            {
                fileOffset = fileOffset / _presentBuffer.TotalLen * _presentBuffer.TotalLen;
                _presentBuffer.Read(fileOffset);
                Text = _presentBuffer.Text;
                //一般情况下重新装载后，定位头部；在滚动条为100%时，定位到尾部
                if (Math.Abs(scrollBar.Value-scrollBar.Maximum) < ZeroConst)
                {
                    TextBoxScrollBarToEnd();
                }
                else
                {
                    TextBoxScrollBarToHome();
                }
            }
        }

        private PresentBuffer _presentBuffer;
        private LargeFile _largeFile;
        private readonly TextRange _textRange;
        //禁止ScrollBar值变化的事件
        private bool _disableScrollBarValueEvent;

        /// <summary>
        /// 内容
        /// </summary>
        private string Text { get { return _textRange.Text; } set { _textRange.Text = value; } }

        public void OpenFile(string path = @"C:\Users\litao\Pictures\盘龙gbk.txt")
        {
            _largeFile = new LargeFile(path);
            CacheBuffer cacheBuffer = new CacheBuffer(_largeFile);
            _presentBuffer = new PresentBuffer(cacheBuffer);
            _presentBuffer.Read(0);
            Text = _presentBuffer.Text;

        }

        /// <summary>
        /// TextBox定位到内容头部
        /// </summary>
        private void TextBoxScrollBarToHome()
        {
            richTextBox.ScrollToHome();
            richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentStart;
        }

        /// <summary>
        /// TextBxo定位到内容尾部
        /// </summary>
        private void TextBoxScrollBarToEnd()
        {
            richTextBox.ScrollToEnd();
            richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentEnd;
        }
        
        private void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down
                || e.Key == Key.PageDown)
            {
                OnContentScaleValueChanged(e,true);
            }
            else if (e.Key == Key.Up
                     || e.Key == Key.PageUp)
            {
                OnContentScaleValueChanged(e, false);
            }
            else if (e.Key == Key.Right
                && Math.Abs(richTextBox.CaretPosition.GetOffsetToPosition(richTextBox.Document.ContentEnd)) < 4)
            {
                OnContentScaleValueChanged(e, true);
            }
            else if (e.Key == Key.Left
                && Math.Abs(richTextBox.CaretPosition.GetOffsetToPosition(richTextBox.Document.ContentStart)) < 4)
            {
                OnContentScaleValueChanged(e, false);
            }
        }

        private void RichTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            OnContentScaleValueChanged(e, e.Delta < 0);//鼠标向下滚动delta为负数，向上滚动为负数
        }

        private const double ZeroConst = 0.0000000001;

        private void OnContentScaleValueChanged(RoutedEventArgs e, bool isScrollDown)
        {
            //翻Buffer
            bool isTurned = TurnPresentBuffer(isScrollDown);
            e.Handled = isTurned;

            //通知ScrollBar改变刻度值
            double verticalOffset = richTextBox.VerticalOffset;
            double viewportHeight = richTextBox.ViewportHeight;
            double extentHeight = richTextBox.ExtentHeight;
            double scaleValue = verticalOffset / (extentHeight - viewportHeight);
            int offset = (int)(scaleValue * _presentBuffer.TotalLen) + _presentBuffer.StartedOffset;
            scrollBar.Value = ((double)offset / _largeFile.FileContentLength) * (scrollBar.Maximum-scrollBar.Minimum);
            _disableScrollBarValueEvent = true;
        }

        /// <summary>
        /// 翻PresentBuffer
        /// </summary>
        private bool TurnPresentBuffer(bool isScrollDown)
        {
            double verticalOffset = richTextBox.VerticalOffset;
            double viewportHeight = richTextBox.ViewportHeight;
            double extentHeight = richTextBox.ExtentHeight;

            double headScale = verticalOffset / extentHeight;
            double tailScale = (verticalOffset + viewportHeight) / extentHeight;

            if (isScrollDown)
            {
                if (Math.Abs(tailScale - 1) < ZeroConst)
                {
                    if (_presentBuffer.StartedOffset + _presentBuffer.TotalLen >= _largeFile.FileContentLength)
                    {
                        return false;
                    }
                    _presentBuffer.ReadNext();
                    Text = _presentBuffer.Text;
                    TextBoxScrollBarToHome();
                    return true;
                }
            }
            else
            {
                if (Math.Abs(headScale - 0) < ZeroConst)
                {
                    if (_presentBuffer.StartedOffset ==0)
                    {
                        return false;
                    }
                    _presentBuffer.ReadLast();
                    Text = _presentBuffer.Text;
                    TextBoxScrollBarToEnd();
                    return true;
                }
            }

            return false;
        }
    }
}
