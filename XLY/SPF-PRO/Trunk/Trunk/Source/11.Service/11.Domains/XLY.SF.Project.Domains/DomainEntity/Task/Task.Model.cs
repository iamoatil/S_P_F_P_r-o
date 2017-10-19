using System;
using System.Collections.Generic;
using System.IO;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// the task domain model
    /// </summary>
    public partial class SPFTask
    {
        #region 静态变量

        /// <summary>
        /// 任务扩展名
        /// </summary>
        [NonSerialized]
        public const string EXT_TASK = "xat";

        /// <summary>
        /// 证据包文件夹
        /// </summary>
        [NonSerialized]
        public const string PATH_PACKAGE = "package";

        /// <summary>
        /// 报表文件夹
        /// </summary>
        [NonSerialized]
        public const string PATH_REPORT = "report";

        /// <summary>
        /// 证据包文件扩展名
        /// </summary>
        [NonSerialized]
        public const string EXT_PACKAGE = "pkg";

        /// <summary>
        /// 镜像文件夹
        /// </summary>
        [NonSerialized]
        public const string PATH_MIRROR = "mirror";

        /// <summary>
        /// Backup文件夹
        /// </summary>
        [NonSerialized]
        public const string PATH_BACKUP = "backup";

        /// <summary>
        /// 源数据文件夹
        /// </summary>
        [NonSerialized]
        public const string PATH_SOURCE = "source";

        /// <summary>
        /// MTP源数据文件夹
        /// </summary>
        [NonSerialized]
        public const string PATH_MTP = "mtp";

        [NonSerialized]
        public const string PATH_BCP = "bcp";


        /// <summary>
        /// 校验码文件扩展名
        /// </summary>
        [NonSerialized]
        public const string EXT_VERIFYCODE_FILE = "md5";

        /// <summary>
        /// 设备信息文件扩展名
        /// </summary>
        [NonSerialized]
        public const string EXT_DEVICE = "device";

        /// <summary>
        /// 日志文件名称
        /// </summary>
        [NonSerialized]
        public const string PATH_LOGFILE = "task.log";

        /// <summary>
        /// 数据区镜像文件名称格式，名称+序列号
        /// </summary>
        public const string DEVICE_MIRROR_FILE = "{0}_{1}_{2}_{3}." + EXT_MIRROR;

        /// <summary>
        /// 镜像文件扩展名
        /// </summary>
        public const string EXT_MIRROR = "bin";

        /// <summary>
        /// 暴恐文件夹
        /// </summary>
        [NonSerialized]
        public const string VIOLENCE = "Violence";

        #endregion

        #region 任务基本属性

        /// <summary>
        /// 任务零时编号，主要用于界面呈现、排列
        /// </summary>
        public string KeyId { get; set; }

        /// <summary>
        /// 唯一 标示
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        private EnumTaskStatus _status;
        public EnumTaskStatus Status { 
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        /// <summary>
        /// 案件名称
        /// </summary>
        public string CaseName { get; set; }

        /// <summary>
        /// 送检人
        /// </summary>
        public string CensorshipPeople { get; set; }

        /// <summary>
        /// 送检时间
        /// </summary>
        public DateTime CensorshipTime { get; set; }

        /// <summary>
        /// 正面照片
        /// </summary>
        public string MobileFrontPhoto { get; set; }

        /// <summary>
        /// 正面照片完整路径
        /// </summary>
        public string MobileFrontPhotoFullPath
        {
            get
            {
                return this.MobileFrontPhoto == null ? string.Empty : System.IO.Path.Combine(this.TaskPath, this.MobileFrontPhoto);
            }
        }

        /// <summary>
        /// 反面照片
        /// </summary>
        public string MobileNegativePhoto { get; set; }

        /// <summary>
        /// 反面照片完整路径
        /// </summary>
        public string MobileNegativePhotoFullPath
        {
            get
            {
                return this.MobileNegativePhoto == null ? string.Empty : System.IO.Path.Combine(this.TaskPath, this.MobileNegativePhoto);
            }
        }

        /// <summary>
        /// 设备
        /// </summary>
        public Device Device { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// 最后编辑时间
        /// </summary>
        public DateTime LastDate { get; set; }

        /// <summary>
        /// 任务日志列表
        /// </summary>
        [field: NonSerializedAttribute()]
        public List<TaskLog> Logs;

        #endregion

        #region 任务配置属性

        public bool IsSaved { get; set; }

        /// <summary>
        /// 自动保存数据
        /// </summary>
        public bool AutoSave { get; set; }

        /// <summary>
        /// 自动保存的数据项
        /// </summary>
        public EnumDataItem AutoSaveItems { get; set; }

        /// <summary>
        /// 任务工作空间路径 ： mirror， source， package，report，log
        /// </summary>
        public string WorkSpace { get; set; }

        #endregion

        #region 任务运行时配置

        /// <summary>
        /// 其他信息存储。
        /// </summary>
        [NonSerialized]
        public object Tag;

        /// <summary>
        /// 提取后的验证码集合
        /// </summary>
        public List<FileVerifyCode> VerifyCodes { get; set; } = new List<FileVerifyCode>();

        /// <summary>
        /// 源数据文件本地映射字典
        /// </summary>
        [NonSerialized]
        public SourceFileItems SourceFiles;

        /// <summary>
        /// 任务工作路径
        /// </summary>
        public string TaskPath
        {
            get
            {
                if (String.IsNullOrEmpty(this._TaskPath))
                    this._TaskPath = this.WorkSpace+this.Name;
                return this._TaskPath;
            }
            set { this._TaskPath = value; }
        }
        private string _TaskPath;

        /// <summary>
        /// 任务文件路径
        /// </summary>
        public string TaskFile
        {
            get
            {
                //if (this._TaskFile.IsInvalid())
                this._TaskFile = this.TaskPath+string.Format("{0}.{1}", Name, SPFTask.EXT_TASK);
                return this._TaskFile;
            }
            set { this._TaskFile = value; }
        }
        private string _TaskFile;

        /// <summary>
        /// 任务数据源路径
        /// </summary>
        public string TaskSourcePath
        {
            get { return FileHelper.ConnectPath(this.TaskPath, PATH_SOURCE); }
        }

        /// <summary>
        /// 任务MTP数据源路径
        /// </summary>
        public string TaskMTPSourcePath
        {
            get { return FileHelper.ConnectPath(this.TaskPath, PATH_MTP); }
        }

        /// <summary>
        /// 任务证据包路径
        /// </summary>
        public string TaskPackagePath
        {
            get { return FileHelper.ConnectPath(this.TaskPath, SPFTask.PATH_PACKAGE); }
        }

        /// <summary>
        /// 报表导出路径
        /// </summary>
        public string TaskReportPath
        {
            get { return FileHelper.ConnectPath(this.TaskPath, SPFTask.PATH_REPORT); }
        }

        /// <summary>
        /// 任务mirror路径
        /// </summary>
        public string TaskMirrorPath
        {
            get { return FileHelper.ConnectPath(this.TaskPath, SPFTask.PATH_MIRROR); }
        }

        /// <summary>
        /// 任务Backup路径
        /// </summary>
        public string TaskBackupPath
        {
            get { return FileHelper.ConnectPath(this.TaskPath, PATH_BACKUP); }
        }

        /// <summary>
        /// 任务Bcp文件导出路径
        /// </summary>
        public string TaskBcpPath
        {
            get { return FileHelper.ConnectPath(this.TaskPath, PATH_BCP); }
        }

        /// <summary>
        /// 任务镜像文件存储路径
        /// </summary>
        public string TaskMirrorFilePath { get; set; }

        /// <summary>
        /// 任务日志文件
        /// </summary>
        public string TaskLogPath
        {
            get { return FileHelper.ConnectPath(this.TaskPath, SPFTask.PATH_LOGFILE); }
        }

        /// <summary>
        /// 任务证据包文件路径格式
        /// </summary>
        public string TaskPackageFileFromat
        {
            get { return FileHelper.ConnectPath(this.TaskPackagePath, string.Format("{0}_{1}.{2}", "{0}", DateTime.Now.ToString("yyyyMMddHHmmssff"), SPFTask.EXT_PACKAGE)); }
        }

        /// <summary>
        /// 源文件校验码文件名
        /// </summary>
        public string SourceVerifyCodeFile
        {
            get { return string.Format("{0}.{1}", SPFTask.PATH_SOURCE, SPFTask.EXT_VERIFYCODE_FILE); }
        }

        /// <summary>
        /// 本地提取数据源路径
        /// </summary>
        public string LocalDataSourcePath { get; set; }

        /// <summary>
        /// 本地暴恐文件下载保存路径
        /// </summary>
        public string ViolenceDataSourcePath
        {
            get { return Path.Combine(this.TaskSourcePath, SPFTask.VIOLENCE); }
        }

        #endregion
    }
}
