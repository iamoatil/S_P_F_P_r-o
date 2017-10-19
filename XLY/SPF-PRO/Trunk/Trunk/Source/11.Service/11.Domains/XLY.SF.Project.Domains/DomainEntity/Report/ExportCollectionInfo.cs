using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.CollectionInfo
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/9 14:05:11
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 手机采集的基本信息(导出时）
    /// </summary>
    public class ExportCollectionInfo
    {
        /// <summary>
        /// 案件编号
        /// </summary>
        public string CaseCode { get; set; }
        /// <summary>
        /// 案件类型
        /// </summary>
        public string CaseType { get; set; }
        /// <summary>
        /// 案件名称
        /// </summary>
        public string CaseName { get; set; }
        /// <summary>
        /// 持有人姓名
        /// </summary>
        public string HolderName { get; set; }
        /// <summary>
        /// 持有人证件类型
        /// </summary>
        public string HolderCertificateType { get; set; }
        /// <summary>
        /// 持有人证件号码
        /// </summary>
        public string HolderCertificateCode { get; set; }
        /// <summary>
        /// 送检人姓名
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// 送检人单位
        /// </summary>
        public string SenderCompany { get; set; }
        /// <summary>
        /// 送检人证件号码
        /// </summary>
        public string SenderCertificateCode { get; set; }
        /// <summary>
        /// 采集人姓名
        /// </summary>
        public string CollectorName { get; set; }
        /// <summary>
        /// 采集时间
        /// </summary>
        public string CollectTime { get; set; }
        /// <summary>
        /// 采集人证件号码
        /// </summary>
        public string CollectorCertificateCode { get; set; }
        /// <summary>
        /// 采集点名称
        /// </summary>
        public string CollectLocation { get; set; }
        /// <summary>
        /// 采集点编号
        /// </summary>
        public string CollectLocationCode { get; set; }
        /// <summary>
        /// 说明信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 此采集设备的制造商名称，四川效率源信息安全技术股份有限公司
        /// </summary>
        public string ManufacturerName => "四川效率源信息安全技术股份有限公司";
        /// <summary>
        /// 此采集设备的制造商的厂商组织机构代码，673536061
        /// </summary>
        public string ManufacturerCode => "673536061";
    }
}
