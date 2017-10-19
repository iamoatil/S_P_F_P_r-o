using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/16 17:43:55
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 案例信息
    /// </summary>
    public class CaseInfo
    {
        /// <summary>
        /// 创建案例
        /// </summary>
        public CaseInfo()
        {
            CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 自动名称规则：业务名+设备名称+时间
        /// </summary>
        public string Name;

        /// <summary>
        /// 送检人
        /// </summary>
        public string CensorshipPeople { get; set; }
        /// <summary>
        /// 案发时间
        /// </summary>
        public DateTime IncidentTime { get; set; }
        /// <summary>
        /// 送检时间
        /// </summary>
        public DateTime CensorshipTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// 任务工作空间路径 ： mirror， source， package，report，log
        /// </summary>
        public string WorkSpace { get; set; }

        public string Remark;
        public List<string> Photos;
    }
}
