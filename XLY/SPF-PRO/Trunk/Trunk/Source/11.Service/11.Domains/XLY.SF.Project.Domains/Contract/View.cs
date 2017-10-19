using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Description：View  
* Author     ：Fhjun
* Create Date：2017/3/17 15:57:38
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 视图（数据）类型
    /// </summary>
    public enum EnumViewType
    {
        /// <summary>
        /// xly预定义
        /// </summary>
        XLY = 0,

        /// <summary>
        /// 用户自定义类型
        /// </summary>
        Custom = 1,
    }

    #region DataView（数据视图配置）

    /// <summary>
    /// 数据视图配置
    /// </summary>
    [Serializable]
    [XmlRoot("Data")]
    public class DataView
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// 支持的数据契约
        /// </summary>
        [XmlAttribute("contract")]
        public string Contract { get; set; }

        ///// <summary>
        ///// 日期过滤器，只能指定一个字段名称
        ///// </summary>
        //[XmlAttribute("datefilter")]
        //public string DateFilter { get; set; }


        [XmlElement("Item")]
        public List<DataItem> Items { get; set; }

        /// <summary>
        /// 使用emit动态生成的类型
        /// </summary>
        [XmlIgnore]
        public Type DynamicType { get; set; }
    }

    #endregion

    #region DataItem（扩展视图配置项）
    /// <summary>
    ///数据项配置
    /// </summary>
    [Serializable]
    public class DataItem : IEquatable<DataItem>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [XmlAttribute("code")]
        public string Code { get; set; }

        /// <summary>
        /// 视图显示宽度
        /// </summary>
        [XmlAttribute("width")]
        public int Width { get; set; }

        /// <summary>
        /// 显示格式化
        /// </summary>
        [XmlAttribute("format")]
        public string Format { get; set; }

        /// <summary>
        /// 列数据类型
        /// </summary>
        [XmlAttribute("type")]
        public EnumColumnType Type { get; set; }

        /// <summary>
        /// 排序方式，默认None不排序
        /// </summary>
        [XmlAttribute("order")]
        public EnumOrder Order { get; set; }

        /// <summary>
        /// 对齐方式，默认左对齐
        /// </summary>
        [XmlAttribute("alignment")]
        public EnumAlignment Alignment { get; set; }

        /// <summary>
        /// 分组序号设置，默认-1不分组
        /// </summary>
        [XmlAttribute("groupidex")]
        public int GroupIdex { get; set; }

        /// <summary>
        /// 是否列表显示
        /// </summary>
        [XmlAttribute("show")]
        public bool Show { get; set; }

        #region ExtendView-构造函数（初始化）

        /// <summary>
        ///  ExtendView-构造函数（初始化）
        /// </summary>
        public DataItem()
        {
            this.Width = 100;
            this.Order = EnumOrder.None;
            this.Alignment = EnumAlignment.Left;
            this.GroupIdex = -1;
            this.Show = true;
        }

        #endregion

        #region IEquatable
        /// <summary>
        /// 视图项配置是否相同
        /// </summary>
        public bool Equals(DataItem obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.Name == this.Name && obj.Code == this.Name && obj.Width == this.Width && obj.Format == this.Format)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
    #endregion
}
