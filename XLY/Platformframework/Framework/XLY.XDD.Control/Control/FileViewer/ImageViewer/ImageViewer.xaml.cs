using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.IconLib;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageGlass.Core;
using ImageGlass.Services.Configuration;
using Image = System.Drawing.Image;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Path = System.IO.Path;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using UserControl = System.Windows.Controls.UserControl;

namespace XLY.XDD.Control
{
    /// <summary>
    /// ImageViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ImageViewer : UserControl, IFileViewer
    {
        public ImageViewer()
        {
            InitializeComponent();
        }

        private void NextPic(string path)
        {
            if (GlobalSetting.ImageList.Length < 1)
                return;

            //The image data will load
            Image im = null;
            try
            {
                //Check if the image is a icon or not
                if (Path.GetExtension(path).ToLower() == ".ico")
                {
                    try
                    {
                        MultiIcon mIcon = new MultiIcon();
                        mIcon.Load(path);

                        //Try to get the largest image of it
                        SingleIcon sIcon = mIcon[0];
                        IconImage iImage = sIcon.OrderByDescending(ico => ico.Size.Width).ToList()[0];

                        //Convert to bitmap
                        im = iImage.Icon.ToBitmap();
                    }
                    catch //If a invalid icon
                    {
                        im = GlobalSetting.ImageList.GetImage(GlobalSetting.CurrentIndex);

                    }
                }
                else //If a normal image
                {
                    im = GlobalSetting.ImageList.GetImage(GlobalSetting.CurrentIndex);
                    var canPlay = ImageAnimator.CanAnimate(im);
                    var isAnime = System.Utility.Helper.File.GetExtension(path).ToLower() == "gif";
                    if (canPlay && !isAnime)
                        return;
                }
                GlobalSetting.IsImageError = GlobalSetting.ImageList.imgError;
                //Show image
                picturePanel.Zoom = 100;
                picturePanel.Image = im;
            }
            catch//(Exception ex)
            {
                picturePanel.Image = null;
            }

            if (GlobalSetting.IsImageError)
            {
                picturePanel.Image = null;
            }

            //Collect system garbage
            System.GC.Collect();
        }

        private readonly System.Drawing.Imaging.PixelFormat[] indexedPixelFormats = {
                                                               System.Drawing.Imaging.PixelFormat.Undefined,
                                                               System.Drawing.Imaging.PixelFormat.DontCare,
                                                               System.Drawing.Imaging.PixelFormat.Format16bppArgb1555,
                                                               System.Drawing.Imaging.PixelFormat.Format1bppIndexed,
                                                               System.Drawing.Imaging.PixelFormat.Format4bppIndexed,
                                                               System.Drawing.Imaging.PixelFormat.Format8bppIndexed
                                                            };
        /// <summary>
        /// 判断图片的PixelFormat 是否在 引发异常的 PixelFormat 之中
        /// </summary>
        /// <param name="imgPixelFormat">原图片的PixelFormat</param>
        /// <returns></returns>
        private bool IsPixelFormatIndexed(PixelFormat imgPixelFormat)
        {
            return indexedPixelFormats.Any(n => n.Equals(imgPixelFormat));
        }

        #region IFileViewer

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public void Open(string path)
        {
            var paths = new List<string> { path };
            //Declare a new list to store filename
            GlobalSetting.ImageFilenameList = paths;
            //Dispose all garbage
            if (null != GlobalSetting.ImageList)
                GlobalSetting.ImageList.Dispose();
            //Set filename to image list
            GlobalSetting.ImageList = new ImgMan(paths.ToArray());
            GlobalSetting.CurrentIndex = 0;
            this.NextPic(path);
            this.OpenArgs = path;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(System.IO.Stream stream, string extension)
        {
            this.OpenArgs = stream;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="buffer">文件Buffer</param>
        /// <param name="extension">要打开的文件扩展名</param>
        public void Open(byte[] buffer, string extension)
        {
            this.OpenArgs = buffer;

            MemoryStream ms = new MemoryStream(buffer);
            ms.Position = 0;
            Image img = Image.FromStream(ms);
            ms.Close();
            picturePanel.Zoom = 100;
            picturePanel.Image = img;

            //Collect system garbage
            System.GC.Collect();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="args">打开文件参数</param>
        public void Open(IFileViewerArgs args)
        {
            if (args.Type == FileViewerArgsType.Path)
                this.Open(args.Path);
            else if (args.Type == FileViewerArgsType.Stream)
                this.Open(args.Stream, args.Extension);
            else if (args.Type == FileViewerArgsType.Buffer)
                this.Open(args.Buffer, args.Extension);
            this.OpenArgs = args;
        }

        /// <summary>
        /// 打开参数
        /// </summary>
        public object OpenArgs { get; set; }

        /// <summary>
        /// 关闭文件
        /// </summary>
        public void Close()
        {
            if (null != this.OpenArgs)
            {
                if (this.OpenArgs is Stream)
                    (this.OpenArgs as Stream).Dispose();

                this.OpenArgs = null;
            }

            if (null != GlobalSetting.ImageFilenameList)
                GlobalSetting.ImageFilenameList.Clear();

            if (null != GlobalSetting.ImageList)
            {
                GlobalSetting.ImageList.Dispose();
                GlobalSetting.ImageList = null;
            }

            if (null != picturePanel.Image)
                picturePanel.Image.Dispose();

            picturePanel.Image = null;
        }

        /// <summary>
        /// 获取选中的文本内容
        /// </summary>
        /// <returns>当前选中的内容文本</returns>
        public string GetSelectionString()
        {
            return string.Empty;
        }

        /// <summary>
        /// 拷贝选中的内容
        /// </summary>
        public void Copy()
        {

        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {

        }

        /// <summary>
        /// 是否支持该文件后缀
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns>是否支持该文件后缀</returns>
        public bool IsSupport(string extension)
        {
            return FileViewerConfig.Config.IsSupport(FileViewerType.Image, extension);
        }

        /// <summary>
        /// 视图类型
        /// </summary>
        public FileViewerType ViewerType
        {
            get { return FileViewerType.Image; }
        }

        #endregion

        private void Fangda_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.picturePanel.Image == null)
            {
                return;
            }

            this.picturePanel.ZoomIn();
        }

        private void Suoxiao_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.picturePanel.Image == null)
            {
                return;
            }

            this.picturePanel.ZoomOut();
        }

        private void shunxuanzhuanXuanzhuan_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.picturePanel.Image == null)
            {
                return;
            }

            this.picturePanel.Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
            //this.picturePanel.Image.RotateFlip(RotateFlipType.Rotate90FlipX);
            this.picturePanel.Zoom += 1;
            this.picturePanel.Zoom -= 1;
        }

        private void Nishixuanzhuan_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.picturePanel.Image == null)
            {
                return;
            }

            this.picturePanel.Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
            this.picturePanel.Zoom += 1;
            this.picturePanel.Zoom -= 1;
        }

        private void PicturePanel_OnMouseEnter(object sender, EventArgs e)
        {
            if (this.picturePanel.Image == null)
            {
                return;
            }

            picturePanel.Focus();
        }
    }
}
