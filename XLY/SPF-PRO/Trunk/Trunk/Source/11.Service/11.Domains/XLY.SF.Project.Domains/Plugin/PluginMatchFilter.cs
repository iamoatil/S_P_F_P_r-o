using System;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    public class PluginMatchFilter
    {
        public String AppName { get; set; }

        public EnumPump PumpType { get; set; }

        public EnumOSType EnumOSType { get; set; }

        public Version AppVersion { get; set; }

        public String Manufacture { get; set; }

        public String Brand { get; set; }
        
    }
}
