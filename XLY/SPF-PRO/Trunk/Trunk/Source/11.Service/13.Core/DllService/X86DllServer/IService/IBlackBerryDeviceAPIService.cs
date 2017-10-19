/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/11 14:10:50 
 * explain :  
 *
*****************************************************************************/

using System.Collections.Generic;
using System.ServiceModel;

namespace X86DllServer.IService
{
    [ServiceContract]
    public interface IBlackBerryDeviceAPIService
    {
        /// <summary>
        /// 查找黑莓手机列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<BlackPhoneInfo> BlackBerry_FindDevices();

        [OperationContract]
        int BlackBerry_Mount(string pinStr);

        [OperationContract]
        void BlackBerry_Close(int blackberryHandle);

        [OperationContract]
        List<BlackPhoneAppContentInfo> BlackBerry_GetAppDataInfo(int blackberryHadnle);

        [OperationContract]
        int BlackBerry_ImageAppData(int blackberryHadnle, string psavedir);

        [OperationContract]
        void BlackBerry_ReleaseBuffer(int dataHandle);

    }
}
