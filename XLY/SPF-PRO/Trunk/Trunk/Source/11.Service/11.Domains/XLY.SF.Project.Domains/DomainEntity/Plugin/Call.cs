using System;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 通话记录
    /// </summary>
    [Serializable]
    public class Call : AbstractDataItem
    {

        /// <summary>
        /// 联系人全称
        /// </summary>
        [Display]
        public string ContactName
        {
            get { return String.IsNullOrEmpty(this.Name) ? this.Number : string.Format("{0}({1})", this.Name, this.Number); }
        }

        /// <summary>
        /// 号码
        /// </summary>
        [Display]
        public string Number { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [Display]
        public DateTime? StartDate { get; set; }

        [Display]
        public string _StartDate
        {
            get { return this.StartDate.ToDateTimeString(); }
        }

        /// <summary>
        /// 持续秒数
        /// </summary>
        [Display]
        public int DurationSecond { get; set; }

        /// <summary>
        /// 持续时间格式化描述
        /// </summary>
        [Display]
        public string DurationSecondDesc 
        { 
            get 
            { 
                return FormatHelper.FormatSecond(this.DurationSecond); 
            } 
        }

        /// <summary>
        /// 号码归属地
        /// </summary>
        [Display]
        public string LocationInfo
        {
            get { return string.Format("{0}-{1} {2}", this.Province, this.City, this.Operator); }
        }

        /// <summary>
        /// 通话类型
        /// </summary>
        [Display]
        public EnumCallType Type { get; set; }

        /// <summary>
        /// 通话类型描述
        /// </summary>
        [Display]
        public string TypeDesc
        {
            //get { return XLY.SF.Framework.BaseUtility.TryExtension.GetDescriptionX(this.Type); }
            get { return this.Type.ToString(); }
        }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 运营商
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 动态类型扩展
        /// </summary>
        public DynamicEx Dynamic
        {
            get { return this._Dynamic; }
            set { this._Dynamic = value; }
        }
        private DynamicEx _Dynamic;

        #region Call-构造函数（初始化）

        /// <summary>
        ///  Call-构造函数（初始化）
        /// </summary>
        public Call()
        {
            this.Type = EnumCallType.None;
        }
        #endregion

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 维度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 最后联系时间
        /// </summary>
        public DateTime? LastContactDate { get; set; }
    }
}