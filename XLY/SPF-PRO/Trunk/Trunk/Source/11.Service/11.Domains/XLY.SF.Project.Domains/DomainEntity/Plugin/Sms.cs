using System;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    public class Sms
    {
        /// <summary>
        /// 短信
        /// </summary>
        [Serializable]
        public class SMS : AbstractDataItem
        {
            /// <summary>
            /// 号码
            /// </summary>
            [Display]
            public string Number { get; set; }

            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 联系人全称
            /// </summary>
            [Display]
            public string ContactName
            {
                get { return String.IsNullOrWhiteSpace(this.Name) ? this.Number : string.Format("{0}({1})", this.Name, this.Number); }
            }

            /// <summary>
            /// 发送者姓名
            /// </summary> 
            public string SenderName { get { return String.IsNullOrWhiteSpace(this.ContactName) ? this.SmsStateDesc : string.Format("[{0}] ☞ {1}", this.SmsStateDesc, this.ContactName); } set { } }

            /// <summary>
            /// 发送者图片
            /// </summary>
            public string SenderImage { get; set; }

            /// <summary>
            /// 时间，可空
            /// </summary>
            
            public DateTime? Date { get { return this.StartDate; } set { } }

            /// <summary>
            /// 数据类型
            /// </summary>
            public EnumColumnType Type { get; set; }

            /// <summary>
            /// 短信内容
            /// </summary>
            [Display]
            public string Content { get; set; }

            /// <summary>
            /// 发送状态
            /// </summary>
            
            public EnumSendState SendState { get { return (this.SmsState == EnumSMSState.SendSMS ? EnumSendState.Send : EnumSendState.Receive); } set { } }

            /// <summary>
            /// 开始日期
            /// </summary>

            public DateTime? StartDate { get; set; }

            [Display]
            public string _StartDate
            {
                get { return this.StartDate.ToString(); }
            }

            /// <summary>
            /// 发送时间（接收短信）
            /// </summary>

            public DateTime? SentDate { get; set; }
            
            public string _SentDate
            {
                get { return this.SentDate.ToString(); }
            }

            /// <summary>
            /// 接收或者发送。
            /// </summary>
            public EnumSMSState SmsState { get; set; }

            /// <summary>
            /// 接收或者发送。
            /// </summary>
            [Display(Key = "SmsState")]
            public string SmsStateDesc
            {
                get { return this.SmsState.GetDescription(); }
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
            /// 号码归属地
            /// </summary>
            [Display]
            public string LocationInfo
            {
                get { return string.Format("{0}-{1} {2}", this.Province, this.City, this.Operator); }
            }

            /// <summary>
            /// 读取状态
            /// </summary>
            public EnumReadState ReadState { get; set; }

            /// <summary>
            /// 短信是否查看状态
            /// </summary>
            [Display]
            public string SmsReadState
            {
                get { return this.ReadState.GetDescription(); }
            }

            /// <summary>
            /// 读取短信的时间。（Ios系统特有）
            /// </summary>
            public DateTime? ReadTime { get; set; }

            //[Display(Width = 160,Text="读取时间")]
            public string _ReadTime
            {
                get { return this.ReadTime.ToString(); }
            }

            /// <summary>
            /// 其他备注
            /// </summary>
            [Display(Key = "RemarkInfo")]
            public string Remark { get; set; }

            /// <summary>
            /// 经度
            /// </summary>
            public double Longitude { get; set; }

            /// <summary>
            /// 维度
            /// </summary>
            public double Latitude { get; set; }

            #region SMS-构造函数（初始化）

            /// <summary>
            ///  SMS-构造函数（初始化）
            /// </summary>
            public SMS()
            {
                this.Type = EnumColumnType.String;
            }
            #endregion


            public object Clone()
            {
                return this.MemberwiseClone();
            }

            /// <summary>
            /// 详细信息
            /// </summary>
            public string DetailContent
            {
                get { return this.ToString(); }
            }

            public bool StrEquals(SMS obj)
            {
                return this.Number == obj.Number && this.StartDate.ToString() == obj.StartDate.ToString() && this.SmsState == obj.SmsState;
            }

        }
    }
}
