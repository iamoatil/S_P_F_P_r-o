using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.Contract.DataItemContract.IMoney
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/21 10:56:04
* ==============================================================================*/

namespace XLY.SF.Project.Domains.Contract.DataItemContract
{
    /// <summary>
    /// 金额查看模式，比如红包、转账
    /// </summary>
    public interface IMoney
    {
        /// <summary>
        /// 时间，可空
        /// </summary>
        DateTime? Date { get; set; }
        /// <summary>
        /// 发送者姓名
        /// </summary> 
        string SenderName { get; set; }
        /// <summary>
        /// 接收者姓名
        /// </summary> 
        string ReceiverName { get; set; }
        /// <summary>
        /// 金额数目（单位：元）
        /// </summary>
        double Money { get; set; }
    }
}
