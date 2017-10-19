using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XLY.SF.Framework.BaseUtility.Plist
{
    public class PListXmlItemString:IPlistXmlItem
    {
        public int Uid { get; set; }

        public string Value { get; set; }

        public void Load(XElement node)
        {
            if(node.Value == "$null")
            {
                Value = string.Empty;
            }
            else
            {
                Value = node.Value;
            }
        }

    }
}
