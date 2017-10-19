using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Language;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Adapter.Loader
{
    //[Export("PluginLoader", typeof(IPluginLoader))]
    public class JavascriptPluginLoader : AbstractPluginLoader
    {
        public const string DESKEY = "#s^XLY_DESKEY_1986,11+15";

        public const string JS_EXT = ".js";
        public const string RELEASE_JS_EXT = ".pluginjs";

        protected override void LoadPlugin(IAsyncProgress asyn)
        {
            List<IPlugin> pluginList = new List<IPlugin>();

            string dir = SystemContext.Instance.CurLanguage == LanguageType.Cn ? FileHelper.GetPhysicalPath("\\Script\\cn")
                : FileHelper.GetPhysicalPath("\\Script\\en");
            
            var res = System.Threading.Tasks.Parallel.ForEach(FileHelper.GetFiles(dir, new[] { JS_EXT, RELEASE_JS_EXT }), (s) =>
            {
                var plug = this.LoadFile(s.FullName);
                lock (pluginList)
                {
                    if (plug != null)
                    {
                        pluginList.Add(plug);
                    }
                }
                System.Threading.Thread.Sleep(20);
            });
 
            Plugins = pluginList;
        }
        
        private IPlugin LoadFile(string file)
        {
            try
            {
                var plug = TryParseScritpPluginFile(file);
                return plug;
            }
            catch
            {
                
            }
            return null;
        }

        /// <summary>
        /// 解析脚本插件
        /// </summary>
        private DataJSScriptPlugin TryParseScritpPluginFile(string file)
        {
            if (String.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
            {
                throw new Exception("No Plugin File");
            }

            string jsContent = file.EndsWith(JS_EXT) ? File.ReadAllText(file) : CryptographyHelper.DecodeDES(File.ReadAllText(file));
            return TryParseScritpPlugin(jsContent);
        }

        /// <summary>
        /// 解析脚本插件
        /// </summary>
        private DataJSScriptPlugin TryParseScritpPlugin(string content)
        {
            if (String.IsNullOrEmpty(content))
            {
                throw new Exception("Plugin content is empty");
            }
            var plug = new DataJSScriptPlugin();
            var reg = new Regex(@"(?<=\[config\]).*(?=\[config\])",
                                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture);
            var config = reg.Match(content).Value;
            var js = content;
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo()
            {
                ScriptObject = js,
                PluginType = PluginType.SpfDataParse
            };
            //解析xml配置，装配
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(config);
            var root = doc.DocumentElement;
            if (root == null || root.Name != "plugin")
            {
                throw new Exception("No Plugin Config");
            }
            this.ReadPluginConfig(pluginInfo, root);
            this.ReadSourceNode(pluginInfo, root);
            this.ReadIncludeFile(pluginInfo, root);
            this.ReadDataNode(pluginInfo, root);
            plug.PluginInfo = pluginInfo;
            return plug;
        }

        /// <summary>
        /// 读取节点基本配置
        /// </summary>
        private void ReadPluginConfig(DataParsePluginInfo plugin, XmlNode node)
        {
            var n = node.GetSafeAttributeValue("name").Replace("，", ",");
            var g = node.GetSafeAttributeValue("group").Replace("，", ",");
            var ns = n.Split(',');
            var gs = g.Split(',');
            if (ns.Length >= 2)
            {
                plugin.OrderIndex = int.Parse(ns[1]);
            }
            if (gs.Length >= 2)
            {
                plugin.GroupIndex = int.Parse(gs[1]);
            }

            plugin.Name = ns[0];
            plugin.Group = gs[0];
            plugin.DeviceOSType = this.ReadEnum(node, "devicetype", plugin.DeviceOSType);
            plugin.Pump = this.ReadEnum(node, "pump", plugin.Pump);
            plugin.AppName = node.GetSafeAttributeValue("app");
            plugin.VersionStr = node.GetSafeAttributeValue("version").ToSafeString();
            plugin.Description = node.GetSafeAttributeValue("description");
            plugin.Icon = node.GetSafeAttributeValue("icon").Replace('/', '\\');
            plugin.Manufacture = node.GetSafeAttributeValue("manufacture");

        }

        /// <summary>
        /// 读取并转化节点中的枚举属性
        /// </summary>
        private T ReadEnum<T>(XmlNode node, string att, T dv)
        {
            var value = node.GetSafeAttributeValue(att);
            try
            {
                if (value.IsValid())
                {
                    return value.Replace("，", ",").ToEnum<T>();
                }
                return dv;
            }
            catch (Exception)
            {
                throw new ApplicationException("No Data Config");
            }
        }

        #region ReadSourceNode
        /// <summary>
        /// 读取节点的源数据文件路径定义
        /// </summary>
        private void ReadSourceNode(DataParsePluginInfo plugin, XmlNode node)
        {
            var source = node.SelectSingleNode("source");
            if (source == null || !source.HasChildNodes)
            {
                return;
            }
            var values = source.SelectNodes("value");
            if (values == null || values.Count <= 0)
            {
                return;
            }
            List<String> items = new List<string>();
            foreach (XmlNode v in values)
            {
                items.Add(v.InnerText);
            }
            plugin.SourcePath = new SourceFileItems();
            plugin.SourcePath.AddItems(items);
        }
        #endregion

        #region ReadIncludeFile
        /// <summary>
        /// 读取引用的外部文件，该文件路径为相对于exe程序的相对路径
        /// </summary>
        private void ReadIncludeFile(DataParsePluginInfo plugin, XmlNode node)
        {
            var source = node.SelectSingleNode("include");
            if (source == null || !source.HasChildNodes)
            {
                return;
            }
            var values = source.SelectNodes("script");
            if (values == null || values.Count <= 0)
            {
                return;
            }
            foreach (XmlNode v in values)//通过相对路径得到绝对路径
            {
                System.Uri baseUri = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "main.exe"));
                Uri addr = new Uri(baseUri, v.InnerText);
                if (System.IO.File.Exists(addr.LocalPath))
                {
                    string content = System.IO.File.ReadAllText(addr.LocalPath, Encoding.UTF8);
                    //content = LanguageHelper.SetScriptFileLanguage(content);        //修改脚本中的语言，比如改为英文
                    plugin.ScriptObject += Environment.NewLine + Environment.NewLine + content;      //将引用的文件加到末尾
                }
            }
        }
        #endregion

        #region ReadDataNode
        /// <summary>
        /// 读取节点的自定义数据格式
        /// </summary>
        private void ReadDataNode(DataParsePluginInfo plugin, XmlNode node)
        {
            var datas = node.SelectNodes("data");
            if (datas == null || datas.Count <= 0)
            {
                return;
            }

        }


        /// <summary>
        /// 添加固定数据列MD5
        /// </summary>
        /// <returns></returns>
        private DataItem GetMD5Item()
        {
            DataItem item = new DataItem
            {
                Name = "MD5",
                Code = "MDFString",
                Type = EnumColumnType.String,
                Width = 100,
                Format = string.Empty,
                Order = EnumOrder.None,
                Alignment = EnumAlignment.Left
            };
            return item;
        }

        #endregion
    }
}
