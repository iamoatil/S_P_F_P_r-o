using System;
using System.IO;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 案例。
    /// </summary>
    public sealed class Case
    {
        #region Fields

        private readonly CPConfiguration _configuration;

        private readonly FileSystemWatcher _watcher;

        private readonly RestrictedCaseInfo _caseInfo;

        #endregion

        #region Constructors

        private Case(RestrictedCaseInfo caseInfo, CPConfiguration configuration)
        {
            _caseInfo = caseInfo;
            _configuration = configuration;
            _watcher = CreateWatcher(configuration.Directory);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 案例信息。
        /// </summary>
        public RestrictedCaseInfo CaseInfo
        {
            get
            {
                ThrowExceptionIfNotExisted();
                return _caseInfo;
            }
        }

        /// <summary>
        /// 案例项目信息。
        /// </summary>
        public CPConfiguration Configuration
        {
            get
            {
                ThrowExceptionIfNotExisted();
                return _configuration;
            }
        }

        /// <summary>
        /// 案例名称。
        /// </summary>
        public String Name
        {
            get
            {
                ThrowExceptionIfNotExisted();
                return CaseInfo.Name;
            }
            set
            {
                ThrowExceptionIfNotExisted();
                if (CaseInfo.Name != value)
                {
                    if (_configuration.UpdateName(value))
                    {
                        CaseInfo._caseInfo.Name = value;
                        CaseInfo.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// 案例是否存在。
        /// </summary>
        public Boolean Existed { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 创建新的案例。
        /// </summary>
        /// <param name="caseInfo">案例信息。</param>
        /// <returns>新的案例。</returns>
        public static Case New(CaseInfo caseInfo)
        {
            if(caseInfo == null) throw new ArgumentNullException("caseInfo");
            CPConfiguration configuration = CPConfiguration.Create(caseInfo);
            if (configuration == null) return null;
            if (!configuration.Save()) return null;
            RestrictedCaseInfo rci = configuration.GetCaseInfo();
            return new Case(rci, configuration) { Existed = true };
        }

        /// <summary>
        /// 打开案例。
        /// </summary>
        /// <param name="path">案列所在路径。</param>
        /// <returns>案例。</returns>
        public static Case Open(String path)
        {
            CPConfiguration configuration = CPConfiguration.Open(path);
            if (configuration == null) return null;
            RestrictedCaseInfo caseInfo = configuration.GetCaseInfo();
            return new Case(caseInfo, configuration) { Existed = true };
        }

        /// <summary>
        /// 删除指定目录路径下的案例，同时删除所在目录。
        /// </summary>
        /// <param name="path">案例所在目录路径。</param>
        public void Delete()
        {
            Delete(false);
        }

        #endregion

        #region Private

        private void Delete(Boolean isEvent)
        {
            if (!Existed) return;
            _watcher?.Dispose();
            if (!isEvent)
            {
                Directory.Delete(_configuration.Directory, true);
            }
            Existed = false;
        }

        private FileSystemWatcher CreateWatcher(String directory)
        {
            String parentDirectory = Path.GetDirectoryName(directory);
            if (!Directory.Exists(parentDirectory)) return null;
            FileSystemWatcher watcher = new FileSystemWatcher(parentDirectory);
            watcher.BeginInit();
            watcher.Deleted += Watcher_Deleted;
            watcher.NotifyFilter = NotifyFilters.DirectoryName;
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;
            watcher.EndInit();
            return watcher;
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == _configuration.Directory)
            {
                Delete(true);
            }
        }

        private void ThrowExceptionIfNotExisted()
        {
            if (Existed) return;
            throw new InvalidOperationException("This case has been deleted.");
        }

        #endregion

        #endregion
    }
}
