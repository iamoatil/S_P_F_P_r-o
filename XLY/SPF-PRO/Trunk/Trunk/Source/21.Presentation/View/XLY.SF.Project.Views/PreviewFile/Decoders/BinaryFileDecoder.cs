using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XLY.SF.Project.Views.PreviewFile.Decoders
{
    class BinaryFileDecoder : IFileDecoder
    {
        public FrameworkElement Element { get { return _textBox; } }
        private TextBox _textBox = new TextBox() { TextWrapping = TextWrapping.Wrap, IsReadOnly = true, FontFamily = new FontFamily("Courier New") };

        /// <summary>
        /// 解码内容到属性Element上。
        /// </summary>
        public async void Decode(string path)
        {
            Byte[] bytes = new byte[1 * 1024 * 1024];            
            StringBuilder textStringBuilder = new StringBuilder();

            using (FileStream fs = new FileStream(path,FileMode.Open,FileAccess.Read, FileShare.Read))
            {
                while(true)
                {
                    int count = await fs.ReadAsync(bytes, 0, bytes.Length);   
                    textStringBuilder.Append(BytesToHexString(bytes,count));

                    if (count < bytes.Length)
                    {
                        break;
                    }
                }                
            }

            _textBox.Text = textStringBuilder.ToString();
        }

        /// <summary>
        /// Byte数组转换为二进制字符串
        /// </summary>
        /// <returns></returns>
        private string BytesToBinString(byte[] bytes,int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                stringBuilder.Append(Convert.ToString(bytes[i], 2).PadLeft(8, '0')+ " ");
            } 
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Byte数组转化为16进制字符串
        /// </summary>
        /// <returns></returns>
        private string BytesToHexString(byte[] bytes, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                stringBuilder.Append(Convert.ToString(bytes[i], 16).PadLeft(8, '0') + " ");
            }
            return stringBuilder.ToString();
        }
    }
}
