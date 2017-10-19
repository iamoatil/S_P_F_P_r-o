using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 此实体类专为黑莓PhoneCallLogs信息提取服务
    /// @Ahthor luochao
    /// @Date 20160720
    /// @Copy XLY
    /// </summary>
    public class PhoneCallLogs
    {
        public PhoneCallLogs()
        {
            this.Type = EnumCallType.None;
        }
        /// <summary>
        /// 号码
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        public string _StartDate
        {
            get { return this.StartDate.ToString(); }
        }

        /// <summary>
        /// 持续秒数
        /// </summary>
        public int DurationSecond { get; set; }

        /// <summary>
        /// 通话类型
        /// </summary>
        public EnumCallType Type
        {
            get
            {
                /**
                 * 呼入
                 * 呼出
                 * 呼入未接
                 * 呼出未接
                 * 未知
                 * 
                 * Type_0D：
                 *    Type_02：
                 * 00 呼出
                 * 02 呼出 空号
                 *    01 呼出
                 * 01 呼入
                 * 03 呼入
                 *    00 已接
                 *    02 未接（未查看）
                 *    03 未接（已打开）
                 * 
                 * */
                if (Type_0D == "00" || Type_0D == "02")        //呼出
                {
                    return this.DurationSecond > 0 ? EnumCallType.CallOut : EnumCallType.MissedCallOut;
                }
                else if (Type_0D == "01" || Type_0D == "03")   //呼入
                {
                    if (Type_02 == "00" && this.DurationSecond > 0)
                        return EnumCallType.CallIn;
                    else if (Type_02 == "02")
                        return EnumCallType.MissedCallInNotViewed;
                    else if (Type_02 == "03")
                        return EnumCallType.MissedCallInAlreadyOpen;
                }
                return EnumCallType.None;
            }
            set { }
        }

        /// <summary>
        /// 00 呼出
        /// 03 呼入
        /// </summary>
        public string Type_0D { get; set; }

        /// <summary>
        /// 00 已接
        /// 01 呼出
        /// 02 未接（未查看）
        /// 03 未接（已打开）
        /// </summary>
        public string Type_02 { get; set; }
    }
}
