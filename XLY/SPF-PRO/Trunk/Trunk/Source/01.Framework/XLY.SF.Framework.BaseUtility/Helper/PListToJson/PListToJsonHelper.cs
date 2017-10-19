using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using XLY.SF.Framework.BaseUtility.Plist;

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// PList转Json辅助类
    /// 目前用于IOS微信数据解析，其他数据暂未验证
    /// </summary>
    public class PListToJsonHelper
    {
        /// <summary>
        /// 将Plist数据转换成Json
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static JToken PlistToJson(byte[] buffer)
        {
            using(MemoryStream memSourceStream = new MemoryStream())
            {
                memSourceStream.Write(buffer, 0, buffer.Length);
                memSourceStream.Seek(0, SeekOrigin.Begin);

                PListRoot root = PListRoot.Load(memSourceStream);

                using(MemoryStream memXmlStream = new MemoryStream())
                {
                    root.Save(memXmlStream, PListFormat.Xml);
                    memXmlStream.Seek(0, SeekOrigin.Begin);

                    return PlistXmlToJson(memXmlStream);
                }
            }
        }

        /// <summary>
        /// 将Plist数据转换成Json
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static JToken PlistToJson(string plistFile)
        {
            PListRoot root = PListRoot.Load(plistFile);
            using(MemoryStream memXmlStream = new MemoryStream())
            {
                root.Save(memXmlStream, PListFormat.Xml);
                memXmlStream.Seek(0, SeekOrigin.Begin);

                return PlistXmlToJson(memXmlStream);
            }
        }

        /// <summary>
        /// 将Plist数据转换成Json
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static JToken PlistToJson(Stream plistFile)
        {
            PListRoot root = PListRoot.Load(plistFile);
            using(MemoryStream memXmlStream = new MemoryStream())
            {
                root.Save(memXmlStream, PListFormat.Xml);
                memXmlStream.Seek(0, SeekOrigin.Begin);

                return PlistXmlToJson(memXmlStream);
            }
        }

        private static JToken PlistXmlToJson(Stream xmlFile)
        {
            XElement xmlDoc = XElement.Load(xmlFile);

            return PXmlToJson(xmlDoc);
        }

        private static JToken PlistXmlToJson(string xmlFile)
        {
            XElement xmlDoc = XElement.Load(xmlFile);

            return PXmlToJson(xmlDoc);
        }

        private static JToken PXmlToJson(XElement xmlDoc)
        {
            PListXmlReader reader = new PListXmlReader();

            return reader.LoadXml(xmlDoc);
        }

    }
}
