using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XLY.SF.Framework.BaseUtility.Plist
{
    public class PListXmlItemArray:IPlistXmlItem
    {
        public int Uid { get; set; }

        public List<IPlistXmlItem> Value { get; set; }

        public void Load(XElement node)
        {
            Value = new List<IPlistXmlItem>();

            IPlistXmlItem tempItem = null;

            foreach(var childNode in node.Elements())
            {
                tempItem = PListXmlReader.LoadXmlNode(childNode);
                if(null != tempItem)
                {
                    Value.Add(tempItem);
                }
            }
        }

    }
}
