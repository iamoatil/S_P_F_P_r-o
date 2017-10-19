/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/29 16:52:33 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace X86DllServer.IService
{
    /// <summary>
    /// Vivo手机备份接口服务
    /// </summary>
    [ServiceContract]
    public interface IVivoBackupAPIService
    {
        /// <summary>
        /// 打开设备获取设备句柄
        /// </summary>
        /// <param name="deviceSerialnumber">设备序列号</param>
        /// <returns>设备句柄</returns>
        [OperationContract]
        Int32 VivoBackup_OpenDevice(string deviceSerialnumber);

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="imgHandle">打开设备接口返回的句柄</param>
        /// <returns>0：成功；其他：参照错误码</returns>
        [OperationContract]
        Int32 VivoBackup_Initialize(Int32 imgHandle);

        /// <summary>
        /// 获取APPID列表
        /// </summary>
        /// <param name="imgHandle"></param>
        /// <param name="listAppId"></param>
        /// <returns></returns>
        [OperationContract]
        Int32 VivoBackup_GetAppIDList(Int32 imgHandle, ref List<string> listAppId);

        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="imgHandle"></param>
        /// <param name="psavePath"></param>
        /// <param name="pbackupappid"></param>
        /// <param name="nums"></param>
        /// <returns></returns>
        [OperationContract]
        Int32 VivoBackup_BackupFiles(Int32 imgHandle, string psavePath, string[] pbackupappid, int nums);

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <param name="imgHandle"></param>
        /// <returns></returns>
        [OperationContract]
        Int32 VivoBackup_Close(ref Int32 imgHandle);

    }

}
