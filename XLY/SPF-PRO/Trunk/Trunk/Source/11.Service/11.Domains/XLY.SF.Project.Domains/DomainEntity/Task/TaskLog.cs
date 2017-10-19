using System;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 任务日志
    /// </summary>
    [Serializable]
    public class TaskLog
    {
        /// <summary>
        /// 操作
        /// </summary>
        public string Operate { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>

        public DateTime CreateDate { get; set; }
    }
}
