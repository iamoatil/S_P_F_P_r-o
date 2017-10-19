using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace XLY.XDD.ForensicService
{
    /// <summary>
    /// 标准基于接口IDateTime的时间轴项
    /// </summary>
    public class TimelineItem : BaseTimelineItem
    {
        /// <summary>
        /// 数据源集合
        /// </summary>
        public ICollection<IDateTime> DataSource { get; set; }

        /// <summary>
        /// 包含的总数
        /// </summary>
        public int Total { get { return this.DataSource.IsValid() ? this.DataSource.Count : 0; } }

        #region TimelineItem-构造函数（初始化）

        /// <summary>
        ///  TimelineItem-构造函数（初始化）
        /// </summary>
        public TimelineItem(int value, EnumDateTime type)
        {
            this.DataSource = new Collection<IDateTime>();
            this.Type = type;
            this.Value = value;
            this.Text = string.Concat(value, this.Type.GetDescription());
        }

        #endregion

        /// <summary>
        /// 添加数据项
        /// </summary>
        public virtual void Add(IDateTime obj)
        {
            if (!this.Contain(obj)) return;
            this.DataSource.Add(obj);
            //add child items
            switch (this.Type)
            {
                case EnumDateTime.Year:
                    var mitme = this.AddChildItem(obj.DateTime.Value.Month, EnumDateTime.Month);
                    mitme.Add(obj);
                    break;
                case EnumDateTime.Month:
                    var ditme = this.AddChildItem(obj.DateTime.Value.Day, EnumDateTime.Day);
                    ditme.Add(obj);
                    break;
            }
        }

        protected virtual TimelineItem AddChildItem(int value, EnumDateTime type)
        {
            if (this.Items.IsInvalid()) this.Items = new List<BaseTimelineItem>();
            var citem = this.Items.FirstOrDefault(s => s.Value == value);
            if (citem == null)
            {
                citem = new TimelineItem(value, type);
                this.Items.Add(citem);
                this.Items = this.Items.OrderBy(s => s.Value).ToList();
            }
            return citem as TimelineItem;
        }

        /// <summary>
        /// 验证数据项是否可以包含在该时间轴内.
        /// true表示可以包含在内
        /// </summary>
        public virtual bool Contain(IDateTime obj)
        {
            if (!obj.DateTime.HasValue) return false;
            switch (this.Type)
            {
                case EnumDateTime.Year:
                    return this.Value == obj.DateTime.Value.Year;
                case EnumDateTime.Month:
                    return this.Value == obj.DateTime.Value.Month;
                case EnumDateTime.Day:
                    return this.Value == obj.DateTime.Value.Day;
            }
            return false;
        }
    }
}