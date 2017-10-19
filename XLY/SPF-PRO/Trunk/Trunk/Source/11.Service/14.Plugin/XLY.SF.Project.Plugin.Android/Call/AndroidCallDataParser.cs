using System;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android.Call
{
    [Export(PluginExportKeys.PluginKey, typeof(IPlugin))]
    public class AndroidCallDataParser:AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidCallDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Name = "通话记录";
            pluginInfo.Group = "基本信息";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;
            
            pluginInfo.AppName = "com.android.providers.contacts";
            pluginInfo.Icon = "\\icons\\call.png";
            pluginInfo.Description = "提取安卓设备通话记录信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.contacts/databases/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.motorola.blur.providers.contacts/databases/#F");
            this.PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncProgress progress)
        {
            return null;
        }

    }
}
