/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/15 9:56:50 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace X64Service
{
    public static class IOSDeviceCoreDll
    {
        private const string IPhoneDeviceDll = @"bin\iphoneDevice.dll";
        private const string DevicePreDealDll = @"bin\iphoneDevice_pre.dll";

        /// <summary>
        /// 预先处理，必须第一个调用。
        /// </summary>
        /// <returns></returns>
        public static uint DeviceMount()
        {
            return DevicePreDeal("");
        }

        #region IOS设备底层交互接口

        /// <summary>
        /// 预先处理，必须第一个调用。
        /// </summary>
        /// <returns></returns>
        [DllImport(DevicePreDealDll, EntryPoint = "iphonepreDeal")]
        public static extern uint DevicePreDeal(string path);

        /// <summary>
        /// 开始设备监听，需要传入回调函数以实时通知设备变更情况。
        /// </summary>
        /// <param name="startMonitoringCallback"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "openIphoneDevice")]
        public static extern uint StartDeviceMonitoring(StartMonitoringCallbackDelegate startMonitoringCallback);

        /// <summary>
        /// 关闭设备监听。
        /// </summary>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "closeIphoneDevice")]
        public static extern uint CloseDeviceMonitoring();

        /// <summary>
        /// 判断设备是否可用。
        /// </summary>
        /// <param name="uniqueDeviceID"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "checkIphoneOnline")]
        public static extern uint CheckDeviceIsOnline(string uniqueDeviceID);

        /// <summary>
        /// 获取设备的属性信息，包括名字，是否越狱，序列化等。
        /// </summary>
        /// <param name="uniqueDeviceID"></param>
        /// <param name="devicePropertyList"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "getIphoneDeviceInfos")]
        public static extern uint GetDeviceProperties(string uniqueDeviceID, ref IntPtr devicePropertyList);

        /// <summary>
        /// 获取设备所有安装的应用名称列表，含内置应用。
        /// </summary>
        /// <param name="uniqueDeviceID"></param>
        /// <param name="AppNameList"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "getIPhoneALLDomainNames")]
        public static extern uint GetALLInstalledApp(string uniqueDeviceID, ref IntPtr AppNameList);

        /// <summary>
        /// 从Iphone设备中拷贝一个文件到Windwos系统中。
        /// </summary>
        /// <param name="uniqueDeviceID"></param>
        /// <param name="iosFilePath"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "getIphoneOneFile")]
        public static extern uint CopyOneIosFile(string uniqueDeviceID, string iosFilePath, string savePath);

        /// <summary>
        /// /从Iphone设备指定文件夹中得到第一级文件系统（文件、文件夹、快捷方式）。
        /// 根据type值判断文件类别
        /// </summary>
        /// <param name="uniqueDeviceID"></param>
        /// <param name="iosFolder"></param>
        /// <param name="fileSystemList"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "getIphoneALLFileInfosInDir")]
        public static extern uint GetIphoneALLFileInfosInDir(string uniqueDeviceID, string iosFolder, ref IntPtr fileSystemList);

        /// <summary>
        /// 根据应用ID，拷贝出用户应用文件。
        /// 此方法，能获取比CopyUserData对应应用更多数据。
        /// </summary>
        /// <param name="uniqueDeviceID"></param>
        /// <param name="appId"></param>
        /// <param name="savePath"></param>
        /// <param name="copyAppFilesCallback"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "getIphoneAllFilesByDomains")]
        public static extern uint CopyAppFilesByAppId(string uniqueDeviceID, string appId, string savePath, CopyAppFilesCallbackDelegate copyAppFilesCallback);

        /// <summary>
        /// 拷贝出所有（含内置）应用的用户数据。
        /// 本方法只能拷贝出应用的部分数据。
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="uniqueDeviceID"></param>
        /// <param name="copyUserDataCallback"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "ImgUserDATA")]
        public static extern uint CopyUserData(string savePath, string uniqueDeviceID, CopyUserDataCallbackDelegate copyUserDataCallback);

        /// <summary>
        /// 拷贝出所有（含内置）应用的用户数据，如果有密码情况要求用户输入密码
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="uniqueDeviceID"></param>
        /// <param name="copyUserDataCallback"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "ImgUserDATAPWDinput")]
        public static extern uint CopyUserDataPWD(string savePath, string uniqueDeviceID, CopyUserDataCallbackDelegate copyUserDataCallback, BackupPasswordInput backupPassword);

        /// <summary>
        /// 镜像文件
        /// 注：后续镜像需要重新做。
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="uniqueDeviceID"></param>
        /// <param name="copyUserDataCallback"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "ImgDATABin")]
        public static extern uint ImgDATABin(string savePath, string uniqueDeviceID, CopyUserDataCallbackDelegate copyUserDataCallback);

        /// <summary>
        /// 镜像完成后调用 获取镜像完成后的生成的bin包的md5值
        /// </summary>
        /// <param name="uniqueDeviceID"></param>
        /// <param name="md5"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "getImgDataMD5")]
        public static extern uint GetImgDataMD5(string uniqueDeviceID, ref IntPtr md5);

        /// <summary>
        /// 按需提取接口，必须先调用;
        /// </summary>
        /// <param name="puid">手机序列号</param>
        /// <param name="pdomain">应用域，如果为null,则包含整个文件系统（内置应用、外置应用）</param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "setIPhoneFileOpreationDomain")]
        public static extern uint SetIPhoneFileOpreationDomain(string puid, string pdomain);


        [DllImport(IPhoneDeviceDll, EntryPoint = "setIPhoneFileOpreationDomain")]
        public static extern uint SetIPhoneFileOpreationDomain(string puid, IntPtr pdomain);

        /// <summary>
        /// 设置当前操作文件服务
        /// </summary>
        /// <param name="puid">设备序列号</param>
        /// <param name="isjailbreak">是否越狱：true-越狱文件服务，false-正常文件服务</param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "setIphoneFileService")]
        public static extern uint SetIphoneFileService(string puid, bool isjailbreak);

        /// <summary>
        /// 分析Itunes备份文件
        /// </summary>
        /// <param name="pBackupDir"></param>
        /// <param name="pSaveDir"></param>
        /// <param name="backupcallback"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "AnalyzeItunesBackupDATA")]
        public static extern uint AnalyzeItunesBackupDATA(string pBackupDir, string pSaveDir, BackupAnalysizeCallbackDelegate backupcallback);

        /// <summary>
        /// 分析备份文件新接口，如果有密码情况要求用户输入密码
        /// </summary>
        /// <param name="pBackupDir"></param>
        /// <param name="pSaveDir"></param>
        /// <param name="backupcallback"></param>
        /// <param name="backupPassword"></param>
        /// <returns></returns>
        [DllImport(IPhoneDeviceDll, EntryPoint = "AnalyzeItunesBackupDATAPWDInput")]
        public static extern uint AnalyzeItunesBackupDATAPWD(string pBackupDir, string pSaveDir, BackupAnalysizeCallbackDelegate backupcallback, BackupPasswordInput backupPassword);

        #endregion
    }

    #region Ios文件系统结构体

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct IosFileSystem
    {
        public IntPtr Name;

        public byte Type;

        public uint Size;

        public uint CreateTime;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct IosFileSystem1
    {
        public byte Type;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IosFileSystemList
    {
        public int Length;

        public IntPtr FileSystemNodes;
    }

    #endregion

    #region 回调委托定义

    /// <summary>
    /// 设备监听回调
    /// </summary>
    /// <param name="connetctStatus"></param>
    /// <param name="uniqueDeviceID"></param>
    /// <returns></returns>
    public delegate int StartMonitoringCallbackDelegate(uint connetctStatus, string uniqueDeviceID);

    /// <summary>
    /// 拷贝单个应用数据回调
    /// </summary>
    /// <param name="uniqueDeviceID"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSize"></param>
    /// <returns></returns>
    public delegate int CopyAppFilesCallbackDelegate(string uniqueDeviceID, string fileName, long fileSize);

    /// <summary>
    /// 按需提取，拷贝数据回调
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="fileSize"></param>
    /// <returns></returns>
    public delegate int CopySourceFilesCallbackDelegate(string fileName, long fileSize);

    /*step当前操作步骤，coypedSize，拷贝的字节数，
     isStop是引用传递地址，回调函数中可以改变该值，1为终止拷贝（终止拷贝，已经拷贝的数据无法使用）
     step：1  初始化， 第二参数含义就是初始化百分比,此时不会响应停止要求
     step:   2   拷贝中， 第二参数含义就是拷贝的字节数，此时响应停止要求
     step:   3   后期处理，第二参数含义就是处理的百分比，此时不会响应停止要求
     结束：正常情况为第3 步完成百分之百
     错误情况，step为4 ，出错退出 第二参数无意义
     用户结束，step为5，用户终止腿 第二参数无意义
    */
    public delegate int CopyUserDataCallbackDelegate(string uniqueDeviceID, byte step, float copyedSize, ref UInt32 isStopCopy);

    /// <summary>
    /// ITUNES数据备份分析回调
    /// </summary>
    /// <param name="pFilename"></param>
    /// <param name="curfileno"></param>
    /// <param name="allnums"></param>
    /// <returns></returns>
    public delegate int BackupAnalysizeCallbackDelegate(string pFilename, int curfileno, int allnums);

    /// <summary>
    /// 备份分析回调
    /// </summary>
    /// <param name="pPassword">密码</param>
    /// <returns></returns>
    public delegate int BackupPasswordInput(IntPtr pPassword);

    #endregion

    #region 设备属性结构体

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DeviceProperty
    {
        public string PropertyKey;

        public string PropertyValue;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DevicePropertyList
    {
        [FieldOffset(0)]
        public int Length;

        [FieldOffset(4)]
        public IntPtr PropertyNodes;
    }

    #endregion

    #region 安装应用结构体

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct InstalledApp
    {
        public string AppId;

        public string AppInstallPath;

        public string InstallTime;

        public string Name;

        public string Version;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InstalledAppList
    {
        public int Length;

        public IntPtr AppNodes;
    }

    #endregion

}
