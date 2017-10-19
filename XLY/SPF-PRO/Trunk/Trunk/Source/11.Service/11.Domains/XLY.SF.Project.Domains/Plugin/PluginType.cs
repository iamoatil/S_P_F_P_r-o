using System;

/* ==============================================================================
* Description：PluginType  
* Author     ：Fhjun
* Create Date：2017/4/11 14:41:13
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 支持的插件类型
    /// </summary>
    public enum PluginType
    {
        ///// <summary>
        ///// 设备检测插件
        ///// </summary>
        //SpfDeviceDetection,
        ///// <summary>
        ///// 数据泵插件
        ///// </summary>
        //SpfDataPump,
        /// <summary>
        /// 数据解析插件
        /// </summary>
        SpfDataParse,
        /// <summary>
        /// 数据展示插件
        /// </summary>
        SpfDataView,
        /// <summary>
        /// 数据预览插件
        /// </summary>
        SpfDataPreview,
        /// <summary>
        /// 综合分析插件
        /// </summary>
        SpfComprehensiveAnalysis,
        ///// <summary>
        ///// 镜像插件
        ///// </summary>
        //SpfMirror,
        ///// <summary>
        ///// 镜像提取插件
        ///// </summary>
        //SpfMirrorExract,
        /// <summary>
        /// 本地文件提取插件(含镜像提取）
        /// </summary>
        SpfLocalFileExract,
        /// <summary>
        /// 本地文件夹提取插件
        /// </summary>
        SpfLocalFolderExract,
        /// <summary>
        /// 导出报表模板插件
        /// </summary>
        SpfReportModule,
        /// <summary>
        /// 导出报表插件
        /// </summary>
        SpfReport,
    }

    /// <summary>
    /// 脚本编写的语言
    /// </summary>
    public enum PluginLanguage
    {
        JavaScript,
        Python36,
        Html,
    }

    /// <summary>
    /// 插件状态，用于插件的管理
    /// </summary>
    [Flags]
    public enum PluginState : ushort
    {
        /// <summary>
        /// 无任何状态
        /// </summary>
        None = 0,
        /// <summary>
        /// 状态正常，并可以使用
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 被禁用
        /// </summary>
        Disabled = 2,
        /// <summary>
        /// 被卸载移除
        /// </summary>
        Removed = 4,
        /// <summary>
        /// 错误的插件（比如格式错误，无法使用）
        /// </summary>
        Invalid = 8,
    }
}
