using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Description：IFilterItem  
* Author     ：Fhjun
* Create Date：2017/3/23 10:46:28
* ==============================================================================*/

namespace XLY.SF.Project.Domains.Contract
{
    /// <summary>
    /// 提供对数据的查询过滤接口
    /// </summary>
    public interface IFilterItem
    {
        /// <summary>
        /// 过滤数据
        /// </summary>
        /// <param name="args">过滤参数，可以设置多个过滤参数</param>
        /// <returns>true符合条件，false不符合过滤条件</returns>
        bool Filter(params FilterArgs[] args);

        /// <summary>
        /// 查询数据，并返回数据结果
        /// </summary>
        /// <param name="args">查询参数(比如获取数据中的身份证号码)</param>
        /// <returns></returns>
        List<string> Search(params FilterArgs[] args);

        /// <summary>
        /// 统计数据，并返回数据出现的次数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        int Static(params FilterArgs[] args);

        /// <summary>
        /// 是否过滤
        /// </summary>
        bool IsVisible { get; set; }

    }
}
