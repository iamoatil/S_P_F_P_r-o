using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.IScriptContext
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/12 10:55:24
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 脚本执行上下文
    /// </summary>
    public interface IScriptContext
    {
        /// <summary>
        /// 执行脚本，并返回执行结果
        /// </summary>
        /// <param name="script">在js脚本中为脚本内容，在Python脚本中为脚本路径</param>
        /// <param name="asyn">执行进度消息通知</param>
        /// <param name="argrument">执行主函数传入的参数</param>
        /// <param name="paramValues">在语言中动态设置的参数</param>
        /// <param name="isThrowExeception">如果执行出现错误，是否抛出异常</param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        object Execute(string script, IAsyncProgress asyn, object[] argrument = null, Dictionary<string, object> paramValues = null, bool isThrowExeception = true);
    }
}
