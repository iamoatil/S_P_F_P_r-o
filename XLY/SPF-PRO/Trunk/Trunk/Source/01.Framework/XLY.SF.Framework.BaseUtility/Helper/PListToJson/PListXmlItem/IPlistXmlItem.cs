using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XLY.SF.Framework.BaseUtility.Plist
{
    public interface IPlistXmlItem
    {
        int Uid { get; set; }

        void Load(XElement node);

    }
}
