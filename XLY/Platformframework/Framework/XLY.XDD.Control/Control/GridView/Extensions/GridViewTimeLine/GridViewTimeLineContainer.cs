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

namespace XLY.XDD.Control
{
    /// <summary>
    /// 时间轴容器
    /// </summary>
    public class GridViewTimeLineContainer : System.Windows.Controls.Control
    {
        static GridViewTimeLineContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridViewTimeLineContainer), new FrameworkPropertyMetadata(typeof(GridViewTimeLineContainer)));
        }

        #region Time -- 时间

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? Time
        {
            get { return (DateTime?)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Time.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(DateTime?), typeof(GridViewTimeLineContainer), new UIPropertyMetadata(null));

        #endregion

        #region Icon -- 图标

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(GridViewTimeLineContainer), new UIPropertyMetadata(null));

        #endregion

        #region Title -- 标题

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(GridViewTimeLineContainer), new UIPropertyMetadata(null));

        #endregion

        #region IsDelete -- 是否是删除数据

        /// <summary>
        /// 是否是删除数据
        /// </summary>
        public bool IsDelete
        {
            get { return (bool)GetValue(IsDeleteProperty); }
            set { SetValue(IsDeleteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDelete.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDeleteProperty =
            DependencyProperty.Register("IsDelete", typeof(bool), typeof(GridViewTimeLineContainer), new UIPropertyMetadata(false));

        #endregion

        #region Summary -- 摘要

        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary
        {
            get { return (string)GetValue(SummaryProperty); }
            set { SetValue(SummaryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Summary.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SummaryProperty =
            DependencyProperty.Register("Summary", typeof(string), typeof(GridViewTimeLineContainer), new UIPropertyMetadata(null));

        #endregion
    }
}
