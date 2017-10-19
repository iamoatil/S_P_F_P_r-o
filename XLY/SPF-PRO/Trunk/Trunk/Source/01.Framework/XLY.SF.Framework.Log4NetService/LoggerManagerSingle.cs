using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Log4NetService.LoggerEnum;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:54:09
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Log4NetService
{
    public class LoggerManagerSingle
    {
        #region Single

        private ILog _iLog;

        private static object _objLock = new object();
        private static LoggerManagerSingle _instance;

        private LoggerManagerSingle()
        {
            _iLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log4net.Config.XmlConfigurator.Configure();
        }

        public static LoggerManagerSingle Instance
        {
            get
            {
                if (_instance == null)
                    lock (_objLock)
                        if (_instance == null)
                            _instance = new LoggerManagerSingle();
                return _instance;
            }
        }

        #endregion

        #region 日志

        #region 错误日志

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="errorMsg">信息内容</param>
        public void Error(Exception ex, string errorMsg = null)
        {
#if DEBUG
            Console.WriteLine("【异常】{0}【{1:yyyyMMdd HH:mm:ss}】错误信息：{2}", errorMsg, DateTime.Now, ex);
#endif
            if (string.IsNullOrEmpty(errorMsg))
                _iLog.Error(ex);
            else
                _iLog.Error(errorMsg, ex);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="errorMsg">信息内容</param>
        public void Error(string errorMsg)
        {
#if DEBUG
            Console.WriteLine("【异常】{0}【{1:yyyyMMdd HH:mm:ss}】", errorMsg, DateTime.Now);
#endif
            _iLog.Error(errorMsg);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="errorMsg">信息内容</param>
        public void Error(string errorMsg, Exception ex)
        {
#if DEBUG
            Console.WriteLine("【异常】{0}【{1:yyyyMMdd HH:mm:ss}】错误信息：{2}", errorMsg, DateTime.Now, ex);
#endif
            if (string.IsNullOrEmpty(errorMsg))
                _iLog.Error(ex);
            else
                _iLog.Error(errorMsg, ex);
        }

        #endregion

        #region 调试日志

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="debugMsg">信息内容</param>
        public void Debug(Exception ex, string debugMsg = null)
        {
#if DEBUG
            Console.WriteLine("【调试】{0}【{1:yyyyMMdd HH:mm:ss}】错误信息：{2}", debugMsg, DateTime.Now, ex);
#endif
            if (string.IsNullOrEmpty(debugMsg))
                _iLog.Debug(ex);
            else
                _iLog.Debug(debugMsg, ex);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="debugMsg">信息内容</param>
        public void Debug(string debugMsg)
        {
#if DEBUG
            Console.WriteLine("【调试】{0}【{1:yyyyMMdd HH:mm:ss}】", debugMsg, DateTime.Now);
#endif
            _iLog.Debug(debugMsg);
        }

        #endregion

        #region 普通日志

        /// <summary>
        /// 一般信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="infoMsg">信息内容</param>
        public void Info(Exception ex, string infoMsg = null)
        {
#if DEBUG
            Console.WriteLine("【一般】{0}【{1:yyyyMMdd HH:mm:ss}】错误信息：{2}", infoMsg, DateTime.Now, ex);
#endif
            if (string.IsNullOrEmpty(infoMsg))
                _iLog.Info(ex);
            else
                _iLog.Info(infoMsg, ex);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="infoMsg">信息内容</param>
        public void Info(string infoMsg)
        {
#if DEBUG
            Console.WriteLine("【一般】{0}【{1:yyyyMMdd HH:mm:ss}】", infoMsg, DateTime.Now);
#endif
            _iLog.Info(infoMsg);
        }

        #endregion

        #region 警告日志

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="warnMsg">信息内容</param>
        public void Warn(Exception ex, string warnMsg = null)
        {
#if DEBUG
            Console.WriteLine("【警告】{0}【{1:yyyyMMdd HH:mm:ss}】错误信息：{2}", warnMsg, DateTime.Now, ex);
#endif
            if (string.IsNullOrEmpty(warnMsg))
                _iLog.Warn(ex);
            else
                _iLog.Warn(warnMsg, ex);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="warnMsg">信息内容</param>
        public void Warn(string warnMsg)
        {
#if DEBUG
            Console.WriteLine("【警告】{0}【{1:yyyyMMdd HH:mm:ss}】", warnMsg, DateTime.Now);
#endif
            _iLog.Warn(warnMsg);
        }

        #endregion

        #endregion
    }
}
