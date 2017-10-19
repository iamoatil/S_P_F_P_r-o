using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter;
using System.Runtime.Serialization.Formatters.Binary;
using XLY.SF.Project.Persistable;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using System.Reflection;
using DllClient;
using XLY.SF.Framework.Language;
using System.Windows;

namespace XLY.SF.Project.TestApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //IocManagerSingle.Instance.LoadParts();

            //var pluginAdapter = (PluginAdapter)IocManagerSingle.Instance.GetPart<IPluginAdapter>(ExportKeys.PluginAdapterKey);
            //pluginAdapter.Initialization(null);
            //var plugins = pluginAdapter.Plugins;
            //var pluginList = plugins.Select(p => p.Key).ToList();

            //List<PluginMatchFilter> filters = new List<PluginMatchFilter>();
            //PluginMatchFilter filter = new PluginMatchFilter();
            //filter.AppName = "com.android.chrome";
            //filter.AppVersion = new Version("3.0");
            //filter.EnumOSType = EnumOSType.Android;
            //filter.PumpType = EnumPump.USB;
            //filters.Add(filter);

            //var list = pluginAdapter.SmartMatchPlugin(pluginList, filters);

            //var a = X86DLLClientSingle.Instance.LoginWcf();
            //a = DeviceManagementSingle.Instance.OpenUsbService();
            //LanguageHelperSingle.Instance.SwitchLanguage();
            //var a = LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.DeviceLanguage_DeviceUseableValidation_InUse);
            HuJingDataFilterTest test = new HuJingDataFilterTest();
            test.TestAggregationFilter();

            //Fhjun_Test test = new Fhjun_Test();
            //test.TestHtmlReport();

            Console.WriteLine("OVER");
            Console.ReadLine();
        }

        //public static List<PluginMatchFilter> MatchPlugin(ExtractItemCollection items,Task task,Pump pump)
        //{
        //    List<PluginMatchFilter> filters = new List<PluginMatchFilter>();
        //    var selectedItems = items.Where(p => p.Checked == true);
        //    foreach(var item in selectedItems)
        //    {
        //        PluginMatchFilter filter = new PluginMatchFilter();
        //        filter.AppName = item.Name;
        //    }
        //    return filters;
        //}
    }
}
