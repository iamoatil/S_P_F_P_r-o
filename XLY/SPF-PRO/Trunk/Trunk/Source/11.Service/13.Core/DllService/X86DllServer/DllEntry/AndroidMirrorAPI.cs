/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/29 10:32:31 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace X86DllServer
{
    /// <summary>
    /// Android 镜像API
    /// </summary>
    public static class AndroidMirrorAPI
    {
        // Android镜像
        private const string _MirrorDllPath = "phoneAndroidImg.dll";

        /// <summary>
        /// 镜像回调函数
        /// </summary>
        /// <param name="data">每次返回数据</param>
        /// <param name="datasize">每次返回字节数</param>
        /// <param name="stop">是否停止，0-继续，1-停止</param>
        /// <returns></returns>
        public delegate int ImageDataCallBack(IntPtr data, int datasize, ref int stop);

        /// <summary>
        /// 打开设备获取设备句柄
        /// </summary>
        /// <param name="deviceSerialnumber">设备序列号</param>
        /// <returns>设备句柄</returns>
        [DllImport(_MirrorDllPath, EntryPoint = "android_imageOpen")]
        public static extern IntPtr OpenDevice(string deviceSerialnumber);

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="oIntPtr">打开设备接口返回的句柄</param>
        /// <param name="eachReadMaxSize">底层最大数据回馈：61440 标识每次数据的回馈有64K.底层最大数据回馈20M</param>
        /// <param name="htc">默认IntPtr.Zero</param>
        /// <returns>0：成功；其他：参照错误码</returns>
        [DllImport(_MirrorDllPath, EntryPoint = "android_imageIniEnvironment")]
        public static extern int Initialize(IntPtr oIntPtr, int eachReadMaxSize, IntPtr htc);

        /// <summary>
        /// 断点续传镜像
        /// </summary>
        /// <param name="imgHandle">打开设备接口返回的句柄</param>
        /// <param name="pPhysicDataPhonePath">分区块区：镜像制定的块区</param>
        /// <param name="start">开始扇区数（默认0）：镜像文件字节总数/512</param>
        /// <param name="count">镜像总数（默认-1）：扇区长度-开始扇区数</param>
        /// <param name="imgDATACallback">回调方法</param>
        /// <returns>0：成功；其他：参照错误码</returns>
        [DllImport(_MirrorDllPath, EntryPoint = "android_imageDataZone")]
        public static extern int ImageDataZone(IntPtr imgHandle, string pPhysicDataPhonePath, Int64 start, Int64 count, ImageDataCallBack imgDATACallback);

    }
}
