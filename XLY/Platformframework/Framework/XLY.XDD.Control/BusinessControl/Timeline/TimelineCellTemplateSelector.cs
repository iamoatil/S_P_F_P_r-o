using System;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using XLY.XDD.ForensicService;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 时间轴刻度单元模板选择器
    /// </summary>
    public class TimelineCellTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item,
                                                                   System.Windows.DependencyObject container)
        {
            EditGridCellData cell = item as EditGridCellData;
            BaseTimelineItem titem = cell.RowData.Row as BaseTimelineItem;
            if (titem == null) return null;
            FrameworkElement element = container as FrameworkElement;
            DataTemplate dt = null;
            switch (titem.Type)
            {
                case EnumDateTime.Year:
                    dt = element.FindResource("YearItem") as DataTemplate;
                    break;
                case EnumDateTime.Month:
                    dt = element.FindResource("MonthItem") as DataTemplate;
                    break;
                case EnumDateTime.Day:
                    dt = element.FindResource("DayItem") as DataTemplate;
                    break;
            }
            return dt;
        }
    }

    /// <summary>
    /// 时间轴一般数据单元模板选择器
    /// </summary>
    public class TimelineItemTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            EditGridCellData cell = item as EditGridCellData;
            BaseTimelineItem titem = cell.RowData.Row as BaseTimelineItem;
            if (titem == null) return null;
            FrameworkElement element = container as FrameworkElement;
            DataTemplate dt = null;
            switch (titem.Type)
            {
                case EnumDateTime.Year:
                    dt = element.FindResource("Year") as DataTemplate;
                    break;
                case EnumDateTime.Month:
                    dt = element.FindResource("Month") as DataTemplate;
                    break;
                case EnumDateTime.Day:
                    dt = element.FindResource("Day") as DataTemplate;
                    break;
            }
            return dt;
        }
    }

    /// <summary>
    /// 时间轴FileX数据单元模板选择器
    /// </summary>
    public class TimelineItemFileXTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            EditGridCellData cell = item as EditGridCellData;
            BaseTimelineItem titem = cell.RowData.Row as BaseTimelineItem;
            if (titem == null) return null;
            FrameworkElement element = container as FrameworkElement;
            DataTemplate dt = null;
            switch (titem.Type)
            {
                case EnumDateTime.Year:
                    dt = element.FindResource("YearFileX") as DataTemplate;
                    break;
                case EnumDateTime.Month:
                    dt = element.FindResource("MonthFileX") as DataTemplate;
                    break;
                case EnumDateTime.Day:
                    dt = element.FindResource("DayFileX") as DataTemplate;
                    break;
            }
            return dt;
        }
    }
}