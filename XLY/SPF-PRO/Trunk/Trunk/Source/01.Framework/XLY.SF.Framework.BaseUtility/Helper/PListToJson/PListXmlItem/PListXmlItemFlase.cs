using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XLY.SF.Framework.BaseUtility.Plist
{
    public class PListXmlItemFalse:IPlistXmlItem
    {
        public int Uid { get; set; }

        public bool Value { get { return false; } }

        public void Load(XElement node)
        {

        }
    }
}
