using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.BaseUtility
{
    public static class CmdProcessHelper
    {
        /// <summary>
        /// 执行CMD命令
        /// </summary>
        /// <param name="filePath">要启动的exe路径</param>
        /// <param name="arguments">命令行参数</param>
        public static void DoCmd(string filePath, string arguments)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = filePath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
        }

        /// <summary>
        /// 执行CMD命令并返回命令行输出结果
        /// </summary>
        /// <param name="filePath">要启动的exe路径</param>
        /// <param name="arguments">命令行参数</param>
        /// <returns>输出内容</returns>
        public static string GetCmdResult(string filePath, string arguments)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = filePath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit();

                return process.StandardOutput.ReadToEnd();
            }
        }

    }
}
