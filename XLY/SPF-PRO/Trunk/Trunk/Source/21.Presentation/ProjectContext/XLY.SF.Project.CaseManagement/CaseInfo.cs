using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 案例信息。
    /// </summary>
    public class CaseInfo
    {
        #region Constructors

        public CaseInfo()
        {
            Id = Guid.NewGuid().ToString();
            Timestamp = DateTime.Now;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 标识符。
        /// </summary>
        public String Id { get; internal set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public virtual String Name { get; set; }

        /// <summary>
        /// 编号。
        /// </summary>
        public virtual String Number { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public virtual String Type { get; set; }

        /// <summary>
        /// 作者。
        /// </summary>
        public virtual String Author { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public DateTime Timestamp
        {
            get;
            internal set;
        }

        /// <summary>
        /// 路径。
        /// </summary>
        public virtual String Path { get; set; }

        #endregion

        #region Methods

        #region Internal

        internal String GetDirectoryName()
        {
            return $"{Name}_{Timestamp.ToString("yyyyMMdd[hhmmss]")}";
        }

        #endregion

        #endregion
    }
}
