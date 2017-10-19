using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Utility.Logger;
using Microsoft.Win32;

namespace System
{
    public class DeletePartitionParas
    {
        /// <summary>
        /// 获取操作系统类型
        /// </summary>
        /// <returns></returns>
        public static OSType GetCurrentOsType()
        {
            RegistryKey osKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");           

            try
            {
                var osName = osKey.GetValue("ProductName").ToString();
                if (osName.Contains("8"))
                {
                    if (Environment.Is64BitOperatingSystem) return OSType.Win8X64;
                    else return OSType.Win8X32;
                }
                else if (osName.Contains("7"))
                {
                    if (Environment.Is64BitOperatingSystem) return OSType.Win7X64;
                    else return OSType.Win7X32;
                }
                else
                {
                    return OSType.Win7X32;
                }
            }
            catch (Exception exp)
            {
                LogHelper.Error("OSType", exp);
                return OSType.Win7X32;
            }
            finally
            {
                osKey.Close();
            }
        }

        /// <summary>
        /// 获取当前进程位数：64位，32位
        /// </summary>
        /// <returns></returns>
        public static BitProcess GetCurBitPrcoss()
        {
            try
            {
                return Environment.Is64BitProcess ? BitProcess.x64 : BitProcess.x32;
            }
            catch (Exception ex)
            {
                LogHelper.Error("删除分区保护，获取BitPrcoss失败:", ex);
                return BitProcess.x32;
            }
        }
    }

    /// <summary>
    /// 操作系统类型
    /// </summary>
     public enum OSType
    {
        Win7X32,
        Win7X64,
        Win8X32,
        Win8X64
    }
    /// <summary>
    /// 进程位数：64位，32位
    /// </summary>
    public enum BitProcess
    {
        x32,
        x64
    }
}
