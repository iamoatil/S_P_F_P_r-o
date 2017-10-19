using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Utility.Logger;

namespace System
{

    public class DeletePartitionProtection
    {
        private static string _driverName = "XlyDiskSecureWrite";

        /// <summary>
        /// 关闭底层删除分区保护的程序
        /// 1.关闭写入接管
        /// 2.停止运行驱动
        /// 3.删除驱动
        /// </summary>
        public static void CancelDeleteProtection()
        {
            if (string.IsNullOrWhiteSpace(_driverName))
            {
                return;
            }

            try
            {
                switch (DeletePartitionParas.GetCurBitPrcoss())
                {
                    case BitProcess.x32:
                        DeletePartitionProtectionCoreX32.WriteOFF();
                        DeletePartitionProtectionCoreX32.StopDriver(_driverName);
                        DeletePartitionProtectionCoreX32.DeleteDriver(_driverName);
                        break;
                    case BitProcess.x64:
                        DeletePartitionProtectionCoreX64.WriteOFF();
                        DeletePartitionProtectionCoreX64.StopDriver(_driverName);
                        DeletePartitionProtectionCoreX64.DeleteDriver(_driverName);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("取消删除分区保护失败:", ex);
            }
        }

        /// <summary>
        /// 删除分区保护
        /// 1.安装驱动
        /// 2.运行驱动
        /// 3.启动写入接管
        /// </summary>
        /// <returns></returns>
        public static bool ExcuteDeleteProtection()
        {
            var lpszDriverPath = GetDriverPhysicalPath();
            if (string.IsNullOrEmpty(lpszDriverPath))
            {
                return false;
            }

            if (!System.IO.File.Exists(lpszDriverPath))
            {
                return false;
            }

            CancelDeleteProtection();

            try
            {
                var curBitProcess = DeletePartitionParas.GetCurBitPrcoss();

                if (InstallDriver(lpszDriverPath, curBitProcess))
                {
                    LogHelper.Error("删除磁盘分区保护：安装驱动不成功\r\n");
                    return false;
                }

                if (StartDriver(curBitProcess))
                {
                    LogHelper.Error("删除磁盘分区保护：开始驱动不成功\r\n");
                    return false;
                }

                if (WriteOn(curBitProcess))
                {
                    LogHelper.Error("删除磁盘分区保护：开始写不成功\r\n");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("删除分区保护失败:", ex);
            }


            return true;
        }

        private static string GetDriverPhysicalPath()
        {
            try
            {
                var operationType = DeletePartitionParas.GetCurrentOsType().ToString();
                var rootPath = AppDomain.CurrentDomain.BaseDirectory;
                var curBitProcess = DeletePartitionParas.GetCurBitPrcoss().ToString();

                var physicalPath = System.IO.Path.Combine(rootPath, "XlyDeleteWriteProtectNew", curBitProcess,
                    operationType, "XlyDiskSecureWrite.sys");
                return physicalPath;
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取删除分区保护的驱动文件失败：", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 安装删除分区保护驱动
        /// </summary>
        /// <param name="driverPath">分区保护驱动全路径</param>
        /// <param name="curBitProcess">当前程序是64位，还是32位</param>
        /// <returns></returns>
        private static bool InstallDriver(string driverPath, BitProcess curBitProcess)
        {
            if (curBitProcess == BitProcess.x32)
            {
                return DeletePartitionProtectionCoreX32.InstallDriver(_driverName, driverPath) != 0;
            }

            return DeletePartitionProtectionCoreX64.InstallDriver(_driverName, driverPath) != 0;
        }

        /// <summary>
        /// 启动分区保护驱动
        /// </summary>
        /// <param name="curBitProcess">当前程序是64位，还是32位</param>
        /// <returns></returns>
        private static bool StartDriver(BitProcess curBitProcess)
        {
            if (curBitProcess == BitProcess.x32)
            {
                return DeletePartitionProtectionCoreX32.StartDriver(_driverName) != 0;
            }

            return DeletePartitionProtectionCoreX64.StartDriver(_driverName) != 0;
        }

        /// <summary>
        /// 开启写保护
        /// </summary>
        /// <param name="curBitProcess">当前程序是64位，还是32位</param>
        /// <returns></returns>
        private static bool WriteOn(BitProcess curBitProcess)
        {
            if (curBitProcess == BitProcess.x32)
            {
                return DeletePartitionProtectionCoreX32.WriteOn() != 0;
            }

            return DeletePartitionProtectionCoreX64.WriteOn() != 0;
        }
    }
}