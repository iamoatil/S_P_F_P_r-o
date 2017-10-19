using System;
using System.Collections.Generic;
using System.Linq;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Devices.AdbSocketManagement;
using XLY.SF.Project.Domains;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Project.Devices
{
    /// <summary>
    /// 安卓设备管理
    /// </summary>
    public sealed class AndroidDeviceManager : IDeviceManager
    {
        [Obsolete("请使用AndroidDeviceManager.Instance获取实例！")]
        public AndroidDeviceManager()
        {

        }

        /// <summary>
        /// 安卓设备管理服务实例，全局唯一单例
        /// </summary>
        public static AndroidDeviceManager Instance
        {
            get { return SingleWrapperHelper<AndroidDeviceManager>.Instance; }
        }

        /// <summary>
        /// 查找所有已安装应用列表
        /// </summary>
        public List<AppEntity> FindInstalledApp(Device device)
        {
            List<AppEntity> appEntitys = AndroidHelper.Instance.GetApps(device);
            return appEntitys;
        }

        /// <summary>
        /// 查找所有已卸载应用列表
        /// </summary>
        public List<AppEntity> FindUnInstalledApp(Device device)
        {
            return null;
        }

        /// <summary>
        /// 获取设备分区结构列表
        /// </summary>
        public List<Partition> GetPartitons(Device device)
        {
            var mounts = AndroidHelper.Instance.GetMountPoints(device);
            List<Partition> res = new List<Partition>();
            if (mounts.IsInvalid())
                return res;

            foreach (var m in mounts)
            {
                var p = new Partition();
                p.Name = m.Name;
                p.Block = m.Block;
                p.Size = m.Size;
                if (p.Name.Equals(ConstCodeHelper.PARTITION_DATA)) p.Text = LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.DeviceLanguage_Partition_data);
                else if (p.Name.Equals(ConstCodeHelper.PARTITION_All)) p.Text = LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.DeviceLanguage_Partition_all);
                else if (p.Name.EndsWith(ConstCodeHelper.PARTITION_SYSTEM)) p.Text = LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.DeviceLanguage_Partition_system);
                else if (p.Name.Contains(ConstCodeHelper.PARTITION_SDCARD)) p.Text = LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.DeviceLanguage_Partition_sdcard);
                else if (p.Name.EndsWith(ConstCodeHelper.PARTITION_CACHE)) p.Text = LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.DeviceLanguage_Partition_cache);
                else continue;
                res.Add(p);
            }
            if (res.IsValid())
            {
                res = res.OrderByDescending(s => s.Size).ToList();
            }
            return res;
        }

        /// <summary>
        /// 获取设备属性信息
        /// </summary>
        public Dictionary<string, string> GetProperties(Device device)
        {
            return device.Properties;
        }

        /// <summary>
        /// 获取设备SD卡路径
        /// </summary>
        public string GetSDCardPath(Device device)
        {
            return AndroidHelper.Instance.GetSDCardPath(device);
        }

        /// <summary>
        /// 获取手机是否Root信息
        /// </summary>
        public bool IsRoot(Device device)
        {
            return device.IsRoot;
        }

        /// <summary>
        /// 设备是否可用
        /// </summary>
        public bool IsValid(Device device)
        {
            if (device.Status == EnumDeviceStatus.Online || device.Status == EnumDeviceStatus.Recovery || device.Status == EnumDeviceStatus.InUse)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 拷贝指定文件
        /// </summary>
        public string CopyFile(Device device, string source, string targetPath, IAsyncProgress asyn)
        {
            return AndroidHelper.Instance.CopyFile(device, source, targetPath, asyn);
        }

        /// <summary>
        /// 读取手机指定文件目录的内容
        /// </summary>
        public string ReadFile(Device device, string file)
        {
            return AndroidHelper.Instance.ReadFile(device, file);
        }

        /// <summary>
        /// 清除屏幕锁
        /// </summary>
        public void ClearScreenLock(Device device)
        {
            AndroidHelper.Instance.ClearScreenLock(device);
        }

        /// <summary>
        /// 恢复屏幕锁
        /// </summary>
        public void RecoveryScreenLock(Device device)
        {
            AndroidHelper.Instance.RecoveryScreenLock(device);
        }
    }
}
