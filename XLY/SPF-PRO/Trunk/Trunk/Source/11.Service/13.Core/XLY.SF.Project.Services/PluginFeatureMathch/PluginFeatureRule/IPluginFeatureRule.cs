using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 验证规则接口
    /// </summary>
    interface IPluginFeatureRule
    {
        /// <summary>
        /// Rule ID
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 验证通过结果
        /// </summary>
        string Success { get; set; }

        /// <summary>
        /// 验证失败结果
        /// </summary>
        string Failure { get; set; }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="taskSourcePath">数据源路径</param>
        /// <returns></returns>
        string TryMathch(string taskSourcePath);
    }
}
