using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

/*************************************************
 * 创建人：Bob
 * 创建时间：2017/9/8
 * 类功能说明：
 * 1. 推荐方案统一返回的数据类型
 * 2. 目前是预定义项，正式开发可能需要添加
 *
 *************************************************/

namespace XLY.SF.Project.ViewDomain.Model
{
    /// <summary>
    /// 推荐方案元素
    /// </summary>
    public class StrategyElement
    {
        #region 解决方案基础信息

        /// <summary>
        /// 推荐方案名称
        /// </summary>
        [XmlElement]
        public string SolutionStrategyName { get; set; }

        /// <summary>
        /// 解决方案描述信息
        /// </summary>
        [XmlElement]
        public string SolutionStrategyDes { get; set; }

        /// <summary>
        /// 解决方案图标
        /// </summary>
        [XmlElement]
        public string SolutionIcon { get; set; }

        #endregion

        [XmlArray]
        public List<StrategyFilterElement> Filter { get; set; }
    }

    /// <summary>
    /// 推荐方案筛选条件
    /// </summary>
    public class StrategyFilterElement
    {
        /// <summary>
        /// 品牌
        /// </summary>
        [XmlElement]
        public string PhoneBrand { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [XmlElement]
        public string DevType { get; set; }

        /// <summary>
        /// 设备型号
        /// </summary>
        [XmlElement]
        public string DevModel { get; set; }

        /// <summary>
        /// 设备系统版本
        /// </summary>
        [XmlElement]
        public string DevOSVersion { get; set; }

        /// <summary>
        /// 是否Root
        /// </summary>
        [XmlElement]
        public bool? IsRoot { get; set; }

        /// <summary>
        /// CPU品牌
        /// </summary>
        [XmlElement]
        public string CpuBrand { get; set; }

        /// <summary>
        /// CPU型号
        /// </summary>
        [XmlElement]
        public string CpuModel { get; set; }

        /// <summary>
        /// 是否打开调试模式
        /// </summary>
        [XmlElement]
        public bool IsOpenDebugMode { get; set; }
    }
}
