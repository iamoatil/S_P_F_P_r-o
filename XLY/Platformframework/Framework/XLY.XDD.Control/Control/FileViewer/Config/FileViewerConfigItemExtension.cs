using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文件预览配置后缀
    /// </summary>
    public class FileViewerConfigItemExtension
    {
        /// <summary>
        /// 后缀
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 该优先级确定是否优先于其他支持的视图
        /// </summary>
        public int Priority { get; set; }

        public XElement ToXElement()
        {
            XElement element = new XElement("FileViewerConfigItemExtension");
            element.SetAttributeValue("Extension", this.Extension);
            element.SetAttributeValue("Priority", this.Priority);
            return element;
        }

        public void Load(XElement element)
        {
            this.Extension = element.Attribute("Extension").Value;
            this.Priority = int.Parse(element.Attribute("Priority").Value);
        }
    }
}
