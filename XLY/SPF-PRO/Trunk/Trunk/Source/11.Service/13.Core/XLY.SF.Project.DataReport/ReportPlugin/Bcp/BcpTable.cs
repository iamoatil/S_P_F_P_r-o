using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.BcpTable
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/30 10:59:27
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// BcpTable
    /// </summary>
    [Serializable]
    public class BcpTable
    {
        /// <summary>
        /// 对应的设备类型，如“Android"
        /// </summary>
        [XmlAttribute]
        public string Device { get; set; }

        /// <summary>
        /// 对应的App名称，如“短信”、“QQ”
        /// </summary>
        [XmlAttribute]
        public string AppName { get; set; }

        /// <summary>
        /// 对应插件版本
        /// </summary>
        [XmlAttribute]
        public string AppVersion { get; set; }

        /// <summary>
        /// BCP表格中文名称
        /// </summary>
        [XmlAttribute]
        public string TableName { get; set; }

        /// <summary>
        /// BCP表格对应蛛网编号
        /// </summary>
        [XmlAttribute]
        public string GAWACode { get; set; }

        /// <summary>
        /// 表格所在节点
        /// </summary>
        [XmlAttribute]
        public string Node { get; set; }

        /// <summary>
        /// 定义的列
        /// </summary>
        [XmlArray]
        public List<BcpColumn> Columns { get; set; }

        /// <summary>
        /// 行数据筛选
        /// eg：消息类型='微星转账' and 
        /// </summary>
        [XmlAttribute]
        public string RowFilter { get; set; }

        public override string ToString()
        {
            return string.Format("BcpTable, AppName:{0},AppVersion:{1},TableName:{2},GAWACode:{3}", AppName, AppVersion,
                TableName, GAWACode);
        }
    }
}
