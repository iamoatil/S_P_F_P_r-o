namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    /// <summary>
    /// shell指令集
    /// </summary>
    internal class ConstInstructionSet
    {
        ///// <summary>
        ///// su超级权限指令
        ///// </summary>
        //public const string SU = "su -c \"{0}\" ";

        ///// <summary>
        ///// 查看系统目录指令
        ///// </summary>
        //public const string LS = "ls -l {0}";

        /// <summary>
        /// 读取文件指令
        /// </summary>
        public const string CAT = "cat {0}";

        /// <summary>
        /// 读取文件到sdcard临时存储
        /// </summary>
        public const string CAT_TO_SDCARD = "cat {0} > {1}";

        /// <summary>
        /// sdcard的download目录
        /// </summary>
        public const string SDCARD_DOWNLOAD = "/mnt/sdcard/Download/";

        /// <summary>
        /// 删除文件
        /// </summary>
        public const string DELETE_FILE = "rm {0}";

        /// <summary>
        /// 文件修改（提权）指令 
        /// -R,递归设置所有子文件子文件夹
        /// 坑：-R参数在某些（三星）手机上无法使用
        /// </summary>
        public const string CHMODR = "chmod -R 777 {0}";

        public const string CHMOD = "chmod 777 {0}";

        /// <summary>
        /// 获取安装包信息指令
        /// </summary>
        public const string PACKAGE = "dumpsys package";

        /// <summary>
        /// The getprop command
        /// </summary>
        public const string GETPROP = "getprop";

        /// <summary>
        /// mount分区信息指令
        /// </summary>
        public const string MOUNT = "cat /proc/mounts";

        /// <summary>
        /// linux分区信息指令
        /// </summary>
        public const string PARTITON = "cat /proc/partitions";

        /// <summary>
        /// 验证设备是否root的指令
        /// </summary>
        public const string CAN_SU = "ls -l /data/data/com.android.providers.telephony/";
        public const string CAN_SU2 = "ls -l /data/data/com.android.providers.contacts/";

        /// <summary>
        /// rename a file
        /// </summary>
        public const string RENAM_EFILE = "mv {0} {1}";

        public const string CREATE_DIR = "mkdir -p {0}";
        /// <summary>
        /// screen lock files
        /// </summary>
        public static readonly string[] SCREEN_LOCK_FILES = new string[] { "/data/system/gesture.key", "/data/system/password.key", "/data/system/sparepassword.key" };
        /// <summary>
        /// 清除屏幕锁，修改文件名添加的后缀字符
        /// </summary>
        public const string SCREEN_LOCK_EXT = "xlysk_k";

        /// <summary>
        /// diff命令用于两个文件之间的比较，并指出两者的不同，它的使用权限是所有用户
        /// diff [options] 源文件 目标文件 
        /// </summary>
        public const string DIFF = "diff ";

        /// <summary>
        /// df命令用来检查文件系统的磁盘空间占用情况，使用权限是所有用户。
        /// </summary>
        public const string DF = "df ";

        /// <summary>
        /// 获取文件的MD5值，比如：md5 /system/app/SDKLongRrs.apk
        /// </summary>
        public const string MD5_FILE = "md5 {0}";

        /// <summary>
        /// 获取文件夹的MD5值，比如：md5 /system/app/*
        /// </summary>
        public const string MD5_FOLDER = "md5 {0}/*";

        #region 截屏

        /// <summary>
        /// 截屏指令。StringFormat格式
        /// </summary>
        public const string ScreenShotFormat = "/system/bin/screencap -p /sdcard/{0}.png";

        #endregion
    }
}
