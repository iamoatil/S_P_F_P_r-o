using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains.Search
{
    public class SearchCondition
    {
        /// <summary>
        /// 搜索内容的关键字。
        /// </summary>
        public String Keyword { get; set; }

        /// <summary>
        /// 搜索内容开始时间。
        /// </summary>
        public String StartDate { get; set; }

        /// <summary>
        /// 搜索内容结束时间。
        /// </summary>
        public String EndDate { get; set; }

        /// <summary>
        /// 搜索内容状态。 正常/删除
        /// </summary>
        public String ContentStatus { get; set; }

        /// <summary>
        /// 红包信息
        /// </summary>
        public String RedPacket { get; set; }
    }
}
