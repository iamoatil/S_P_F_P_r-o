using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/28 15:09:28
 * 类功能说明：
 * 此类用于测试T4模版的运行结果
 * 实际开发中不需要使用到此类型
 *
 *************************************************/

namespace XLY.SF.Framework.Language
{
    public class TestT4
    {
        public static void Main()
        {
            Begin();





            //LanguageHelperSingle.Instance.SwitchLanguage(Project.Domains.Enum.LanguageType.Chinese);
            //List<ClassElementEx> emtTmp = Begin();
            //string a = "";
        }

        #region 同步英文版XML

        /// <summary>
        /// 更新英文XML文件结构（只针对string类型和自定义类型）
        /// </summary>
        public static void UpdateEnXml()
        {
            XmlSerializer xml = new XmlSerializer(typeof(LanguageResource));
            using (FileStream fs = File.OpenRead(""))
            {
                var value = xml.Deserialize(fs) as LanguageResource;
                //清空值
                foreach (var item in value.GetType().GetProperties())
                {
                    if (item.PropertyType == typeof(string))
                    {
                        //属性
                        item.SetValue(value, "内容区域");
                    }
                    else
                    {
                        //自定义类型
                        UpdateCustromClass(item.PropertyType, item.GetValue(value));
                    }
                }

                //反序列化更新XML结构
                MemoryStream ms = new MemoryStream();
                StreamWriter ffff = new StreamWriter(ms);
                xml.Serialize(ffff, value);
                ffff.Flush();
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ffff.BaseStream.Read(buffer, 0, buffer.Length);

                string a = Encoding.UTF8.GetString(buffer);
            }
        }

        public static void UpdateCustromClass(Type targetType, object instance)
        {
            foreach (var item in targetType.GetProperties())
            {
                if (item.PropertyType == typeof(string))
                {
                    //属性
                    item.SetValue(instance, "内容区域");
                }
                else
                {
                    //自定义类型
                    UpdateCustromClass(item.PropertyType, item.GetValue(instance));
                }
            }
        }

        #endregion

        #region 链表式

        public static void LoadClass(XmlNodeList xmlNodes, ref List<ClassElementEx> emt)
        {
            ClassElementEx classEmt = new ClassElementEx();
            classEmt.ClassName = xmlNodes.Item(0).ParentNode.LocalName;
            foreach (XmlElement item in xmlNodes)
            {
                var a = item.ParentNode.LocalName;
                //得到最外层模块名
                ClassPropertyElement proEmt = new ClassPropertyElement();
                if (item.HasChildNodes && item.FirstChild.NodeType != XmlNodeType.Text)
                {
                    //类型
                    proEmt.IsCustromClass = true;
                    proEmt.PropertyName = item.LocalName;
                    proEmt.PropertyValue = item.HasAttributes ? GetAttributeValue(item) : item.FirstChild != null && item.FirstChild.NodeType == XmlNodeType.Text ? item.InnerText : "";
                    LoadClass(item.ChildNodes, ref emt);
                }
                else if (!item.HasChildNodes || item.FirstChild.NodeType == XmlNodeType.Text)
                {
                    //属性
                    proEmt.IsCustromClass = false;
                    proEmt.PropertyName = item.LocalName;
                    proEmt.PropertyValue = item.HasAttributes ? GetAttributeValue(item) : item.FirstChild != null && item.FirstChild.NodeType == XmlNodeType.Text ? item.InnerText : "";
                }
                classEmt.Propertes.Add(proEmt);
            }
            emt.Add(classEmt);
        }

        public static List<ClassElementEx> Begin()
        {
            List<ClassElementEx> emtTmp = new List<ClassElementEx>();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(Resource1.Language_Cn);

            ClassElementEx languageElement = new ClassElementEx();
            languageElement.ClassName = "LanguageResource";
            foreach (var item in xml.DocumentElement.ChildNodes)
            {
                if (item is XmlElement)
                {
                    var xmlItem = item as XmlElement;
                    //得到最外层模块名
                    ClassPropertyElement emt = new ClassPropertyElement();
                    if (xmlItem.HasChildNodes && xmlItem.FirstChild.NodeType != XmlNodeType.Text)
                    {
                        //类型
                        emt.IsCustromClass = true;
                        emt.PropertyName = xmlItem.LocalName;
                        emt.PropertyValue = xmlItem.HasAttributes ? GetAttributeValue(xmlItem) : xmlItem.FirstChild != null && xmlItem.FirstChild.NodeType == XmlNodeType.Text ? xmlItem.InnerText : "";
                        LoadClass(xmlItem.ChildNodes, ref emtTmp);
                    }
                    else if (!xmlItem.HasChildNodes || xmlItem.FirstChild.NodeType == XmlNodeType.Text)
                    {
                        //属性
                        emt.IsCustromClass = false;
                        emt.PropertyName = xmlItem.LocalName;
                        emt.PropertyValue = xmlItem.HasAttributes ? GetAttributeValue(xmlItem) : xmlItem.FirstChild != null && xmlItem.FirstChild.NodeType == XmlNodeType.Text ? xmlItem.InnerText : "";
                    }
                    languageElement.Propertes.Add(emt);
                }
            }
            emtTmp.Add(languageElement);
            return emtTmp;
        }

        public static string GetAttributeValue(XmlElement xml)
        {
            var attTmp = xml.Attributes.GetNamedItem("Prompt");
            if (attTmp != null)
                return attTmp.Value;
            return string.Empty;
        }

        public class ClassElementEx
        {
            public ClassElementEx()
            {
                Propertes = new List<ClassPropertyElement>();
            }

            public string ClassName { get; set; }

            public List<ClassPropertyElement> Propertes { get; set; }
        }

        public class ClassPropertyElement
        {
            /// <summary>
            /// 是否为自定义类
            /// </summary>
            public bool IsCustromClass { get; set; }
            public string PropertyName { get; set; }

            public string PropertyValue { get; set; }
        }

        #endregion

        /* 树节点方式
        static void Main()
        {
            List<ClassElement> emtTmp = new List<ClassElement>();
            string languageText = Resource1.Language_Cn;
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(languageText);

            foreach (XmlElement item in xml.DocumentElement.ChildNodes)
            {
                //得到最外层模块名
                ClassElement emt = new ClassElement();
                if (item.HasChildNodes && item.FirstChild.NodeType != XmlNodeType.Text)
                {
                    emt.ClassName = item.LocalName;
                    //添加类型
                    LoadSubElement(item.ChildNodes, ref emt);
                }
                else if (!item.HasChildNodes || item.FirstChild.NodeType == XmlNodeType.Text)
                {
                    //属性
                    emt.PropertyName = item.LocalName;
                    emt.PropertyValue = item.InnerText;
                }
                emtTmp.Add(emt);
            }
        }

        static void LoadSubElement(XmlNodeList xmlNodes, ref ClassElement emt)
        {
            foreach (XmlNode item in xmlNodes)
            {
                ClassElement emtTmp = new ClassElement();
                if (item.HasChildNodes && item.FirstChild.NodeType != XmlNodeType.Text)
                {
                    emtTmp.ClassName = item.LocalName;
                    //添加类型
                    LoadSubElement(item.ChildNodes, ref emtTmp);
                }
                else if (!item.HasChildNodes || item.FirstChild.NodeType == XmlNodeType.Text)
                {
                    //属性
                    emtTmp.PropertyName = item.LocalName;
                    emtTmp.PropertyValue = item.InnerText;
                }
                emt.SubClass.Add(emtTmp);
            }
        }


        public class ClassElement
        {
            public ClassElement()
            {
                SubClass = new List<ClassElement>();
            }

            public string ClassName { get; set; }

            public string PropertyName { get; set; }

            public string PropertyValue { get; set; }

            public List<ClassElement> SubClass { get; set; }
        }*/
    }
}
