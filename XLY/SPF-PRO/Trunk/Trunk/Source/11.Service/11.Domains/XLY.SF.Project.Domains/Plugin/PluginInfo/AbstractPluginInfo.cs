using System;
using System.Xml.Serialization;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;

/* ==============================================================================
* Description：AbstractPluginInfo  
* Author     ：Fhjun
* Create Date：2017/7/3 14:41:13
* ==============================================================================*/


namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 提供所有插件结构定义的基类
    /// </summary>
    public abstract class AbstractPluginInfo : IPluginInfo
    {
        /// <summary>
        /// 插件ID
        /// </summary>
        [XmlAttribute("guid")]
        public virtual string Guid { get; set; }
        /// <summary>
        /// 插件类型，比如“应用解析插件”
        /// </summary>
        [XmlAttribute("type")]
        public virtual PluginType PluginType { get; set; }
        /// <summary>
        /// 插件名称
        /// </summary>
        [XmlElement("name")]
        public virtual string Name { get; set; }
        /// <summary>
        /// 插件中配置的版本信息
        /// </summary>
        [XmlElement("version")]
        public virtual string VersionStr { get; set; }
        /// <summary>
        /// 插件版本,代表该插件支持的APP最低版本
        /// </summary>
        [XmlIgnore]
        public virtual Version Version => this.VersionStr.ToSafeVersion();
        /// <summary>
        /// 插件描述信息
        /// </summary>
        [XmlElement("description")]
        public virtual string Description { get; set; }
        /// <summary>
        /// 图标位置，
        /// </summary>
        [XmlElement("icon")]
        public virtual string Icon { get; set; }
        /// <summary>
        /// 插件分组信息
        /// </summary>
        [XmlElement("group")]
        public virtual string Group { get; set; }

        /// <summary>
        /// 插件排序
        /// </summary>
        [XmlElement("order")]
        public virtual int OrderIndex { get; set; }
        /// <summary>
        /// 插件状态
        /// </summary>
        [XmlIgnore]
        public virtual PluginState State { get; set; }

        /// <summary>
        /// 在读取解析插件后执行，比如有些数据需要重新计算（比如版本号）
        /// </summary>
        public virtual void AfterReadConfigure()
        {
            
        }
    }
}
