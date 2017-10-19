using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XLY.SF.Framework.BaseUtility.Plist
{
    public class PListXmlItemUid:IPlistXmlItem
    {
        public int Uid { get; set; }

        public int Value { get; set; }

        public void Load(XElement node)
        {
            Value = int.Parse(node.Value);
        }
    }
}
