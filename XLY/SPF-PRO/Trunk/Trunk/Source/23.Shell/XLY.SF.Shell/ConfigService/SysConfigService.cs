using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.SystemKeys;

namespace XLY.SF.Shell.ConfigService
{
    /// <summary>
    /// 系统配置服务
    /// </summary>
    [Export(CoreExportKeys.SysConfigHelper, typeof(ISystemConfigService))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public class SysConfigService : ISystemConfigService
    {
        public SysConfigService()
        {

        }

        /// <summary>
        /// 获取系统配置文件中对应Key的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetSysConfigValueByKey(string key)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(key))
                result = ConfigurationManager.AppSettings.Get(key);
            return result;
        }

        [Obsolete]
        public void SaveSysConfig()
        {

        }

        /// <summary>
        /// 根据Key修改系统配置文件中的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetSysConfigValueBykey(string key, object value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);
                AppSettingsSection sec = (AppSettingsSection)config.AppSettings;
                if (sec.Settings.AllKeys.Contains(key))
                {
                    sec.Settings[key].Value = value.ToString();
                    config.Save();
                    ConfigurationManager.RefreshSection("appSettings");
                    return true;
                }
            }
            return false;
        }
    }
}
