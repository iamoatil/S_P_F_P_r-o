using System;
using System.Xml.Serialization;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;

/* ==============================================================================
* Description：AbstractZipPluginInfo  
* Author     ：Fhjun
* Create Date：2017/7/3 14:41:13
* ==============================================================================*/


namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// zip压缩插件的基本数据结构
    /// </summary>
    public abstract class AbstractZipPluginInfo : AbstractPluginInfo
    {
        /// <summary>
        /// 插件文件名，相对路径，表示解析器调用的主插件
        /// </summary>
        [XmlIgnore]
        public virtual string ScriptFile { get; }

        /// <summary>
        /// 插件对象，如果是js插件，则表示插件的文本内容；如果是Python/Html插件，则表示为插件的路径；如果是C#插件，则为null
        /// </summary>
        [XmlIgnore]
        public virtual string ScriptObject { get; set; }

        /// <summary>
        /// 插件解压后临时存放路径
        /// </summary>
        [XmlIgnore]
        public virtual string ZipTempDirectory { get; set; }
    }
}
