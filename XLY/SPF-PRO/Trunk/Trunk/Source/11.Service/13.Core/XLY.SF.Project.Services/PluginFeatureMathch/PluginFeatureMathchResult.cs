using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{/// <summary>
 /// 特征匹配结果
 /// </summary>
    public class PluginFeatureMathchResult
    {
        /// <summary>
        /// 是否匹配成功
        /// </summary>
        public bool IsSuccessed { get; set; }

        /// <summary>
        /// APP名字
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// APP版本号
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// 操作系统类型
        /// </summary>
        public EnumOSType OSType { get; set; }

        /// <summary>
        /// 手机品牌
        /// </summary>
        public string Manufacture { get; set; }

        public override string ToString()
        {
            if (IsSuccessed)
            {
                return string.Format("匹配成功 {0} {1} {2} Ver{3}", OSType.GetEnumFlagDescription(), Manufacture, AppName, AppVersion);
            }
            else
            {
                return "匹配失败";
            }
        }

    }
}
