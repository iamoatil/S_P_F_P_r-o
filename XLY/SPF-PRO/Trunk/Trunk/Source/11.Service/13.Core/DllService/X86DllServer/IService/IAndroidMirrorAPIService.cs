/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/29 9:59:13 
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
    /// Android 镜像API服务
    /// </summary>\
    [ServiceContract]
    public interface IAndroidMirrorAPIService
    {
        /// <summary>
        /// 打开设备获取设备句柄
        /// </summary>
        /// <param name="deviceSerialnumber">设备序列号</param>
        /// <returns>设备句柄</returns>
        [OperationContract]
        Int32 AndroidMirror_OpenDevice(string deviceSerialnumber);

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="oIntPtr">打开设备接口返回的句柄</param>
        /// <param name="eachReadMaxSize">底层最大数据回馈：61440 标识每次数据的回馈有64K.底层最大数据回馈20M</param>
        /// <param name="htc">默认IntPtr.Zero</param>
        /// <returns>0：成功；其他：参照错误码</returns>
        [OperationContract]
        Int32 AndroidMirror_Initialize(Int32 oIntPtr, Int32 eachReadMaxSize, Int32 htc);

        /// <summary>
        /// 断点续传镜像
        /// </summary>
        /// <param name="imgHandle">打开设备接口返回的句柄</param>
        /// <param name="pPhysicDataPhonePath">分区块区：镜像制定的块区</param>
        /// <param name="start">开始扇区数（默认0）：镜像文件字节总数/512</param>
        /// <param name="count">镜像总数（默认-1）：扇区长度-开始扇区数</param>
        /// <returns>0：成功；其他：参照错误码</returns>
        [OperationContract]
        Int32 AndroidMirror_ImageDataZone(Int32 imgHandle, string pPhysicDataPhonePath, Int64 start, Int64 count);

    }

}
