using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 开关机时间
    /// </summary>
    [Serializable]
    public class SwitchTimeInfo : AbstractDataItem
    {
        /// <summary>
        /// 开关机类型
        /// </summary>
        public EnumSwitchTimeType Type { get; set; }

        /// <summary>
        /// 开关机类型
        /// </summary>
        [Display(Key = "SwitchType")]
        public string TypeDesc
        {
            get { return this.Type.GetDescriptionX(); }
        }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? SwitchTimeInfoDate { get; set; }

        [Display]
        public string _SwitchTimeInfoDate
        {
            get { return this.SwitchTimeInfoDate.ToDateTimeString(); }
        }

    }
}
