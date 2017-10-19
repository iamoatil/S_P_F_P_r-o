using System;
using System.Text;

namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    public class ConstCodeHelper
    {
        /// <summary>
        /// 截屏保存到的文件夹名
        /// </summary>
        public const string ScreenShotDirName = "source\\ScreenShotSrc";
        /// <summary>
        /// 截屏图片对应的说明文件名
        /// </summary>
        public const string ScreenShotDetailFileName = "ScreenShotSrcDetail";

        /// <summary>
        /// The default encoding string
        /// </summary>
        public const string DEFAULT_ENCODING_STR = "ISO-8859-1";

        /// <summary>
        /// 默认字符编码格式
        /// The default encoding
        /// </summary>
        public static readonly Encoding ENGLISH_ENCODING = Encoding.GetEncoding(DEFAULT_ENCODING_STR);
        //public static readonly Encoding ENGLISH_ENCODING = Encoding.UTF8;
        public static readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;
        public static readonly Encoding GBK_ENCODING = Encoding.GetEncoding("GBK");


        /// <summary>
        /// SDCard路径字典
        /// </summary>
        public static readonly string[] SDCardPathDictionary = new[] { "/mnt/sdcard/", "/storage/sdcard0/", "/mnt/shell/emulated/0/" };

        /// <summary>
        /// 文件推送到手机的默认路径
        /// </summary>
        public const string PUSHFILETEMPPATH = "/data/local/tmp/";

        /// <summary>
        /// 文件路径中的空格替换符
        /// </summary>
        public const string PATH_WHITESPACE_REPLACE = "\\ ";

        /// <summary>
        /// 分区：sdcard区
        /// </summary>
        public const string PARTITION_All = "all storage";

        /// <summary>
        /// adb默认端口号
        /// The default ADB bridge port 
        /// </summary>
        public const int ADB_PORT = 5037;
        /// <summary>
        /// 监控时间间隔
        /// </summary>
        public const int MONITOR_INTERVAL = 500;
        /// <summary>
        /// 监控重试次数
        /// </summary>
        public const int MONITOR_ATTEMPTCOUNT = 12;

        /// <summary>
        /// adb进程名称
        /// </summary>
        public const string ADB = "adb";

        /// <summary>
        /// Device list info regex
        /// </summary>
        public const String RE_DEVICELIST_INFO = @"^([a-z0-9-_&#@\?]+(?:\s[a-z0-9\?]+)?)\s+(device|offline|unknown|bootloader|recovery|download)$";

        public const String LS_PATTERN_EX = @"^([bcdlsp-][-r][-w][-xsS][-r][-w][-xsS][-r][-w][-xstST])\s+(?:\d{0,})?\s*(\S+)\s+(\S+)\s+(?:\d{1,},\s+)?(\d{1,}|\s)\s+(\w{3}|\d{4})[\s-](?:\s?(\d{1,2})\s?)[\s-]\s?(?:(\d{2}|\d{4}|\s)\s*)?(\d{2}:\d{2}|\s)\s*(.*?)([/@=*\|]?)$";

        /** Default timeout values for adb connection (milliseconds) */
        public const int DEFAULT_TIMEOUT = 9000;

        /// <summary>
        /// ADB命令执行超时，单位毫秒
        /// 注意：检测设备是否root方法专用(ADB.CanSU)
        /// by luochao 20161101
        /// </summary>
        public const int CAN_SU_DEFAULT_TIMEOUT = 5000;
        /// <summary>
        /// 设备监控socket超时时长
        /// </summary>
        public const int MONITOR_TIMEOUT = 3000;

        /// <summary>
        /// 默认指令发生buffer大小
        /// </summary>
        public const int DEFAULT_SEND_BUFFER_SIZE = 1024;
        /// <summary>
        /// 默认命令buffer大小
        /// </summary>
        public const int DEFAULT_COMMAND_BUFFER_SIZE = 16 * 1024;
        /// <summary>
        /// 文件拷贝buffer大小
        /// </summary>
        public const int PULLFILE_DATA_BUFFER_SIZE = 64 * 1024;

        /// <summary>
        /// 镜像buffer大小
        /// </summary>
        public const int MIRROR_DATA_BUFFER_SIZE = 64 * 1024;

        /// <summary>
        /// 数据接收标示
        /// </summary>
        public const String RECV = "RECV";
        /// <summary>
        /// 数据标示
        /// </summary>
        public const String DATA = "DATA";
        /// <summary>
        /// 数据接收完成标示
        /// </summary>
        public const String DONE = "DONE";

        public const String SEND = "SEND";
        public const String OKAY = "OKAY";
        public const String FAIL = "FAIL";

        /// <summary>
        /// 分区：数据区
        /// </summary>
        public const string PARTITION_DATA = "/data";
        /// <summary>
        /// 分区：系统区
        /// </summary>
        public const string PARTITION_SYSTEM = "/system";
        /// <summary>
        /// 分区：缓存区
        /// </summary>
        public const string PARTITION_CACHE = "/cache";
        /// <summary>
        /// 分区：sdcard区
        /// </summary>
        public const string PARTITION_SDCARD = "sdcard";

        public delegate void ReceiveDataEvnentHandler(byte[] data, int offset = 0, int length = -1);

        /// <summary>
        /// 数据区
        /// </summary>
        public const string DATA_BLOCK = "/data";

        private static readonly char[] InvalidPathChars = new char[] {
                        '"', '<', '>', '|', '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v',
                        '\f', '\r', '\x000e', '\x000f', '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b',
                        '\x001c', '\x001d', '\x001e', '\x001f','*', '?', ':','\\',
                 };

        /// <summary>
        /// 过滤linux文件路径中的非法字符。
        /// </summary>
        public static string FilterLinuxFileName(string filename)
        {
            var index = filename.IndexOfAny(InvalidPathChars);
            if (index >= 0)
            {
                return FilterLinuxFileName(filename.Replace(filename[index], '_'));
            }
            return filename;
        }
    }
}
