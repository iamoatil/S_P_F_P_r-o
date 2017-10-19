using System;
using Noesis.Javascript;

namespace XLY.SF.Project.ScriptEngine.Engine
{
    /// <summary>
    /// 脚本异常
    /// </summary>
    public class ScritpException : ApplicationException
    {
        public ScritpException(string js, Exception ex)
            : base(string.Format("执行JS出现异常", js), ex)
        { }

        public ScritpException(string js, JavascriptException ex)
            : base(string.Format("执行JS出现异常", ex.StartColumn
            , ex.EndColumn, ex.Line, ex.Source), ex)
        { }
    }
}
