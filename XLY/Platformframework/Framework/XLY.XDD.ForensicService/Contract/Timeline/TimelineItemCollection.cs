using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace XLY.XDD.ForensicService
{
    /// <summary>
    /// 标准基于接口IDateTime的时间轴项 集合。通过构造函数可直接把数据集转换为时间轴集合
    /// </summary>
    public class TimelineItemCollection : List<TimelineItem>
    {
        #region TimelineItems-构造函数（初始化）

        /// <summary>
        ///  TimelineItems-构造函数（初始化）
        /// </summary>
        public TimelineItemCollection(IEnumerable<IDateTime> objs)
        {
            if (objs.IsInvalid()) return;
            var vobjs = objs.Where(s => s.DateTime.HasValue);
            var min = vobjs.Min(s => s.DateTime);
            var max = vobjs.Max(s => s.DateTime);

            //generate items
            for (int i = min.Value.Year; i <= max.Value.Year; i++)
            {
                var item = new TimelineItem(i, EnumDateTime.Year);
                this.Add(item);
            }
            //analysis files
            foreach (var obj in objs)
            {
                var yitems = this.Where(s => s.Contain(obj));
                foreach (var yitem in yitems)
                {
                    yitem.Add(obj);
                }
            }
            //remove empty year
            this.RemoveAll(s => s.Total == 0);
        }

        #endregion
    }
}