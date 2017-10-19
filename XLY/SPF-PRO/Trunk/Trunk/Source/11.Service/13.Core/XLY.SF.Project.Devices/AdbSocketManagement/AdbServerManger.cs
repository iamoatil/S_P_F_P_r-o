using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.BaseUtility;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/4/7 16:17:55
 * 类功能说明：
 * 1. Adb服务管理器
 * 2. 只负责维护Adb服务
 *
 *************************************************/

namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    /// <summary>
    /// 维护ADB服务
    /// </summary>
    public class AdbServerManger
    {
        /// <summary>
        /// ADB进程名称
        /// </summary>
        private static readonly string _adbProcessName = "adb";

        /// <summary>
        /// ADB程序本地地址
        /// </summary>
        private static readonly string _adbOsLocation = @"ADB\adb.exe";

        #region 关闭Adb

        /// <summary>
        /// 关闭adb
        /// </summary>
        public static void CloseAdbServer()
        {
            int status = -1;
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(_adbOsLocation, "kill-server");
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;
                using (Process proc = Process.Start(psi))
                {
                    proc.WaitForExit();
                    status = proc.ExitCode;
                }
                //再补一刀
                SystemHelper.KillProcess(_adbProcessName);
            }
            catch (Exception e)
            {
                LoggerManagerSingle.Instance.Error(e);
            }

            if (status != 0)
            {
                LoggerManagerSingle.Instance.Error(string.Format("终止ADB失败, ExitCode:{0}", status));
            }

            //此处，要歇会儿
            System.Threading.Thread.Sleep(1000);
        }

        #endregion

        #region 开启Adb

        /// <summary>
        /// 启动adb
        /// </summary>
        private static void StartAdbServer()
        {
            int status = -1;
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(_adbOsLocation, "start-server");
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;

                using (Process proc = Process.Start(psi))
                {
                    //DoAdbHelper(proc.Id);  //修改adb启动端口号
                    proc.WaitForExit();
                    status = proc.ExitCode;
                }
            }
            catch (Exception e)
            {
                LoggerManagerSingle.Instance.Error(e);
            }

            if (status != 0)
            {
                LoggerManagerSingle.Instance.Error(string.Format("ADB启动失败, ExitCode:{0}", status));
            }
        }

        #endregion

        #region 关闭其他占用5037端口的进程，并重启Adb服务

        /// <summary>
        /// 关闭其他占用5037端口的进程，并重启Adb服务
        /// </summary>
        public static void KillPortProcessRestart()
        {
            try
            {
                Process[] otheradbpro = GetPortProcessForListen(AdbSocketOperator.DefaultPort);
                foreach (Process pro in otheradbpro)
                {
                    try
                    {
                        pro.Kill();
                        StartAdbServer();
                    }
                    catch
                    {
                        Process parent = GetProcessParent(pro);
                        if (parent != null && parent.ProcessName.ToLower() != "explorer")
                        {
                            try
                            {
                                parent.Kill();
                                StartAdbServer();
                            }
                            catch(Exception ex)
                            {
                                LoggerManagerSingle.Instance.Error(ex);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
        }

        #endregion

        #region 获取指定进程的父进程

        /// <summary>
        /// 扩展Process，添加获取父进程方法
        /// </summary>
        /// <param name="pro">进程信息</param>
        /// <returns>父进程(如果存在),null(不存在父进程)</returns>
        public static Process GetProcessParent(Process process)
        {
            int id = GetParentProcessID(process.Id);
            if (id != 0)
            {
                try
                {
                    return Process.GetProcessById(id);
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error(ex);
                }
            }
            return null;
        }

        #endregion

        #region 通过C++获取占用指定端口的进程
        [DllImport("Win32Libraryx64.dll", EntryPoint = "GetPortProcessForListen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void c_GetPortProcessForListen64(int port, StringBuilder ProcessIDs, int bufflen);

        [DllImport("Win32Library.dll", EntryPoint = "GetPortProcessForListen", CallingConvention = CallingConvention.Cdecl)]
        private static extern void c_GetPortProcessForListen(int port, StringBuilder ProcessIDs, int bufflen);

        private static Process[] GetPortProcessForListen(int port)
        {
            StringBuilder ProcessIDs = new StringBuilder(102400);
            if (Environment.Is64BitProcess)
                c_GetPortProcessForListen64(port, ProcessIDs, 102400);
            else
                c_GetPortProcessForListen(port, ProcessIDs, 102400);
            List<Process> Pros = new List<Process>();
            string[] prosstr = ProcessIDs.ToString().Split(';');
            foreach (string s in prosstr)
            {
                if (string.IsNullOrEmpty(s)) continue;
                try
                {
                    Process pro = Process.GetProcessById(Convert.ToInt32(s));
                    if (pro.MainModule.FileName.ToLower() == (AppDomain.CurrentDomain.BaseDirectory + _adbOsLocation).ToLower()) continue;
                    Pros.Add(pro);
                }
                catch
                {

                }
            }
            return Pros.ToArray();
        }
        #endregion
        
        #region 通过C++获取进程的父进程

        [DllImport("Win32Libraryx64.dll", EntryPoint = "GetParentProcessID", CallingConvention = CallingConvention.Cdecl)]
        private static extern int c_GetParentProcessID64(int pid);
        [DllImport("Win32Library.dll", EntryPoint = "GetParentProcessID", CallingConvention = CallingConvention.Cdecl)]
        private static extern int c_GetParentProcessID(int pid);
        private static int GetParentProcessID(int id)
        {
            if (Environment.Is64BitProcess)
                return c_GetParentProcessID64(id);
            else
                return c_GetParentProcessID(id);
        }

        #endregion
    }
}
