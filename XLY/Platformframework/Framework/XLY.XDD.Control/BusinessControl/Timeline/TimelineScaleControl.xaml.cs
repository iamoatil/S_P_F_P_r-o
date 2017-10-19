using System;
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
using XLY.XDD.ForensicService;

namespace XLY.XDD.Control
{
    /// <summary>
    /// TimelineControl.xaml 的交互逻辑
    /// </summary>
    public partial class TimelineScaleControl : UserControl
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public IEnumerable<BaseTimelineItem> DataSource { set { this.treeList.ItemsSource = value; } }

        public static readonly DependencyProperty TimelineItemTemplateSelectorProperty =
            DependencyProperty.Register("TimelineItemTemplateSelector", typeof(DataTemplateSelector), typeof(TimelineScaleControl), new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>
        /// 数据项模板选择器
        /// </summary>
        public DataTemplateSelector TimelineItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(TimelineItemTemplateSelectorProperty); }
            set { SetValue(TimelineItemTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(int), typeof(TimelineScaleControl), new PropertyMetadata(240));

        /// <summary>
        /// 数据项宽度：表格第二列宽度
        /// </summary>
        public int ItemWidth
        {
            get { return (int)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public TimelineScaleControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当前选中的时间轴项
        /// </summary>
        public BaseTimelineItem SelectedTimelineItem
        {
            get
            {
                if (this.treeList.SelectedItem == null) return null;
                TreeListNode node = this.treeList.View.GetNodeByContent(this.treeList.SelectedItem);
                return this.treeList.SelectedItem as BaseTimelineItem;
            }
        }

        /// <summary>
        /// 展开、收缩所有节点，expand=true展开；expand=false收缩节点
        /// </summary>
        /// <param name="expand"></param>
        public void ExpandAll(bool expand = true)
        {
            var source = this.treeList.ItemsSource;
            if (source == null) return;
            var items = source as IEnumerable<BaseTimelineItem>;
            if (items.IsInvalid()) return;
            foreach (var y in items)
            {
                y.IsExpanded = expand;
                if (y.Items.IsValid())
                {
                    y.Items.ForEach(s => s.IsExpanded = expand);
                }
            }
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.treeList.SelectedItem == null) return;
            TreeListNode node = this.treeList.View.GetNodeByContent(this.treeList.SelectedItem);
            if (node != null)
            {
                node.IsExpanded = !node.IsExpanded;
                (this.treeList.SelectedItem as BaseTimelineItem).IsExpanded = node.IsExpanded;
            }
        }
    }
}
