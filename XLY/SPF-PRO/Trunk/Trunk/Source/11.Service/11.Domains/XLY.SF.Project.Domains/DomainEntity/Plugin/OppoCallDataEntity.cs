using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XLY.SF.Project.Domains
{
    public class CallLog
    {
        [XmlElement(ElementName = "Call_Records")]
        public List<Call_Records> listDatas { get; set; }
    }

    public class Call_Records
    {
        [XmlAttribute(AttributeName = "_id")]
        public string _id
        {
            get;
            set;
        }
        [XmlAttribute(AttributeName = "number")]
        public string number
        {
            get;
            set;
        }
        [XmlAttribute(AttributeName = "duration")]
        public string duration
        {
            get;
            set;
        }
        [XmlAttribute(AttributeName = "type")]
        public string type
        {
            get;
            set;
        }
        [XmlAttribute(AttributeName = "date")]
        public string date
        {
            get;
            set;
        }
        [XmlAttribute(AttributeName = "name")]
        public string name
        {
            get;
            set;
        }
        [XmlAttribute(AttributeName = "numbertype")]
        public string numbertype
        {
            get;
            set;
        }
        [XmlAttribute(AttributeName = "numberlabel")]
        public string numberlabel
        {
            get;
            set;
        }
    }
}
