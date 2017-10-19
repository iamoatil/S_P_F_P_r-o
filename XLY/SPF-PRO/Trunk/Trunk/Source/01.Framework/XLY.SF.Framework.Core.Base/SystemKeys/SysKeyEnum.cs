using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:59:18
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.SystemKeys
{
    /// <summary>
    /// 系统消息定义
    /// </summary>
    public class SystemKeys
    {
        /// <summary>
        /// 重启程序
        /// </summary>
        public const string RestartProgram = "SystemKey_RestartProgram";
        /// <summary>
        /// 注销用户登录
        /// </summary>
        public const string LogoutUser = "SystemKey_LogoutUser";
        /// <summary>
        /// 结束程序，Parameter中带有提示信息则需要用于确认
        /// </summary>
        public const string ShutdownProgram = "SystemKey_ShutdownProgram";
        /// <summary>
        /// 打开一个新窗体
        /// </summary>
        public const string OpenNewWindow = "SystemKey_OpenNewWindow";
        /// <summary>
        /// 打开一个模式对话框
        /// </summary>
        public const string OpenNewDialogWindow = "SystemKey_OpenNewDialogWindow";
        /// <summary>
        /// 显示消息提示框
        /// </summary>
        public const string ShowMessageBox = "SystemKey_ShowMessageBox";
        /// <summary>
        /// 主界面内导航
        /// </summary>
        public const string MainUcNavigation = "SystemKey_MainUcNavigation";
        /// <summary>
        /// 关闭
        /// </summary>
        public const string CloseWindow = "SystemKey_CloseWindow";
        /// <summary>
        /// 登录完成
        /// </summary>
        public const string LoginComplete = "SystemKey_LoginComplete";
        /// <summary>
        /// 写入操作日志
        /// </summary>
        public const string WirteOperationLog = "SystemKey_WirteOperationLog";
    }

    /// <summary>
    /// 框架级导出关键字
    /// </summary>
    public class CoreExportKeys
    {
        /// <summary>
        /// 日志服务导出关键字
        /// </summary>
        public const string LogService = "XLY.LogServiceKey";

        /// <summary>
        /// 数据库服务
        /// </summary>
        public const string DatabaseService = "XLY.DatabaseService";

        /// <summary>
        /// 配置文件服务
        /// </summary>
        public const string SysConfigHelper = "XLY.SysConfigHelper";
    }
}
