using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static partial class TypeExtension
    {
        /// <summary>
        /// 扩展Process，添加获取父进程方法
        /// </summary>
        /// <param name="pro">进程信息</param>
        /// <returns>父进程(如果存在),null(不存在父进程)</returns>
        public static Process Parent(this Process process)
        {
            int id = GetParentProcessID(process.Id);
            if(id!=0)
            {
                try
                {
                    return Process.GetProcessById(id);
                }
                catch
                {

                }
            }
            return null;
        }
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
