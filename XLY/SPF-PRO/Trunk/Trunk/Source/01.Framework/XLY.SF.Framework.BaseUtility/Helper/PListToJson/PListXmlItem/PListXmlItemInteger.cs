using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XLY.SF.Framework.BaseUtility.Plist
{
    public class PListXmlItemInteger:IPlistXmlItem
    {
        public int Uid { get; set; }

        public Int64 Value { get; set; }

        public void Load(XElement node)
        {
            Value = Int64.Parse(node.Value);
        }
    }
}
