using System;
using System.Collections.Generic;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 设备信息领域逻辑
    /// </summary>
    public partial class Device
    {
        /// <summary>
        /// 设备是否正常链接
        /// </summary>
        public bool IsValid()
        {
            return DeviceManager.IsValid(this);
        }

        #region DeviceUseableValidation（验证设备可用性）

        /// <summary>
        /// 验证设备可用性
        /// </summary>
        public void DeviceUseableValidation()
        {
            switch (Status)
            {
                case EnumDeviceStatus.InUse:
                    //throw new ApplicationException(LanguageHelperSingle.Instance.Language.DeviceLanguage.DeviceUseableValidation_InUse);
                    throw new ApplicationException(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.DeviceLanguage_DeviceUseableValidation_InUse));
                case EnumDeviceStatus.None:
                case EnumDeviceStatus.Offline:
                    //throw new ApplicationException(LanguageHelperSingle.Instance.Language.DeviceLanguage.DeviceUseableValidation_Offline);
                    throw new ApplicationException(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.DeviceLanguage_DeviceUseableValidation_Offline));
            }

            if (!DeviceManager.IsValid(this))
            {
                //throw new ApplicationException(LanguageHelperSingle.Instance.Language.DeviceLanguage.DeviceUseableValidation_IsInvalid);
                throw new ApplicationException(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.DeviceLanguage_DeviceUseableValidation_IsInvalid));
            }
        }

        #endregion

        #region SetDevice status

        /// <summary>
        /// 设置指定设备状态为忙
        /// </summary>
        public void SetDeviceBusy()
        {
            Status = EnumDeviceStatus.InUse;
        }

        /// <summary>
        /// 设置指定设备状态为空闲可使用
        /// </summary>
        public void SetDeviceFree()
        {
            Status = EnumDeviceStatus.Online;
        }

        #endregion

        #region CopyFile
        /// <summary>
        /// 拷贝指定文件
        /// </summary>
        /// <param name="source">原始路径</param>
        /// <param name="targetPath">目标拷贝路径</param>
        /// <param name="asyn"></param>
        /// <returns></returns>
        public string CopyFile(string source, string targetPath, IAsyncProgress asyn)
        {
            return DeviceManager.CopyFile(this, source, targetPath, asyn);
        }
        #endregion

        #region ReadFile
        /// <summary>
        /// 读取手机指定文件目录的内容
        /// </summary>
        public string ReadFile(string file)
        {
            return DeviceManager.ReadFile(this, file);
        }
        #endregion

        #region GetPartitons
        /// <summary>
        /// 获取设备分区结构列表
        /// </summary>
        public List<Partition> GetPartitons()
        {
            return DeviceManager.GetPartitons(this);
        }
        #endregion

        /// <summary>
        /// 清除屏幕锁
        /// </summary>
        public void ClearScreenLock()
        {
            DeviceManager.ClearScreenLock(this);
        }

        /// <summary>
        /// 恢复屏幕锁
        /// </summary>
        public void RecoveryScreenLock()
        {
            DeviceManager.RecoveryScreenLock(this);
        }

        /// <summary>
        /// 获取设备属性信息
        /// </summary>
        public Dictionary<string, string> GetProperties()
        {
            return DeviceManager.GetProperties(this);
        }

        /// <summary>
        /// 查找所有已安装应用列表
        /// </summary>
        public List<AppEntity> FindInstalledApp()
        {
            return DeviceManager.FindInstalledApp(this);
        }

        /// <summary>
        /// 查找所有已卸载应用列表
        /// </summary>
        public List<AppEntity> FindUnInstalledApp()
        {
            return DeviceManager.FindUnInstalledApp(this);
        }

        /// <summary>
        /// 获取指定键值名称的属性信息
        /// </summary>
        public string GetProperty(string name)
        {
            if (Properties.IsInvalid()) return string.Empty;
            if (Properties.ContainsKey(name))
            {
                return Properties[name];
            }
            return string.Empty;
        }
    }

}
