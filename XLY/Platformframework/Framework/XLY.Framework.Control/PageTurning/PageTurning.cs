using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XLY.Framework.Controls
{
    public class PageTurning : ItemsControl
    {
        #region Page元素定义

        public class PageElement : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            private bool _isSelected;

            public bool IsSelected
            {
                get { return _isSelected; }
                set { _isSelected = value; OnPropertyChanged("IsSelected"); }
            }
        }

        #endregion

        private ScrollViewer _sv_View;
        private TextBlock _tb_CurPage;
        public ObservableCollection<PageElement> PageIndexs { get; set; }
        private List<double> _pageOffset;

        static PageTurning()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PageTurning), new FrameworkPropertyMetadata(typeof(PageTurning)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PageIndexs = new ObservableCollection<PageElement>();
            _pageOffset = new List<double>();

            Button btn_left = this.Template.FindName("btn_Left", this) as Button;
            Button btn_Right = this.Template.FindName("btn_Right", this) as Button;
            var itemsPresenter = this.Template.FindName("items", this) as ItemsPresenter;
            _sv_View = this.Template.FindName("sv_View", this) as ScrollViewer;
            _tb_CurPage = this.Template.FindName("tb_CurPage", this) as TextBlock;
            var ic_Index = this.Template.FindName("ic_Index", this) as ItemsControl;

            ic_Index.AddHandler(RadioButton.CheckedEvent, new RoutedEventHandler(CheckedIndex));
            btn_left.Click += btn_left_Click;
            btn_Right.Click += btn_Right_Click;
            itemsPresenter.SizeChanged += itemsPresenter_SizeChanged;
        }

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            var a = newValue as System.Collections.Specialized.INotifyCollectionChanged;
            a.CollectionChanged += a_CollectionChanged;
        }

        void a_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TurnPage(0);
        }

        protected override void OnItemsPanelChanged(ItemsPanelTemplate oldItemsPanel, ItemsPanelTemplate newItemsPanel)
        {
            base.OnItemsPanelChanged(oldItemsPanel, newItemsPanel);
        }

        //选择进入对应页
        private void CheckedIndex(object sender, RoutedEventArgs e)
        {
            TurnPage(GetCurSelectedPageIndex());
        }

        void itemsPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //计算子元素大小
            if (this.ItemsSource != null)
            {
                PageIndexs.Clear();
                _pageOffset.Clear();
                var tmp = this.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
                if (tmp != null)
                {
                    _sv_View.Width = tmp.ActualWidth;
                    //计算总个数
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        PageIndexs.Add(new PageElement() { IsSelected = i == 0 });
                        _pageOffset.Add(tmp.ActualWidth * i);
                    }
                    TurnPage(0);
                }
            }
        }

        /// <summary>
        /// 跳转页面
        /// </summary>
        /// <param name="pageIndex">页面ID</param>
        private void TurnPage(int pageIndex)
        {
            if (pageIndex >= 0 && pageIndex < _pageOffset.Count)
            {
                _sv_View.ScrollToHorizontalOffset(_pageOffset[pageIndex]);
                _tb_CurPage.Text = (pageIndex + 1).ToString();
                PageIndexs[pageIndex].IsSelected = true;
            }
        }

        /// <summary>
        /// 获取当前选中的项
        /// </summary>
        /// <returns></returns>
        private int GetCurSelectedPageIndex()
        {
            var selectedItem = PageIndexs.FirstOrDefault((t) => t.IsSelected);
            if (selectedItem != null)
                return PageIndexs.IndexOf(selectedItem);
            return 0;
        }

        //左翻页
        void btn_left_Click(object sender, RoutedEventArgs e)
        {
            TurnPage(GetCurSelectedPageIndex() - 1);
        }

        //右翻页
        void btn_Right_Click(object sender, RoutedEventArgs e)
        {
            TurnPage(GetCurSelectedPageIndex() + 1);
        }

        #region 依赖属性定义

        public bool ShowPageNo
        {
            get { return (bool)GetValue(ShowPageNoProperty); }
            set { SetValue(ShowPageNoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowPageNo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowPageNoProperty =
            DependencyProperty.Register("ShowPageNo", typeof(bool), typeof(PageTurning), new PropertyMetadata(true));

        public Thickness PageNoPadding
        {
            get { return (Thickness)GetValue(PageNoPaddingProperty); }
            set { SetValue(PageNoPaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageNoPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageNoPaddingProperty =
            DependencyProperty.Register("PageNoPadding", typeof(Thickness), typeof(PageTurning), new PropertyMetadata(new Thickness(0)));

        public HorizontalAlignment PageNoHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(PageNoHorizontalAlignmentProperty); }
            set { SetValue(PageNoHorizontalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageNoHorizontalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageNoHorizontalAlignmentProperty =
            DependencyProperty.Register("PageNoHorizontalAlignment", typeof(HorizontalAlignment), typeof(PageTurning), new PropertyMetadata(HorizontalAlignment.Center));

        public VerticalAlignment PageNoVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(PageNoVerticalAlignmentProperty); }
            set { SetValue(PageNoVerticalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageNoVerticalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageNoVerticalAlignmentProperty =
            DependencyProperty.Register("PageNoVerticalAlignment", typeof(VerticalAlignment), typeof(PageTurning), new PropertyMetadata(VerticalAlignment.Center));

        #endregion
    }
}
