using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace XLY.XDD.Control.MultiMedia
{
    /// <summary>
    /// 长图表情播放控件：
    /// 此控件用于将一张高为宽的N倍的图片，以宽为边长的正方形截取图片帧。进行播放图片，使得展示为表情。
    /// </summary>
    [TemplatePart(Name = "Part_img", Type = typeof(Image))]
    public class LongImageExpression : System.Windows.Controls.Control
    {
        #region 私有属性

        private DispatcherTimer timer;
        private BitmapImage _bigImage;
        private int _currentIndex = 1;
        private int _imgWidth = 0;
        private int _imgHeight = 0;
        private int _totalImgCount = 1;
        private BitmapFrame _temp;
        private Dictionary<int, CroppedBitmap> dic = new Dictionary<int, CroppedBitmap>();
        private CroppedBitmap tempCB = null;
        /// <summary>
        /// 模板中必须有的图片控件。
        /// </summary>
        private Image PartImg { get; set; }

        #endregion

        #region 静态构造函数，默认构造函数和私有方法

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static LongImageExpression()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LongImageExpression), new FrameworkPropertyMetadata(typeof(LongImageExpression)));
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LongImageExpression()
        {
            bool isDesignModel = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
            if (!isDesignModel)
                this.Loaded += LongImageExpression_Loaded;
        }

        /// <summary>
        /// 应用模板
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PartImg = this.Template.FindName("Part_img", this) as Image;
        }

        /// <summary>
        /// 加载完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LongImageExpression_Loaded(object sender, RoutedEventArgs e)
        {
            if (timer == null)
                timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds((1000 / FrameSec)) };

            if (string.IsNullOrEmpty(ImageFullPath) || !(new System.IO.FileInfo(ImageFullPath)).Exists)
                throw new ArgumentException("没有设置图片地址或图片不存在！");

            _bigImage = new BitmapImage(new Uri(ImageFullPath));
            _imgWidth = (int)_bigImage.Width;
            _imgHeight = (int)_bigImage.Height;
            _totalImgCount = _imgHeight / _imgWidth;
            if (_totalImgCount < 2)
                throw new ArgumentException("此控件只能播放一张图片上的多个不同位置的正方形图片");

            timer.Tick += ChangeImageSource;
            if (JudgmentParent != null)
            {
                JudgmentParent.ScrollChanged += JudgmentParent_ScrollChanged;
                JudgmentParent_ScrollChanged(this,null);// 默认调用一次，以免没有触发滚动事件，就没法默认显示。
            }
            else
            {
                timer.Start();
            }
        }

        private void ChangeImageSource(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ImageFullPath))
                    return;

                if (_totalImgCount <= _currentIndex)
                    _currentIndex = 1;

                this.PartImg.Source = GetPartImage(ImageFullPath, 0, (_imgWidth * _currentIndex), _imgWidth, _imgWidth);

                _currentIndex = _currentIndex + 1;
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 控制在ScrollViewer中是否启动播放(当此表情不可见时，不再去消耗资源！)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JudgmentParent_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (timer != null)
            {
                if (this.IsUserVisible(JudgmentParent))
                {
                    //  当前控件在父级Scroll中可见。
                    if (!this.timer.IsEnabled)
                        timer.Start();
                }
                else
                {
                    //  当前控件在父级Scroll中不可见。
                    timer.Stop();
                }
            }
        }

        /// <summary>
        /// 获取一张图片中的一部分 
        /// </summary>
        /// <param name="ImgUri">图片路径</param>
        /// <param name="XCoordinate">要截取部分的X坐标</param>
        /// <param name="YCoordinate">要截取部分的Y坐标</param>
        /// <param name="Width">截取的宽度</param>
        /// <param name="Height">截取的高度</param>
        /// <returns></returns>
        private BitmapSource GetPartImage(string ImgUri, int XCoordinate, int YCoordinate, int Width, int Height)
        {
            if (_temp == null)
                _temp = BitmapFrame.Create(new Uri(ImgUri, UriKind.RelativeOrAbsolute));

            if (dic.ContainsKey(_currentIndex))
            {
                if (dic.TryGetValue(_currentIndex, out tempCB))
                    return tempCB;
            }

            tempCB = new CroppedBitmap(_temp, new Int32Rect(XCoordinate, YCoordinate, Width, Height));
            dic[_currentIndex] = tempCB;
            return tempCB;
        }

        #endregion

        #region 公开属性、依赖属性

        public string ImageFullPath
        {
            get { return (string)GetValue(ImageFullPathProperty); }
            set { SetValue(ImageFullPathProperty, value); }
        }


        // 指定要播放的图片的路径。
        public static readonly DependencyProperty ImageFullPathProperty =
            DependencyProperty.Register("ImageFullPath", typeof(string), typeof(LongImageExpression), new PropertyMetadata(null));



        /// <summary>
        /// 用于判断这个表情控件在这个父类中是否可见的父级ScrollViewer控件。
        /// </summary>
        public ScrollViewer JudgmentParent
        {
            get { return (ScrollViewer)GetValue(JudgmentParentProperty); }
            set { SetValue(JudgmentParentProperty, value); }
        }

        // 判断是否可见的父级ScrollViewer控件
        public static readonly DependencyProperty JudgmentParentProperty =
            DependencyProperty.Register("JudgmentParent", typeof(ScrollViewer), typeof(LongImageExpression), new PropertyMetadata(null));


        /// <summary>
        /// 动画多少帧/秒
        /// </summary>
        public int FrameSec
        {
            get { return (int)GetValue(FrameSecProperty); }
            set { SetValue(FrameSecProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrameSec.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrameSecProperty =
            DependencyProperty.Register("FrameSec", typeof(int), typeof(LongImageExpression), new PropertyMetadata(24));

        #endregion
    }
}
