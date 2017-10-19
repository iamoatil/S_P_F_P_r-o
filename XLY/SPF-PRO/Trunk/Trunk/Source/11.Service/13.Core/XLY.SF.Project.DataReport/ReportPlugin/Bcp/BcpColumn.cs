using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.BcpColumn
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/30 11:00:02
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// BcpColumn
    /// </summary>
    [Serializable]
    public class BcpColumn
    {
        /// <summary>
        /// 该列对应的蛛网编号
        /// </summary>
        [XmlAttribute]
        public string GAWACode { get; set; }
        /// <summary>
        /// 列对应的中文名称
        /// </summary>
        [XmlAttribute]
        public string ColumnName { get; set; }

        /// <summary>
        /// 列对应的属性名称
        /// </summary>
        [XmlAttribute]
        public string Property { get; set; }

        /// <summary>
        /// 外键类型
        /// </summary>
        [XmlAttribute]
        public BcpForeignKey FK { get; set; }
        /// <summary>
        /// 外键对应的属性名称
        /// </summary>
        [XmlAttribute]
        public string FKProperty { get; set; }
        /// <summary>
        /// 外键对应的节点位置
        /// </summary>
        [XmlAttribute]
        public string FKNode { get; set; }

        /// <summary>
        /// 转换函数，将结果转换为需要的格式，比如将“delete”转换为“已删除”
        /// </summary>
        [XmlAttribute("CF")]
        public string ConverterFunction { get; set; }

        /// <summary>
        /// 转换函数的参数BcpConverterParam[]，为Json格式，
        /// 例如[{"FK":"Merge","FKNode":"../../好友列表/*","FKProperty":"昵称"},{"FK":"Constant","FKNode":null,"FKProperty":"ggg"},{"FK":"AppName","FKNode":"/好友列表/*","FKProperty":null}]
        /// </summary>
        [XmlAttribute("CFParam")]
        public string ConverterParam { get; set; }
    }


    /// <summary>
    /// 外键类型
    /// </summary>
    public enum BcpForeignKey
    {
        /// <summary>
        /// 非外键，直接为当前节点的数据列
        /// 需要定义列名BcpColumn.Property
        /// </summary>
        None = 0,
        /// <summary>
        /// 直接从关联节点列中获取第一行的数据，并填充到数据集中
        /// 需要定义关联节点位置BcpColumn.FKNode和列名BcpColumn.FKProperty
        /// </summary>
        Merge,
        /// <summary>
        /// 该列的数据为TreeNode节点显示的文本
        /// 需要定义关联节点位置BcpColumn.FKNode
        /// </summary>
        NodeText,
        /// <summary>
        /// 该数据是通过用户配置中读取
        /// 需要定义BcpColumn.Property
        /// </summary>
        Configure,
        /// <summary>
        /// 该数据为常量
        /// 需要定义BcpColumn.Property
        /// </summary>
        Constant,
        /// <summary>
        /// 行参数，
        /// BcpColumn.Property='rowid'表示行号(1开始)
        /// </summary>
        Row,
    }
}
