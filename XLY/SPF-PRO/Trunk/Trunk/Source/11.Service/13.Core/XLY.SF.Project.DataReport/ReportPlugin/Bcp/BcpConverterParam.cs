using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using XLY.SF.Framework.BaseUtility;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.BcpConverterParam
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/30 11:01:15
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// BcpConverterParam
    /// </summary>
    public class BcpConverterParam
    {
        /// <summary>
        /// 外键类型
        /// </summary>
        public BcpForeignKey FK { get; set; }
        /// <summary>
        /// 外键对应的属性名称
        /// </summary>
        public string FKProperty { get; set; }
        /// <summary>
        /// 外键对应的节点位置
        /// </summary>
        public string FKNode { get; set; }

        /// <summary>
        /// 将json格式的字符串转换为参数列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static BcpConverterParam[] ToConverterParams(string json)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(json))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    return serializer.Deserialize<BcpConverterParam[]>(json);
                }
                return new BcpConverterParam[0];
            }
            catch (Exception)
            {
                return new BcpConverterParam[0];
            }
        }
    }
}
