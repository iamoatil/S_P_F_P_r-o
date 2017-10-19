/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/14 14:57:22 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace X86DllServer.IService
{
    /// <summary>
    /// SIM相关接口服务
    /// </summary>
    [ServiceContract]
    public interface ISIMcoreAPIService
    {
        /// <summary>
        /// 获取SIM卡com口列表
        /// </summary>
        /// <param name="listComs"></param>
        /// <returns></returns>
        [OperationContract]
        int SimCard_scanCom(ref List<string> listComs);

        /// <summary>
        /// 读取本机号码和IMSI
        /// </summary>
        /// <param name="comstr"></param>
        /// <param name="listPhoneNo"></param>
        /// <param name="imsi"></param>
        /// <returns></returns>
        [OperationContract]
        int SimCard_readSimPhoneNoAndIMSI(string comstr, ref List<string> listPhoneNo, ref string imsi);

        /// <summary>
        /// 读取通讯录列表 以json格式返回
        /// </summary>
        /// <param name="comstr"></param>
        /// <returns></returns>
        [OperationContract]
        string SimCard_readAddressbook(string comstr);

        /// <summary>
        /// 读取通话记录列表 以json格式返回
        /// </summary>
        /// <param name="comstr"></param>
        /// <returns></returns>
        [OperationContract]
        string SimCard_readlastCalled(string comstr);

        /// <summary>
        /// 读取短信列表 以json格式返回
        /// </summary>
        /// <param name="comstr"></param>
        /// <returns></returns>
        [OperationContract]
        string SimCard_readSMS(string comstr);

    }
}
