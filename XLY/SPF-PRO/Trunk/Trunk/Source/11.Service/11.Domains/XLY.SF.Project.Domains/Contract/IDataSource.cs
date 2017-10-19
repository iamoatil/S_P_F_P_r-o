using System;
using System.Collections.Generic;
using XLY.SF.Project.Domains.Contract;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据源定义
    /// </summary>
    public interface IDataSource : ITraverse
    {
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        Guid Key { get; set; }

        /// <summary>
        /// 插件定义信息
        /// </summary>
        IPluginInfo PluginInfo { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        IDataItems Items { get; set; }

        /// <summary>
        /// 构建数据
        /// </summary>
        void BuildParent();

        /// <summary>
        /// 数据提取时使用的任务路径 ，该属性用于修正任务包路径修改后导致的路径不正确的问题
        /// </summary>
        string DataExtractionTaskPath { get; set; }

        /// <summary>
        /// 当前任务路径
        /// </summary>
        string CurrentTaskPath { get; set; }

        int Total { get; }

        IEnumerable<T> Filter<T>(params FilterArgs[] args);
    }
}
