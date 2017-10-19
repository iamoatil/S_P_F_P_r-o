using System.Xml;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/15 17:58:49
 * 类功能说明：
 *
 *************************************************/

namespace X86DllServer.Service
{
    public static partial class BaseTypeExtension
    {
        /// <summary>
        /// 安全的获取节点属性值
        /// </summary>
        public static string GetSafeAttributeValue(this XmlNode node, string attribute)
        {
            if (node == null)
            {
                return string.Empty;
            }
            if (node.Attributes != null)
            {
                var att = node.Attributes[attribute];
                if (att == null)
                {
                    return string.Empty;
                }
                return att.Value.Trim();
            }
            return string.Empty;
        }
    }
}
