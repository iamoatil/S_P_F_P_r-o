using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XLY.SF.Framework.BaseUtility.Plist
{
    public class PListXmlItemDict:IPlistXmlItem
    {
        public int Uid { get; set; }

        public Dictionary<string, IPlistXmlItem> Value { get; set; }

        public void Load(XElement node)
        {
            Value = new Dictionary<string, IPlistXmlItem>();

            string key = string.Empty;
            IPlistXmlItem tempItem = null;

            var nodes = node.Elements().ToList();

            int count = nodes.Count / 2;

            for(int pos = 0;pos < count;pos++)
            {
                key = nodes[2 * pos].Value;
                tempItem = PListXmlReader.LoadXmlNode(nodes[2 * pos + 1]);

                if(null != tempItem)
                {
                    Value.Add(key, tempItem);
                }
            }
        }
    }
}
