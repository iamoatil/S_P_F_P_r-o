using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    public static class CommonStrings
    {
        /// <summary>
        /// The default binaries path for x64 systems - "C:\Program Files (x86)\VideoLAN\VLC\".
        /// </summary>
        public const string LIBVLC_DLLS_PATH_DEFAULT_VALUE_AMD64 = @"C:\VLC\";

        /// <summary>
        /// The default binaries path for x86 systems - "C:\Program Files\VideoLAN\VLC\".
        /// </summary>
        public const string LIBVLC_DLLS_PATH_DEFAULT_VALUE_X86 = @"D:\work\DFProject\Source\21-Build\VLC\";

        /// <summary>
        /// The default plugins path for x64 systems - "C:\Program Files (x86)\VideoLAN\VLC\plugins\".
        /// </summary>
        public const string PLUGINS_PATH_DEFAULT_VALUE_AMD64 = @"C:\VLC\plugins\";

        /// <summary>
        /// The default plugins path for x86 systems - "C:\Program Files\VideoLAN\VLC\plugins\".
        /// </summary>
        public const string PLUGINS_PATH_DEFAULT_VALUE_X86 = @"C:\VLC\plugins\";

        internal const string VLC_DOTNET_PROPERTIES_CATEGORY = "VideoLan DotNet";
    }
}
