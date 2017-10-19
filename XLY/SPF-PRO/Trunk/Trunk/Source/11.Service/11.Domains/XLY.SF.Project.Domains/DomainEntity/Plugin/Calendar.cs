using System;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 日历
    /// </summary>
    public class Calendar:AbstractDataItem
    {
        /// <summary>
        /// 事件标题
        /// </summary>
        [Display]
        public string Title { get; set; }

        /// <summary>
        /// 事件地点
        /// </summary>
        [Display]
        public string EventLocation { get; set; }

        /// <summary>
        /// 对事件的描述
        /// </summary>
        [Display]
        public string Description { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display]
        public DateTime? DtStart { get; set; }

        [Display]
        public string _DtStart
        {
            get { return this.DtStart.ToDateTimeString(); }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display]
        public DateTime? DtEnd { get; set; }

        [Display]
        public string _DtEnd
        {
            get { return this.DtEnd.ToDateTimeString(); }
        }

        /// <summary>
        /// URL链接
        /// </summary>
        [Display]
        public string Url { get; set; }

        /// <summary>
        /// 持续时间（日历开始时间到日历结束时间）
        /// </summary>
        [Display]
        public string Duration { get; set; }

        /// <summary>
        /// 日历提醒时间（分）
        /// </summary>
        public string Minutes { get; set; }

        public string EventTimeZone { get; set; }

    }
}
