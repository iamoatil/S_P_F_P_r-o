using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/* ==============================================================================
* Description：WinRarHelper  
* Author     ：Fhjun
* Create Date：2017/4/14 14:01:13
* ==============================================================================*/

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// WinRar压缩解压缩帮助类
    /// </summary>
    public class WinRarHelper
    {
        /// <summary>
        /// rar.exe路径
        /// </summary>
        private static readonly string WinRarPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"toolkit\WinRAR\WinRAR.exe");

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern int ShowWindow(IntPtr hWnd, uint nCmdShow);

        /// <summary>
        /// 解压到某个文件夹中
        /// </summary>
        /// <param name="rarFilePath">rar文件全路径</param>
        /// <param name="unRarPath">解压到哪个文件夹</param>
        /// <param name="password">解压密码</param>
        /// <param name="isOverride">是否覆盖</param>
        public static void UnRar(string rarFilePath, string unRarPath, string password = null, bool isOverride = false)
        {
            RunCmd(string.Format("x{0} -o{1} {2} {3}", (password == null ? "" : " -p" + password), (isOverride ? "+" : "-"), rarFilePath, unRarPath));
        }

        /// <summary>
        /// 压缩文件或者文件夹为压缩包
        /// </summary>
        /// <param name="filePath">需要压缩的文件/文件夹全路径</param>
        /// <param name="saveFilePath">压缩文件保存全路径</param>
        /// <param name="isOverride">是否覆盖</param>
        /// <param name="password">压缩文件密码</param>
        public static void Rar(string filePath, string saveFilePath, bool isOverride = false, string password = null)
        {
            //RunCmd(string.Format("a{0} -o{1} -ep2 -r {2} {3}", (password == null ? "" : " -p" + password), (isOverride ? "+" : "-"), saveFilePath, filePath));
            RunCmd(string.Format(@"a {0} -o{1} {2} {3}\*.* -r -ep1", (password == null ? "" : " -p" + password), (isOverride ? "+" : "-"), saveFilePath, filePath));
        }

        /// <summary>
        /// 执行rar内部命令
        /// </summary>
        /// <param name="cmd">要执行的命令</param>
        private static void RunCmd(string cmd)
        {
            using (var p = new Process())
            {
                p.StartInfo.FileName = WinRarPath;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.Arguments = cmd;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                while (p.MainWindowHandle == IntPtr.Zero && !p.HasExited)
                {
                    //这儿必须等待，不然主界面还没Show出来MainWindowHandle是空的
                    //Thread.Sleep(10);
                }
                ShowWindow(p.MainWindowHandle, 0);//隐藏窗口

                p.WaitForExit();
            }
        }
    }
}
