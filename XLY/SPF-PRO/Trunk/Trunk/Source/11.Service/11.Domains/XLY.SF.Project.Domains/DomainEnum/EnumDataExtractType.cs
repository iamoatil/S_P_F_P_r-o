// ***********************************************************************
// Assembly:XLY.SF.Project.Domains
// Author:Songbing
// Created:2017-03-27 11:27:14
// Description:数据提取工作模式
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据提取工作模式
    /// </summary>
    public enum EnumDataExtractWorkMode
    {
        /// <summary>
        /// 普通模式，即提取完所有应用的数据文件后再依次执行插件
        /// </summary>
        Normal,

        /// <summary>
        /// 同步提取模式，即提取完A的数据文件后执行A的插件，再继续提取B的数据文件
        /// </summary>
        Followers,

        /// <summary>
        /// 半异步提取模式,即提取完A的数据文件后马上提取B的数据文件，同时执行A的插件
        /// </summary>
        HalfAsync,

        /// <summary>
        /// 异步提取模式，即多个应用同时提取
        /// </summary>
        Async,
    }
}
