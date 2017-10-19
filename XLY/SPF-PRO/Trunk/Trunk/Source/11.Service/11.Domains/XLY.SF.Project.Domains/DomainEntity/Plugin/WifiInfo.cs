using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// Wifi连接信息
    /// </summary>
    [Serializable]
    public class WiFiInfo : AbstractDataItem
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Display(Key = "Password")]
        public string Pwd { get; set; }

        /// <summary>
        /// 连接方式
        /// </summary>
        [Display]
        public string Type { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [Display]
        public int Priority { get; set; }

    }
}
