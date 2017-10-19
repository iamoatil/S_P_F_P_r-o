using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.BcpNode
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/30 11:03:52
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// BcpNode
    /// </summary>
    public class BcpNode
    {
        public TreeNode TreeNode { get; set; }
        public TreeNode ParentTreeNode { get; set; }
        public XmlNode XmlNode { get; set; }
        public XmlNode ParentXmlNode { get; set; }
        public DataTable DataTable { get; set; }

        public override string ToString()
        {
            return string.Format("BcpNode：Text:{0}，Items:{1}", TreeNode.Text,
                DataTable == null ? 0 : DataTable.Rows.Count);
        }
    }
}
