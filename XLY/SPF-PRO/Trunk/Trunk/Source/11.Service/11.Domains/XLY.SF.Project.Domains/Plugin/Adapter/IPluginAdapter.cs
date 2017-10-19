using System;
using System.Collections.Generic;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 插件控制器接口
    /// </summary>
    public interface IPluginAdapter
    {
        void Initialization(IAsyncProgress asyn);

        /// <summary>
        /// 获取提取项列表
        /// </summary>
        /// <param name="source">数据泵</param>
        /// <returns></returns>
        ExtractItemCollection GetAllExtractItems(Pump source);

        /// <summary>
        /// 匹配插件（根据数据泵，即设备类型、设备操作系统类型、数据提取类型匹配插件）
        /// 匹配结果中，一个提取项可能包含多个版本的插件
        /// </summary>
        /// <param name="source">数据泵</param>
        /// <param name="extractItems">提取项</param>
        /// <returns></returns>
        Dictionary<ExtractItem, List<DataParsePluginInfo>> MatchPluginByPump(Pump source, IEnumerable<ExtractItem> extractItems);

        /// <summary>
        /// 匹配插件（根据特征库和APP版本信息匹配插件）
        /// 匹配结果只有一个插件
        /// </summary>
        /// <param name="pump">数据泵类型</param>
        /// <param name="appSourePath">APP本地数据根目录</param>
        /// <param name="appVersion">APP版本信息</param>
        /// <returns></returns>
        DataParsePluginInfo MatchPluginByApp(IEnumerable<DataParsePluginInfo> pluginList, Pump pump, string appSourePath, Version appVersion);

        /// <summary>
        /// 执行插件（内部处理异常）
        /// </summary>
        /// <returns></returns>
        void ExecutePlugin(DataParsePluginInfo plugin, IAsyncProgress asyn, Action<IDataSource> callback);

        //object ExecuteJs(string jsCode);

        /// <summary>
        /// 智能匹配插件 按过滤器条件
        /// </summary>
        /// <param name="pluginList"></param>
        /// <param name="pluginFilters"></param>
        /// <returns></returns>
        List<DataParsePluginInfo> SmartMatchPlugin(List<DataParsePluginInfo> pluginList, List<PluginMatchFilter> filterList);
    }
}
