using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/27 14:01:11
 * 接口功能说明：
 * 针对系统配置的操作
 * 通过MEF导出使用
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.CoreInterface
{
    public interface ISystemConfigService
    {
        /// <summary>
        /// 保存系统配置文件
        /// </summary>
        void SaveSysConfig();

        /// <summary>
        /// 根据Key获取对应的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetSysConfigValueByKey(string key);

        /// <summary>
        /// 修改系统配置文件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SetSysConfigValueBykey(string key, object value);
    }
}
