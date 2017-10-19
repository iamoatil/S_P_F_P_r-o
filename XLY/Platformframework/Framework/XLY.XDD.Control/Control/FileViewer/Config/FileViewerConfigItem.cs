using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文件预览配置
    /// </summary>
    public class FileViewerConfigItem
    {
        public FileViewerConfigItem()
        {
            this.Extensions = new List<FileViewerConfigItemExtension>();
        }

        /// <summary>
        /// 预览器
        /// </summary>
        public FileViewerType FileViewer { get; set; }

        /// <summary>
        /// 支持的扩展名集合
        /// </summary>
        public List<FileViewerConfigItemExtension> Extensions { get; set; }

        /// <summary>
        /// 是否支持流读取
        /// </summary>
        public bool IsSupportStream { get; set; }

        public XElement ToXElement()
        {
            XElement element = new XElement("FileViewerConfigItem");
            element.SetAttributeValue("FileViewer", this.FileViewer);
            element.SetAttributeValue("IsSupportStream", this.IsSupportStream);
            foreach (FileViewerConfigItemExtension extension in this.Extensions)
            {
                element.Add(extension.ToXElement());
            }
            return element;
        }

        public void Load(XElement element)
        {
            this.FileViewer = (FileViewerType)Enum.Parse(typeof(FileViewerType), element.Attribute("FileViewer").Value);
            this.IsSupportStream = Convert.ToBoolean(element.Attribute("IsSupportStream").Value);
            foreach (XElement e in element.Elements("FileViewerConfigItemExtension"))
            {
                FileViewerConfigItemExtension extension = new FileViewerConfigItemExtension();
                extension.Load(e);
                this.Extensions.Add(extension);
            }
        }
    }
}
