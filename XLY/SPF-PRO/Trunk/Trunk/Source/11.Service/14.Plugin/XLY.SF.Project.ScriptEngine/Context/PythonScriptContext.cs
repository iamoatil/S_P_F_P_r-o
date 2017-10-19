using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.Hosting;
using XLY.SF.Project.Domains;
using Python.Runtime;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Framework.Core.Base.CoreInterface;
using System.ComponentModel.Composition;

/* ==============================================================================
* Description：PythonScriptContext  
* Author     ：Fhjun
* Create Date：2017/4/1 14:36:01
* ==============================================================================*/

namespace XLY.SF.Project.ScriptEngine.Context
{
    /// <summary>
    /// 执行Python脚本
    /// </summary>
    [ExportMetadata("PluginLanguage", PluginLanguage.Python36)]
    [Export("ScriptContext", typeof(IScriptContext))]
    public class PythonScriptContext : IScriptContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        /// <param name="asyn"></param>
        /// <param name="argrument"></param>
        /// <param name="paramValues"></param>
        /// <param name="isThrowExeception"></param>
        /// <returns></returns>
        public object Execute(string script, IAsyncProgress asyn, object[] argrument = null, Dictionary<string, object> paramValues = null, bool isThrowExeception = true)
        {
            try
            {
                if (!FileHelper.IsValid(script))
                {
                    throw new Exception("Python File '" + script + "' is not exist!");
                }

                InitEnvriment(script);
                dynamic py = LoadPythonScript(script);
                var result = py.main(argrument, asyn);
                return result;
            }
            catch(Exception e)
            {
                if (isThrowExeception)
                {
                    throw e;
                }
                return null;
            }
            finally
            {
                Dispose();
            }
        }

        private IntPtr _gs;
        private bool _isInit = false;
        /// <summary>
        /// 初始化Python环境
        /// </summary>
        /// <param name="script"></param>
        private void InitEnvriment(string script)
        {
            string pyDir = Path.GetDirectoryName(script);
            PythonEngine.PythonHome = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Python36");
            PythonEngine.Initialize();
            _gs = PythonEngine.AcquireLock();
            _isInit = true;

            IntPtr str = Runtime.PyString_FromString(pyDir);
            IntPtr path = Runtime.PySys_GetObject("path");
            Runtime.PyList_Append(path, str);
        }

        /// <summary>
        /// 通过import的方式获取脚本对象
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        private PyObject LoadPythonScript(string script)
        {
            string scriptPath = Path.GetFileName(script).Replace(Path.GetExtension(script), "");        //获取文件名(没有后缀名)
            PyObject plugin = PythonEngine.ImportModule(scriptPath);        //通过import的方式导入脚本
            return plugin;
        }

        private void Dispose()
        {
            if (_isInit)
            {
                PythonEngine.ReleaseLock(_gs);
                PythonEngine.Shutdown();
                _isInit = false;
            }
        }
    }
}
