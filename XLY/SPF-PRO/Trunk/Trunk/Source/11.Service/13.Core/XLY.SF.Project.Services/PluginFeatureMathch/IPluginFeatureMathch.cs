using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 特征匹配接口
    /// </summary>
    interface IPluginFeatureMathch
    {
        /// <summary>
        /// APP名称
        /// </summary>
        string AppName { get; set; }
        /// <summary>
        /// 操作系统类型
        /// </summary>
        EnumOSType OSType { get; set; }
        /// <summary>
        /// 手机品牌
        /// </summary>
        string Manufacture { get; set; }
        /// <summary>
        /// 匹配特征
        /// </summary>
        /// <param name="taskSourcePath">数据源路径</param>
        /// <returns></returns>
        PluginFeatureMathchResult TryMathch(string taskSourcePath);
    }
}
