/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/11 14:08:23 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace X86DllServer
{
    /// <summary>
    /// BlackBerry 设备底层核心交互类
    /// </summary>
    public static class BlackBerryDeviceAPI
    {
        private const string _BlackBerryImg = "phoneBlackBerryImg.dll";

        #region 设备检测

        /// <summary>
        /// 查找设备
        /// </summary>
        /// <param name="link">输出：设备信息结构体指针（指针链），BlackPhoneInfo</param>
        /// <param name="nums">输出：设备数量</param>
        /// <returns>返回:0-成功，反之亦然</returns>
        [DllImport(_BlackBerryImg, EntryPoint = "BlackBerry_DeviceFind")]
        public static extern int BlackBerry_DeviceFind(ref IntPtr link, ref int nums);

        #endregion

        #region 数据提取-装载设备

        /// <summary>
        /// 装载设备
        /// </summary>
        /// <param name="phoneinfo">输入：设备信息结构体指针（指针链），BlackPhoneInfo</param>
        /// <returns>返回：设备句柄</returns>
        [DllImport(_BlackBerryImg, EntryPoint = "BlackBerry_Mount")]
        public static extern IntPtr BlackBerry_Mount(BlackPhoneInfo phoneinfo);

        /// <summary>
        /// 卸载设备句柄
        /// </summary>
        /// <param name="blackberryHadnle">输入：设备句柄</param>
        /// <returns>返回：0-成功，反之亦然</returns>
        [DllImport(_BlackBerryImg, EntryPoint = "BlackBerry_Close")]
        public static extern int BlackBerry_Close(ref IntPtr blackberryHadnle);

        #endregion

        #region 数据提取-数据备份

        /// <summary>
        /// 获取应用列表信息
        /// </summary>
        /// <param name="blackberryHadnle">输入：设备句柄</param>
        /// <param name="link">输出：应用列表信息结构体指针（指针链），BlackPhoneAppContentInfo</param>
        /// <param name="nums">输出：设备数量</param>
        /// <returns></returns>
        [DllImport(_BlackBerryImg, EntryPoint = "BlackBerry_GetAppDataInfo")]
        public static extern int BlackBerry_GetAppDataInfo(IntPtr blackberryHadnle, ref IntPtr link, ref int nums);

        /// <summary>
        /// 镜像应用数据
        /// </summary>
        /// <param name="blackberryHadnle">输入：设备句柄</param>
        /// <param name="psavedir">保存目录</param>
        /// <param name="pAppIndex">需要保存的app索引数据</param>
        /// <param name="nums">保存的app个数</param>
        /// <param name="callback">输入：镜像UserDAta数据回调函数</param>
        /// <returns>返回：0-成功，反之亦然</returns>
        [DllImport(_BlackBerryImg, EntryPoint = "BlackBerry_ImageAppData")]
        public static extern int BlackBerry_ImageAppData(IntPtr blackberryHadnle, string psavedir, int pAppIndex, int nums, CopyUserDataCallbackDelegate callback);

        /// <summary>
        /// 所有返回数据都通过该函数释放
        /// </summary>
        /// <param name="data">输入：释放数据</param>
        /// <returns>返回：设备句柄</returns>
        [DllImport(_BlackBerryImg, EntryPoint = "BlackBerry_ReleaseBuffer")]
        public static extern int BlackBerry_ReleaseBuffer(ref IntPtr data);

        #endregion

    }

    #region 设备基本信息结构体

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct BlackPhoneInfo
    {
        /// <summary>
        /// 设备索引
        /// </summary>
        public int deviceIndex;

        /// <summary>
        /// pin字符
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string pinStr;

        /// <summary>
        /// 型号字符
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string modelStr;

        /// <summary>
        /// 版本号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string softVersion;

        /// <summary>
        /// imei号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string imei;

        /// <summary>
        /// 总大小
        /// </summary>
        public uint apptotalMemory;

        /// <summary>
        /// 空闲大小
        /// </summary>
        public uint appfreeMemory;
    }

    #endregion

    #region 应用信息结构体

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct BlackPhoneAppContentInfo
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string name;

        /// <summary>
        /// 记录条数
        /// </summary>
        public int recordCount;
    }

    #endregion

    #region 文件信息结构体

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct FileInfo
    {
        /// <summary>
        /// UTF-8格式
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string name;

        /// <summary>
        /// 文件类型：1表示文件，2表示目录
        /// </summary>
        public int Flag;

        /// <summary>
        /// 文件大小
        /// </summary>
        public Int64 fileSize;

        /// <summary>
        /// 创建时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string createTime;
    }

    #endregion

    #region 回调委托定义

    /// <summary>
    /// 镜像UserDAta数据回调函数
    /// </summary>
    /// <param name="imagedALLSize">输入：已经镜像的数据大小</param>
    /// <param name="filename">输入：正在镜像的文件</param>
    /// <param name="stop">输出：可修改值，stop=1表示停止镜像</param>
    /// <returns>返回：0-成功，反之亦然</returns>
    public delegate int CopyUserDataCallbackDelegate(Int64 imagedALLSize, string filename, ref int stop);

    #endregion

}
