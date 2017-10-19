using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Extension;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.BaseUtility.SingleWrapper;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/4/12 14:42:15
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.DeviceTools.DeviceService.AdbSocketManagement
{
    /// <summary>
    /// USB端口检测接口处理类
    /// </summary>
    public class USBMonitorService
    {
        private USBMonitorService()
        { 

        }

        public static USBMonitorService Instance
        {
            get { return SingleWrapperHelper<USBMonitorService>.Instance; }
        }

        // USBMonitorService.exe服务程序名称
        private const string _USBServiceName = "USBMonitorService.exe";

        // 默认：默认盘符
        private const string _Default_DriveInfo = "C:\\";

        // 默认：USBMonitorService.exe原始安装路径
        private static string USBServiceName_Install_Path
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _USBServiceName); }
        }

        // 异常：当前盘符
        private static string _Current_DriveInfo = "C:\\";

        // 异常：USBMonitorService.exe初始化重定向目录，C:\\USBMonitorServiceXly
        private const string _USBService_Init_Dir = "{0}USBMonitorServiceXly";

        // 异常：USBMonitorService.exe初始化全路径
        private static string USBServiceName_Init_Path
        {
            get { return Path.Combine(string.Format(_USBService_Init_Dir, _Current_DriveInfo), _USBServiceName); }
        }

        // 异常：USBMonitorService.exe初始化目录
        public static string USBService_Init_Dir
        {
            get { return string.Format(_USBService_Init_Dir, _Current_DriveInfo); }
        }


        #region // USB端口检测DLL封装
        private const string _USBDeviceMonitor = "UsbMonitor.dll";

        /// <summary>
        /// 初始化服务
        /// 创建并运行服务：USBMonitorServiceXly
        /// 初始化规则：
        /// 不存在则创建并启动服务，
        /// 反之先停止服务（若在运行）并删除后再重新创建并启动服务
        /// </summary>
        /// <param name="path">USBMonitorServiceXly.exe服务存放路径，默认当前程序运行目录</param>
        /// <returns>返回:0-成功，反之亦然</returns>
        [DllImport(_USBDeviceMonitor, EntryPoint = "fun_0_0", CallingConvention = CallingConvention.Cdecl)]
        private static extern int USBMonitor_Service_Init(byte[] path);

        /// <summary>
        /// 停止并删除服务
        /// </summary>
        /// <returns>返回:0-成功，反之亦然</returns>
        [DllImport(_USBDeviceMonitor, EntryPoint = "fun_0_1")]
        private static extern int USBMonitor_Service_Del();

        /// <summary>
        /// 开始监听（回调函数）
        /// 队事件进行监听并通过回调函数返回监听到的数据
        /// 前提条件：USBMonitorServiceXly必须运行才可进行监听，反之亦然
        /// </summary>
        /// <returns>返回:0-成功，反之亦然</returns>
        [DllImport(_USBDeviceMonitor, EntryPoint = "fun_0_2", CallingConvention = CallingConvention.Cdecl)]
        private static extern int USBMonitor_Start(USBDeviceMonitorCallbackDelegate callback);

        /// <summary>
        /// 停止监听
        /// </summary>
        /// <returns>返回:0-成功，反之亦然</returns>
        [DllImport(_USBDeviceMonitor, EntryPoint = "fun_0_3")]
        private static extern int USBMonitor_Stop();
        #endregion

        #region // Start（启动设备监控）

        /// <summary>
        /// BlackBerry 设备USB端口监听预处理
        /// 1，初始化服务（传入服务名称初始化目录，绝对路径包括服务名称）
        /// 2，开始监听
        /// 说明，1步骤必须先于2步骤。
        /// </summary>
        public void Start()
        {
            // 服务启动失败：是否进行过重试
            int doretry = 0;
        DoRetry:
            try
            {

                // 默认以安装目录作为初始化目录，反之服务启动失败则以指定目录作为初始化目录
                string str_USBMonitorService_Init_Path = string.Empty;
                if (doretry == 0)
                {
                    str_USBMonitorService_Init_Path = USBServiceName_Install_Path;
                }
                else
                {
                    str_USBMonitorService_Init_Path = USBMonitorService_Init_Path();
                }

                // 获取初始化目录（Unicode）,用于初始化服务传入
                byte[] byte_USBMonitorService_Init_Path = Encoding.Unicode.GetBytes(str_USBMonitorService_Init_Path);

                // 1，初始化服务（传入服务名称初始化目录，绝对路径包括服务名称）
                int result = USBMonitorService.USBMonitor_Service_Init(byte_USBMonitorService_Init_Path);
                if (result != 0)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("设备USB端口监听预处理:初始化服务失败, 错误编码{0}", result));
                    throw new ApplicationException(string.Format("设备USB端口监听预处理:初始化服务失败, 错误编码{0}", result));
                }

                // 2，开始监听
                result = USBMonitorService.USBMonitor_Start(this._USBDeviceMonitorCallback);
                if (result != 0)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("设备USB端口监听预处理:监听启动失败, 错误编码{0}", result));
                    throw new ApplicationException(string.Format("设备USB端口监听预处理:监听启动失败, 错误编码{0}", result));
                }
            }
            catch (Exception ex)
            {
                if (doretry == 0)
                {
                    this.Close();
                    doretry = 1;
                    goto DoRetry;
                }
                LoggerManagerSingle.Instance.Error(ex, "设备USB端口监听预处理异常");
            }
        }

        /// <summary>
        /// 服务程序拷贝（从运行目录拷贝到指定初始化目录）
        /// </summary>
        /// <returns>自定义服务初始化目录</returns>
        private string USBMonitorService_Init_Path()
        {
            string str_USBMonitorService_Init_Path = string.Empty;  // 初始化目录

            try
            {
                // 获取磁盘号
                DriveInfo[] driveInfo = DriveInfo.GetDrives().OrderBy(d => d.Name).ToArray<DriveInfo>();
                bool isHashC = false;
                foreach (var item in driveInfo)
                {
                    if (item.Name == _Default_DriveInfo)
                    {
                        isHashC = true;
                        break;
                    }
                }

                if (!isHashC)
                {
                    _Current_DriveInfo = driveInfo[0].Name;
                }
                else
                {
                    _Current_DriveInfo = _Default_DriveInfo;
                }

                // 创建初始化目录
                FileHelper.CreateExitsDirectory(USBService_Init_Dir);

                // 拷贝服务程序
                if (!File.Exists(USBServiceName_Init_Path))
                {
                    File.Copy(USBServiceName_Install_Path, USBServiceName_Init_Path);
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "服务程序拷贝（从运行目录拷贝到指定初始化目录）失败");
            }
            return USBServiceName_Init_Path;
        }
        #endregion

        #region // Close（关闭设备监听，释放资源）

        /// <summary>
        /// 关闭设备监听，释放资源。
        /// 1，停止监听
        /// 2，停止并删除服务
        /// 3，删除初始化目录
        /// </summary>
        public void Close()
        {
            int m1 = 0;
            int m2 = 0;

            m1 = USBMonitorService.USBMonitor_Stop();
            try
            {
                m1 = USBMonitorService.USBMonitor_Stop();
                m2 = USBMonitorService.USBMonitor_Service_Del();
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
            FileHelper.DeleteDirectory(USBService_Init_Dir);
        }
        #endregion

        #region // USBDeviceMonitorCallbackDelegate（回调方法）
        /// <summary>
        /// USB端口检测回调函数。
        /// 非托管回调的方法必须声明为全局变量，否则可能被垃圾器回收导致C++接口反复调用时出错。
        /// </summary>
        public USBDeviceMonitorCallbackDelegate _USBDeviceMonitorCallback { get; set; }

        /// <summary>
        /// USB端口检测回调函数
        /// </summary>
        /// <param name="connetctStatus">连接状态：1 为连入 2为拔出</param>
        /// <param name="vid_pid">
        /// 返回信息：连接设备的pid和vid信息，
        /// 黑莓：\\?\USB#VID_04C5&PID_201D#0000000000000319#{a5dcbf10-6530-11d2-901f-00c04fb951ed}
        /// 苹果：
        /// HTC：
        /// </param>
        /// <returns>返回：0-成功，反之亦然</returns>
        public delegate int USBDeviceMonitorCallbackDelegate(int connetctStatus, string vid_pid);
        #endregion
    }
}
