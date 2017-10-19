using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 案例信息。
    /// </summary>
    public class CaseInfo
    {
        #region Constructors

        internal CaseInfo()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 名称。
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 编号。
        /// </summary>
        public String Number { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public String Type { get; set; }

        /// <summary>
        /// 作者。
        /// </summary>
        public String Author { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public DateTime Timestamp { get; internal set; }

        /// <summary>
        /// 路径。
        /// </summary>
        public String Path { get; set; }

        #endregion

        #region Methods

        #region Internal

        /// <summary>
        /// 获取案例信息的XML表示。
        /// </summary>
        /// <returns>案例信息的XML表示。</returns>
        internal XElement ToXElement()
        {
            Timestamp = DateTime.Now;
            XElement propertyGroup = new XElement("PropertyGroup",
                new XElement("Name",Name),
                new XElement("Number",Number),
                new XElement("Type",Type),
                new XElement("Author",Author),
                new XElement("Timestamp", Timestamp));
            return propertyGroup;
        }

        #endregion

        #endregion
    }
}
