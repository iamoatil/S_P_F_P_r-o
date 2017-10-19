/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/29 16:54:06 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace X86DllServer.DllEntry
{
    /// <summary>
    /// Vivo手机备份接口
    /// </summary>
    public static class VivoBackupAPI
    {
        private const string _MirrorDllPath = "phoneAndroidImg.dll";

        /// <summary>
        /// 获取镜像句柄，以类型的方式获取
        /// </summary>
        /// <param name="deviceSerialnumber">设备字符串</param>
        /// <param name="enum_imgtype">ENUM_IMGTYPE中的值, 0：常规镜像，1：Vivo备份</param>
        /// <returns></returns>
        [DllImport("phoneAndroidImg.dll", EntryPoint = "android_imageOpenByType")]
        public static extern IntPtr Android_imageOpenByType(string deviceSerialnumber, int enum_imgtype);

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="oIntPtr">打开设备接口返回的句柄</param>
        /// <returns></returns>
        [DllImport("phoneAndroidImg.dll", EntryPoint = "android_imageIniEnvironment")]
        public static extern int Android_imageIniEnvironment(IntPtr oIntPtr, int eachReadMaxSize, IntPtr htc);

        /// <summary>
        /// 获取backup应用列表，
        /// </summary>
        /// <param name="imgHandle">镜像句柄</param>
        /// <param name="pUserdata">返回的列表数据结构体</param>
        /// <param name="nums">  返回的列表数量</param>
        /// <returns></returns>
        [DllImport("phoneAndroidImg.dll", EntryPoint = "android_get_backup_applist")]
        public static extern int Android_get_backup_applist(IntPtr imgHandle, ref IntPtr pUserdata, ref int nums);

        /// <summary>
        /// backup备份数据
        /// </summary>
        /// <param name="imgHandle">镜像句柄</param>
        /// <param name="psavePath">保存路径</param>
        /// <param name="pbackupappid">需要备份的应用id，所有数据备份传NULL即可</param>
        /// <param name="nums">备份应用个数</param>
        /// <param name="callbak">备份过程的回调</param>
        /// <returns></returns>
        [DllImport("phoneAndroidImg.dll", EntryPoint = "android_imgData_backup")]
        public static extern int Android_imgData_backup(IntPtr imgHandle, string psavePath, string[] pbackupappid, int nums, P_IMGUserDATACallback callbak);

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <param name="imgHandle">镜像句柄</param>
        /// <returns></returns>
        [DllImport("phoneAndroidImg.dll", EntryPoint = "android_imageClose")]
        public static extern int Android_imgData_Close(ref IntPtr imgHandle);

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UserDataAPPInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] appid;
        public byte appType; // 0 ：内置应用  1、第三方应用
        public int recordCount; //应用的记录数
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] name; //应用的名称，编码格式为UTF8 
        public byte isMultifile; //是否是多个文件组成
    }

    /// <summary>
    /// 镜像UserDAta数据回调函数
    /// </summary>
    /// <param name="imagedALLSize">已经镜像的数据大小</param>
    /// <param name="filename">正在镜像的文件</param>
    /// <param name="stop">可修改值，stop=1表示停止镜像</param>
    /// <returns></returns>
    public delegate int P_IMGUserDATACallback(Int64 imagedALLSize, string filename, ref int stop);

}
