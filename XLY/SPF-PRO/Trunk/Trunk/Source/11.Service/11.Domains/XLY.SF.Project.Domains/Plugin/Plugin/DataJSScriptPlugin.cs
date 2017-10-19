using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// JavaScript脚本插件
    /// </summary>
    [Export(PluginExportKeys.PluginScriptKey, typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataJSScriptPlugin : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public override object Execute(object arg, IAsyncProgress progress)
        {
            var task = arg as SPFTask;
            DataParsePluginInfo p = PluginInfo as DataParsePluginInfo;
            var files = task.SourceFiles.Where(s => p.SourcePath.Any(k => k.Config == s.Config));
            var str = string.Empty;
            if (files.IsValid())
            {
                str = Serializer.JsonSerilize(files.Select(s => s.Local));
            }
            var js = p.ScriptObject.Replace("$source", str);

            var obj = ExecuteJs(js, progress);
            return null;
        }

        public override void Dispose()
        {

        }

        private IScriptContext GetContext()
        {
            var plugins = IocManagerSingle.Instance.GetMetaParts<IScriptContext, IMetaScriptLanguage>("ScriptContext");
            foreach (var loader in plugins)
            {
                if (loader.Metadata.PluginLanguage == PluginLanguage.JavaScript)
                {
                    return loader.Value;
                }
            }
            return null;
        }

        private static string _ScriptContextRunName = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "XLY.SF.Tools.ScriptContextRun.exe");

        [MethodImpl(MethodImplOptions.Synchronized)]
        private object ExecuteJs(string jsCode, IAsyncProgress progress)
        {
            if (!System.IO.File.Exists(_ScriptContextRunName))
            {
                var context = GetContext();
                return context?.Execute(jsCode, progress);// new ScriptContext().Execute(jsCode);
            }
            else
            {
                string res = "";
                try
                {
                    string jsFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, String.Format("JS_{0}.jscode", Guid.NewGuid().ToString()));
                    System.IO.File.WriteAllText(jsFile, jsCode, System.Text.Encoding.UTF8);

                    Process pro = new Process();
                    pro.StartInfo.FileName = _ScriptContextRunName;
                    pro.StartInfo.Arguments = string.Format("\"{0}\"", jsFile); ;
                    pro.StartInfo.UseShellExecute = false;
                    pro.StartInfo.CreateNoWindow = true;

                    pro.Start();
                    pro.WaitForExit();

                    if (System.IO.File.Exists(jsFile))
                    {
                        res = System.IO.File.ReadAllText(jsFile, System.Text.Encoding.UTF8);
                        System.IO.File.Delete(jsFile);
                    }
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error("执行脚本失败!", ex);
                }

                return res;
            }
        }
    }
}
