using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.DataFilter
{
    /// <summary>
    /// 表示支持过滤功能。
    /// </summary>
    public interface IFilterable
    {
        /// <summary>
        /// 获取数据源的数据提供器。
        /// </summary>
        /// <returns>数据提供器。</returns>
        IFilterDataProvider Provider { get; }
    }
}
