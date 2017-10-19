using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using XLY.SF.Project.ViewDomain.DbModel;
using XLY.SF.Project.ViewDomain.Model;

namespace XLY.SF.ConsoleTest
{
    class Program
    {
        public class ClassPropertyElement
        {
            /// <summary>
            /// 是否为自定义类
            /// </summary>
            public bool IsCustromClass { get; set; }
            public string PropertyName { get; set; }

            public string PropertyValue { get; set; }

            public string XmlInnerPath { get; set; }
        }

        public static string GetAttributeValue(XmlElement xml)
        {
            var attTmp = xml.Attributes.GetNamedItem("Prompt");
            if (attTmp != null)
                return attTmp.Value;
            return string.Empty;
        }

        public static List<ClassPropertyElement> FFFF(XmlNode node, string xmlNodePath, string firstNodeName)
        {
            List<ClassPropertyElement> result = new List<ClassPropertyElement>();

            foreach (XmlNode item in node.ChildNodes)
            {
                var xmlItem = item as XmlElement;
                if (xmlItem == null)
                    continue;
                //得到最外层模块名
                ClassPropertyElement emt = new ClassPropertyElement();
                if (xmlItem.HasChildNodes && xmlItem.FirstChild.NodeType != XmlNodeType.Text)
                {
                    //类型
                    emt.IsCustromClass = true;
                    emt.PropertyName = xmlItem.LocalName;
                    emt.PropertyValue = xmlItem.HasAttributes ? GetAttributeValue(xmlItem) : xmlItem.FirstChild != null && xmlItem.FirstChild.NodeType == XmlNodeType.Text ? xmlItem.InnerText : "";
                    string tmp = string.Format("{0}/{1}", xmlNodePath, xmlItem.LocalName);
                    result.AddRange(FFFF(item, tmp, firstNodeName));
                }
                else if (!xmlItem.HasChildNodes || xmlItem.FirstChild.NodeType == XmlNodeType.Text)
                {
                    //属性
                    emt.IsCustromClass = false;
                    emt.PropertyName = string.Format("{0}_{1}", xmlNodePath, xmlItem.LocalName).TrimStart((firstNodeName + "/").ToArray());
                    emt.PropertyValue = xmlItem.HasAttributes ? GetAttributeValue(xmlItem) : xmlItem.FirstChild != null && xmlItem.FirstChild.NodeType == XmlNodeType.Text ? xmlItem.InnerText : "";
                    emt.XmlInnerPath = string.Format("{0}/{1}", xmlNodePath, xmlItem.LocalName);
                    result.Add(emt);
                }
            }
            return result;
        }

        public static List<ClassPropertyElement> LoadPro()
        {
            string languageXmlPath = @"G:\Work\SPF-PRO\Trunk\Trunk\Source\01.Framework\XLY.SF.Framework.Language\Language\Language_Cn.xml";
            XmlDocument xml = new XmlDocument();
            xml.Load(languageXmlPath);

            List<ClassPropertyElement> pro = new List<ClassPropertyElement>();
            foreach (XmlNode item in xml.ChildNodes)
            {
                pro.AddRange(FFFF(item, "LanguageResource", "LanguageResource"));
            }
            return pro;
        }

        static void Main(string[] args)
        {
            LoadPro();
            //List<PropElement> result = new List<PropElement>();
            //using (var cfgStream = File.Open(@"G:\Work\SPF-PRO\Trunk\Trunk\Source2\11.Service\12.Persistable\XLY.SF.Project.Persistable\Mapping\OperationLog.hbm.xml", FileMode.Open))
            //{
            //    XmlDocument xml = new XmlDocument();
            //    xml.Load(cfgStream);
            //    foreach (XmlElement item in xml.LastChild.LastChild.ChildNodes)
            //    {
            //        PropElement prop = new PropElement();
            //        prop.Name = item.GetAttribute("name");
            //        prop.Type = item.GetAttribute("type");
            //        if (prop.Type.ToUpper() == "DATETIME")
            //            prop.Type = "DateTime";
            //        //是否允许为NULL
            //        prop.NotNull = item.GetAttribute("not-null") == "1" || item.GetAttribute("not-null") == "true";
            //        //长度
            //        int tmpLength;
            //        if (int.TryParse(item.GetAttribute("length"), out tmpLength))
            //        {
            //            prop.Length = tmpLength;
            //        }
            //        //小数点位数
            //        if (int.TryParse(item.GetAttribute("precision"), out tmpLength))
            //        {
            //            prop.Precision = tmpLength;
            //        }


            //        //获取外键
            //        prop.Class = item.GetAttribute("class");
            //        result.Add(prop);
            //    }
            //}
            //var a = result;
        }
    }
}
