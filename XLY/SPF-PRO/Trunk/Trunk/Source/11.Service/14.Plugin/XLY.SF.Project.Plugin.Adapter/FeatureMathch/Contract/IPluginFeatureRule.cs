// ***********************************************************************
// Assembly:XLY.SF.Project.Plugin.Adapter.FeatureMathch
// Author:Songbing
// Created:2017-04-12 09:52:18
// Description:插件特征匹配规则接口
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace XLY.SF.Project.Plugin.Adapter.FeatureMathch
{
    /// <summary>
    /// 插件特征匹配规则接口
    /// </summary>
    internal interface IFeatureMathchRule
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
        /// <param name="sourcePath">数据源路径</param>
        /// <returns></returns>
        string TryMathch(string sourcePath);
    }
}
