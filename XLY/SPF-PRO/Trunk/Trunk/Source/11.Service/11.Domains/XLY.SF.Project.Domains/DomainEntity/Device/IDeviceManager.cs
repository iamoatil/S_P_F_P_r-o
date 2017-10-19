using System;
using System.Collections.Generic;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 设备抽象管理服务接口（用于统一管理Android、IOS设备）
    /// </summary>
    public interface IDeviceManager
    {
        /// <summary>
        /// 设备是否可用
        /// </summary>
        bool IsValid(Device device);

        /// <summary>
        /// 验证是否Root(越狱)。True为已Root
        /// </summary>
        bool IsRoot(Device device);

        /// <summary>
        /// 读取手机指定文件目录的内容
        /// </summary>
        string ReadFile(Device device, string file);

        /// <summary>
        /// 拷贝指定文件
        /// </summary>
        string CopyFile(Device device, string source, string targetPath, IAsyncProgress asyn);

        /// <summary>
        /// 获取设备分区结构列表
        /// </summary>
        List<Partition> GetPartitons(Device device);

        /// <summary>
        /// 清除屏幕锁
        /// </summary>
        void ClearScreenLock(Device device);

        /// <summary>
        /// 恢复屏幕锁
        /// </summary>
        void RecoveryScreenLock(Device device);

        /// <summary>
        /// 获取设备属性信息
        /// </summary>
        Dictionary<string, string> GetProperties(Device device);

        /// <summary>
        /// 查找所有已安装应用列表
        /// </summary>
        List<AppEntity> FindInstalledApp(Device device);

        /// <summary>
        /// 查找所有已卸载应用列表
        /// </summary>
        List<AppEntity> FindUnInstalledApp(Device device);

        /// <summary>
        /// 获取设备SD卡路径
        /// 只支持安卓手机
        /// </summary>
        /// <param name="device">安卓手机设备</param>
        /// <returns></returns>
        string GetSDCardPath(Device device);
    }
}
