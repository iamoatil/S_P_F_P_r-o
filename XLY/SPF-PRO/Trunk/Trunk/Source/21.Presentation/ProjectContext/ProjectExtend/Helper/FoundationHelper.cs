using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/*
 * 创建人：Bob
 * 创建时间：2017/9/19
 * 
 * 说明：基础帮助服务
 * 
 * 目前内容：
 *      1.控件截图功能
 * 
 */

namespace XLY.SF.Project.Extension.Helper
{
    public class FoundationHelper
    {
        /// <summary>
        /// 将当前控件内容转换为图片字节流
        /// </summary>
        public Stream PrintscreenControlByBinary(FrameworkElement control)
        {
            if (double.IsNaN(control.Width) && double.IsNaN(control.Height))
            {
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)control.Width, (int)control.Height, 69, 69, PixelFormats.Default);
                rtb.Render(control);
                using (MemoryStream ms = new MemoryStream())
                {
                    BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    encoder.Save(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;
                }
            }
            return Stream.Null;
        }
    }
}
