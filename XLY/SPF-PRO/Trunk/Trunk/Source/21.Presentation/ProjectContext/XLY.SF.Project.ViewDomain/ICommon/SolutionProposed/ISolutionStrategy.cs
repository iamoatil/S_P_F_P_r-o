using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ViewDomain.Model;

/*************************************************
 * 创建人：Bob
 * 创建时间：2017/9/8
 * 类功能说明：
 * 1. 提供统一的解决方案入口点
 * 2. 目前解决方案具体返回值为定义，等以后正式开始再定义
 *
 *************************************************/

namespace XLY.SF.Project.ViewDomain.ICommon
{
    /// <summary>
    /// 解决方案适配
    /// </summary>
    public interface ISolutionAdapter
    {
        /// <summary>
        /// 方案适配
        /// </summary>
        /// <param name="dev">适配的设备</param>
        /// <param name="solutionSource">匹配的方案列表</param>
        /// <returns>匹配到的结果集合</returns>
        StrategyElement[] SolutionAdaptation(Device dev, StrategyElement[] solutionSource);
    }
}
