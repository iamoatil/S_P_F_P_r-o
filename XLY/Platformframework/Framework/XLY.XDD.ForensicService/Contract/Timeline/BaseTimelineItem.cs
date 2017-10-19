using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace XLY.XDD.ForensicService
{
    /// <summary>
    /// 时间轴抽象基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseTimelineItem : BaseNotifyPropertyChanged
    {
        /// <summary>
        /// 界面显示的文本内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 时间值
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 时间值类型
        /// </summary>
        public EnumDateTime Type { get; set; }

        /// <summary>
        /// 时间轴子项集合
        /// </summary>
        public IList<BaseTimelineItem> Items { get; set; }

        private bool _IsExpanded;
        /// <summary>
        /// 是否已展开
        /// </summary>
        public bool IsExpanded
        {
            get { return this._IsExpanded; }
            set { this._IsExpanded = value; this.OnPropertyChanged("IsExpanded"); }
        }

        #region BaseTimelineItem-构造函数（初始化）

        /// <summary>
        ///  BaseTimelineItem-构造函数（初始化）
        /// </summary>
        public BaseTimelineItem()
        {
            this.IsExpanded = true;
        }

        #endregion
    }
}