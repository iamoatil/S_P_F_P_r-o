using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Grid;

namespace XLY.XDD.Control
{
    /// <summary>
    /// ThumbnailViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ThumbnailViewer : UserControl
    {
        /// <summary>
        /// 标示拖动滑动条是内容是否静止，=ScrollViewer.IsDeferredScrollingEnabled
        /// </summary>
        public static readonly DependencyProperty IsDeferredScrollingEnabledProperty =
            DependencyProperty.Register("IsDeferredScrollingEnabled", typeof(bool), typeof(ThumbnailViewer), new PropertyMetadata(default(bool)));
        /// <summary>
        /// 标示拖动滑动条是内容是否静止，=ScrollViewer.IsDeferredScrollingEnabled
        /// </summary>
        public bool IsDeferredScrollingEnabled
        {
            get { return (bool)GetValue(IsDeferredScrollingEnabledProperty); }
            set { SetValue(IsDeferredScrollingEnabledProperty, value); }
        }

        /// <summary>
        /// 缩略图项的模板，默认提供的模板只支持FileX数据集
        /// </summary>
        public static readonly DependencyProperty CardTemplateProperty =
            DependencyProperty.Register("CardTemplate", typeof(DataTemplate), typeof(ThumbnailViewer), new PropertyMetadata(default(DataTemplate)));
        /// <summary>
        /// 缩略图项的模板，默认提供的模板只支持FileX数据集
        /// </summary>
        public DataTemplate CardTemplate
        {
            get { return (DataTemplate)GetValue(CardTemplateProperty); }
            set { SetValue(CardTemplateProperty, value); }
        }

        public SelectedItemChangedEventHandler CardItemSelected;

        /// <summary>
        /// 数据集合
        /// </summary>
        public IEnumerable ItemsSource
        {
            set { this.gridList.ItemsSource = value; }
        }

        /// <summary>
        /// 当前列表选中的项
        /// </summary>
        public object SelectedItem
        {
            get { return this.gridList.SelectedItem; }
            set { this.gridList.SelectedItem = value; }
        }

        /// <summary>
        /// 当前列表选中的项集合
        /// </summary>
        public IList SelectedItems
        {
            get { return this.gridList.SelectedItems; }
        }

        public ThumbnailViewer()
        {
            InitializeComponent();
            this.CardTemplate = this.FindResource("defaultCardTemplate") as DataTemplate;
        }

        private void gridList_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.CardItemSelected != null)
            {
                this.CardItemSelected(sender, e);
            }
        }
    }
}
