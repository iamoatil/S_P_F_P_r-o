using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
    /// <summary>
    /// 删除磁盘分区保护
    /// </summary>
    public static class DeletePartitionProtectionCoreX64
    {
        #region Dll import
        /// <summary>
        /// 常量对象
        /// </summary>
        private const string _dllPathConst = @"XlyDeleteWriteProtectNew\x64\minifilterInstall.dll";

        #endregion

        /// <summary>
        /// 安装驱动
        /// </summary>
        /// <param name="lpszDriverName">驱动名</param>
        /// <param name="lpszDriverPath">驱动存放路径</param>
        /// <param name="lpszAltitude">驱动级别，最好填写“370050”</param>
        /// <returns>成功，返回0</returns>
        [DllImport(_dllPathConst, EntryPoint = "XLY_InstallDriver")]
        public static extern int InstallDriver(string lpszDriverName, string lpszDriverPath, string lpszAltitude = "370050");

        /// <summary>
        /// 删除驱动
        /// </summary>
        /// <param name="lpszDriverName">驱动名</param>
        /// <returns>成功，返回0</returns>
        [DllImport(_dllPathConst, EntryPoint = "XLY_DeleteDriver")]
        public static extern int DeleteDriver(string lpszDriverName);


        /// <summary>
        ///启动驱动
        /// </summary>
        /// <param name="lpszDriverName">驱动名</param>
        /// <returns>成功，返回0</returns>
        [DllImport(_dllPathConst, EntryPoint = "XLY_StartDriver")]
        public static extern int StartDriver(string lpszDriverName);

        /// <summary>
        ///关闭驱动
        /// </summary>
        /// <param name="lpszDriverName">驱动名</param>
        /// <returns>成功，返回0</returns>
        [DllImport(_dllPathConst, EntryPoint = "XLY_StopDriver")]
        public static extern int StopDriver(string lpszDriverName);

        ///  <summary>
        /// 开始写
        ///  </summary>
        /// <returns>成功，返回0</returns>
        [DllImport(_dllPathConst, EntryPoint = "XLY_WRITE_ON")]
        public static extern int WriteOn();

        ///  <summary>
        /// 关闭写
        ///  </summary>
        /// <returns>成功，返回0</returns>
        [DllImport(_dllPathConst, EntryPoint = "XLY_WRITE_OFF")]
        public static extern int WriteOFF();

    }
}
