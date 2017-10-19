using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XLY.SF.Framework.BaseUtility;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.BcpStandardFile
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/30 11:06:09
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// BCP格式的标准文件定义
    /// </summary>
    [Serializable]
    public class BcpStandardFile
    {
        [XmlArrayItem("Table")]
        public List<BcpStandardTable> Tables { get; set; }

        public BcpStandardTable this[string tableName]
        {
            get { return _dicTable[tableName]; }
        }

        public void Init()
        {
            _dicTable = this.Tables.ToDictionary(t => t.TableName, t => t);
        }

        private Dictionary<string, BcpStandardTable> _dicTable = null;

        private static BcpStandardFile _instance = null;
        public static BcpStandardFile Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Serializer.DeSerializeFromXML<BcpStandardFile>(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\BcpStandardFile.xml"));
                    _instance.Init();
                    _instance.Tables.ForEach(t =>
                    {
                        t.Init();
                    });
                }
                return _instance;
            }
        }
    }
    [Serializable]
    public class BcpStandardTable
    {
        [XmlAttribute]
        public string TableName { get; set; }

        [XmlArrayItem("Item")]
        public List<BcpStardardTableItem> Items { get; set; }

        public BcpStardardTableItem this[string itemKey]
        {
            get { return _dicTable[itemKey]; }
        }

        private Dictionary<string, BcpStardardTableItem> _dicTable = null;

        public void Init()
        {
            _dicTable = this.Items.ToDictionary(t => t.Key, t => t);
        }
    }

    [Serializable]
    public class BcpStardardTableItem
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Description { get; set; }
        [XmlAttribute]
        public string Type { get; set; }
        [XmlAttribute]
        public string Memo { get; set; }
    }
}
