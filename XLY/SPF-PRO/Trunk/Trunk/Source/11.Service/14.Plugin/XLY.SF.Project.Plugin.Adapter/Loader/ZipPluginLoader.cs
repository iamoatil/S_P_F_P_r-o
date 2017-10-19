// ***********************************************************************
// Assembly:XLY.SF.Project.Plugin.Adapter
// Author:Songbing
// Created:2017-04-11 14:04:39
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Xml.Linq;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Contract.DataItemContract;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Language;
using ProjectExtend.Context;
using System.IO.Compression;

namespace XLY.SF.Project.Plugin.Adapter
{
    /// <summary>
    /// 脚本插件加载器
    /// </summary>
    [Export(PluginExportKeys.PluginLoaderKey, typeof(IPluginLoader))]
    public class ZipPluginLoader : AbstractPluginLoader
    {
        public const string DebugScriptExtension = ".zip";          //未加密的脚本文件后缀
        public const string ReleaseScriptExtension = ".xlyx";         //已加密的脚本文件后缀
        public const string PluginConfigFileName = "plugin.config";       //配置文件名
        private const string RarPassword = @"#soif!@1751fsd,84&^%23@())wer32''fsd!!**32199.sfd";   //密码
        private const string DesPassword = @"84@#U*;[FSD848afs@f,lSW";   //密码
        private static readonly string DefaultUnrarPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());  //默认的临时解压缩路径

        /* 脚本文件结构:
         
         --Android_QQ_5.1.0.zip
            |--plugin.config        (主配置文件，名称固定)
            |--main.py              (主插件，名称固定)
            |--icon.png             (图标，名称固定)
            |--chalib               (数据恢复特征库文件夹)
            |--其它文件及文件夹     
         
        */

        protected override void LoadPlugin(IAsyncProgress asyn)
        {
            List<IPlugin> plugins = new List<IPlugin>();
            //string dir = SystemContext.Instance.CurLanguage == LanguageType.Cn ? FileHelper.GetPhysicalPath("\\Script\\cn")
            //    : FileHelper.GetPhysicalPath("\\Script\\en");
            string dir = FileHelper.GetPhysicalPath("\\Script\\cn");
            foreach (var file in FileHelper.GetFiles(dir, new[] { DebugScriptExtension, ReleaseScriptExtension }))
            {
                try
                {
                    bool isPassword = file.Extension.Equals(ReleaseScriptExtension, StringComparison.OrdinalIgnoreCase);

                    //解压缩文件到临时目录
                    string tmpDir = UnRarFile(file, isPassword);

                    //读取配置文件
                    AbstractZipPluginInfo pluginInfo = ReadPluginInfo(Path.Combine(tmpDir, PluginConfigFileName));

                    //读取脚本文件内容
                    pluginInfo.ZipTempDirectory = tmpDir;
                    ReadScriptContent(pluginInfo, isPassword);

                    //生成插件实例
                    IPlugin plugin = GetPlugin(pluginInfo);
                    plugin.PluginInfo = pluginInfo;
                    plugins.Add(plugin);
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Warn(ex, string.Format("解析脚本发生异常！脚本文件：{0}", file.FullName));
                }
            }

            Plugins = plugins;
        }

        /// <summary>
        /// 解压缩文件到临时目录
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="isPassword"></param>
        /// <returns></returns>
        private string UnRarFile(FileInfo fi, bool isPassword)
        {
            string path = Path.Combine(DefaultUnrarPath, fi.Name);
            //WinRarHelper.UnRar(fi.FullName, path, isPassword ? RarPassword : null);
            if (isPassword)
            {

            }
            ZipFile.ExtractToDirectory(fi.FullName, path);
            return path;
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        private AbstractZipPluginInfo ReadPluginInfo(string configFile)
        {
            if (!FileHelper.IsValid(configFile))
            {
                throw new Exception("脚本配置文件不存在!");
            }
            Type pluginType = GetPluginTypeByConfigFile(configFile);
            var plugin = Serializer.DeSerializeFromXML(configFile, pluginType) as AbstractZipPluginInfo;
            if (plugin == null)
            {
                throw new Exception("脚本配置文件格式不正确，反序列化失败！");
            }

            //部分参数需要计算
            plugin.AfterReadConfigure();
            if (plugin is DataParsePluginInfo)
            {
                //DataParsePluginInfo p = (DataParsePluginInfo) plugin;
                ////动态创建数据类型
                //if (plugin.DataView != null)
                //{
                //    plugin.DataView.ForEach(dv => CreateDynamicType(dv));
                //}
            }

            return plugin;
        }

        /// <summary>
        /// 读取脚本文件内容
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="isPassword"></param>
        /// <returns></returns>
        private void ReadScriptContent(AbstractZipPluginInfo plugin, bool isPassword)
        {
            //如果未配置脚本文件名
            //if (string.IsNullOrWhiteSpace(plugin.ScriptFile))
            //{
            //    throw new Exception("未找到脚本文件!" + plugin.ScriptFile);
            //}
            plugin.ScriptObject = string.IsNullOrWhiteSpace(plugin.ScriptFile) ? plugin.ZipTempDirectory 
                : Path.Combine(plugin.ZipTempDirectory, plugin.ScriptFile);
            //plugin.ScriptObject = FileHelper.FileToUTF8String(plugin.ScriptFile);
            //if (isPassword)   //再次使用DES解密
            //{
            //    plugin.ScriptObject = CryptographyHelper.DecodeDES(plugin.ScriptObject, DesPassword);
            //}
        }

        /// <summary>
        /// 动态创建插件的类型
        /// </summary>
        private void CreateDynamicType(DataView dv)
        {
            if (dv == null)
            {
                throw new Exception("加载脚本时出错！DataView为空");
            }
            if (string.IsNullOrWhiteSpace(dv.Type))
            {
                throw new Exception("加载脚本时出错！数据类型名称为空");
            }

            EmitCreator emit = new EmitCreator();
            emit.CreateType(dv.Type, EmitCreator.DefaultAssemblyName, typeof(AbstractDataItem), GetInterfacesTypes(dv.Contract));

            if (dv.Items != null)
            {
                foreach (var item in dv.Items)
                {
                    var property = emit.CreateProperty(item.Name, GetColumnType(item.Type, item.Format));
                    emit.SetPropertyAttribute(property, typeof(DisplayAttribute), null, null);
                }
            }
            dv.DynamicType = emit.Save();
        }

        /// <summary>
        /// 协议类型转换
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        private Type[] GetInterfacesTypes(string contract)
        {
            if (string.IsNullOrWhiteSpace(contract))
            {
                return null;
            }
            List<Type> lst = new List<Type>();
            foreach (var c in contract.Split(','))
            {
                Type t;
                switch (c.ToLower().Trim())
                {
                    case "conversion":
                        t = typeof (IConversion);
                        break;
                    //case "datastate":
                    //    t = typeof(IDataState);
                    //    break;
                    case "file":
                        t = typeof(IFile);
                        break;
                    case "mail":
                        t = typeof(IMail);
                        break;
                    case "map":
                        t = typeof(IMap);
                        break;
                    case "thumbnail":
                        t = typeof(IThumbnail);
                        break;
                    default:
                        t = null;
                        break;
                }
                if (t != null && !lst.Contains(t))
                {
                    lst.Add(t);
                }
            }
            return lst.ToArray();
        }

        /// <summary>
        /// 获取列类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private Type GetColumnType(EnumColumnType type, string format)
        {
            switch (type)
            {
                case EnumColumnType.DateTime:
                    return typeof (DateTime);
                case EnumColumnType.Double:
                    return typeof (double);
                case EnumColumnType.Enum:
                    return type.GetType().Assembly.GetType(string.Format("XLY.SF.Project.Domains.{0}", format));
                case EnumColumnType.Int:
                    return typeof (int);
                case EnumColumnType.List:
                    return typeof(List<string>);
                default:
                    return typeof (string);

            }
        }

        private Type GetPluginTypeByConfigFile(string configFile)
        {
            XDocument doc = XDocument.Load(configFile);
            string pluginType = doc.Element("plugin")?.Attribute("type")?.Value;
            if (string.IsNullOrWhiteSpace(pluginType))
            {
                throw new Exception("配置文件格式错误，未定义插件类型PluginType");
            }
            PluginType pt = (PluginType)Enum.Parse(typeof(PluginType), pluginType, true);
            switch (pt)
            {
                case PluginType.SpfDataParse:
                    return typeof(DataParsePluginInfo);
                case PluginType.SpfDataView:
                    return typeof(DataViewPluginInfo);
                case PluginType.SpfDataPreview:
                    return typeof(DataPreviewPluginInfo);
                case PluginType.SpfReport:
                    return typeof(DataReportPluginInfo);
                case PluginType.SpfReportModule:
                    return typeof(DataReportModulePluginInfo);
                default:
                    return null;
            }
        }

        private IPlugin GetPlugin(AbstractPluginInfo pluginInfo)
        {
            var plugin = IocManagerSingle.Instance.GetMetaParts<IPlugin, IMetaPluginType>(PluginExportKeys.PluginScriptKey);
            foreach (var loader in plugin)
            {
                if(pluginInfo.PluginType == loader.Metadata.PluginType)
                {
                    return loader.Value;
                }
            }
            throw new Exception("未匹配到合适的插件！");
        }
    }
}
