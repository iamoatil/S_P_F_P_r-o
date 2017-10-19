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
using XLY.XDD.ForensicService;

namespace XLY.XDD.Control
{
    /// <summary>
    /// TimelineControl.xaml 的交互逻辑
    /// </summary>
    public partial class TimelineControl : UserControl
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public IEnumerable<BaseTimelineItem> DataSource { set { this.timelineScaleControl1.DataSource = value; } }

        /// <summary>
        /// 时间轴项点击事件委托
        /// </summary>
        /// <param name="item"></param>
        public delegate void TimelineItemClickEventHandler(IEnumerable<IDateTime> objs);
        /// <summary>
        /// 当时间轴项被点击时发生
        /// </summary>
        public TimelineItemClickEventHandler OnTimelineItemClick;

        public TimelineControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 展开、收缩所有节点，expand=true展开；expand=false收缩节点
        /// </summary>
        /// <param name="expand"></param>
        public void ExpandAll(bool expand = true)
        {
            this.timelineScaleControl1.ExpandAll(expand);
        }

        private void btnItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.timelineScaleControl1.SelectedTimelineItem == null) return;
            TimelineItem item = this.timelineScaleControl1.SelectedTimelineItem as TimelineItem;
            if (this.OnTimelineItemClick != null)
            {
                this.OnTimelineItemClick(item.DataSource);
            }
        }
    }
}
