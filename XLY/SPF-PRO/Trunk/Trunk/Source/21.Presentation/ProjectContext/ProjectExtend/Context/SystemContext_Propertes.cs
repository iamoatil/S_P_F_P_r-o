using System;
using System.Collections.Generic;
using System.ComponentModel;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.Model;

namespace ProjectExtend.Context
{
    public partial class SystemContext
    {
        #region 系统时间

        private DateTime _sysStartDateTime;
        /// <summary>
        /// 系统启动时间
        /// </summary>
        public DateTime SysStartDateTime
        {
            get
            {
                return _sysStartDateTime;
            }
            private set
            {
                _sysStartDateTime = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 今日检查手机总数

        private int _todayInspectPhoneCount = 200;
        /// <summary>
        /// 今日检查手机总数
        /// </summary>
        public int TodayInspectPhoneCount
        {
            get
            {
                return _todayInspectPhoneCount;
            }

            private set
            {
                _todayInspectPhoneCount = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 存储路径
        
        #region 默认文件夹名

        private string _saveDefaultFolderName;
        /// <summary>
        /// 默认文件夹名
        /// </summary>
        public string SaveDefaultFolderName
        {
            get
            {
                return this._saveDefaultFolderName;
            }

            private set
            {
                this._saveDefaultFolderName = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 当前存储路径以及案例路径
        
        private string _sysSaveFullPath;
        /// <summary>
        /// 当前存储路径
        /// </summary>
        public string SysSaveFullPath
        {
            get
            {
                return _sysSaveFullPath;
            }
            private set
            {
                _sysSaveFullPath = value;
                base.OnPropertyChanged();

            }
        }
        
        private string _caseSaveFullPath;
        /// <summary>
        /// 案例存储路径
        /// </summary>
        public string CaseSaveFullPath
        {
            get
            {
                return _caseSaveFullPath;
            }
            private set
            {
                _caseSaveFullPath = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 操作日志【图片】

        private string _operationImageFolder;
        /// <summary>
        /// 操作日志图片存储文件夹名称
        /// </summary>
        public string OperationImageFolderName
        {
            get
            {
                return this._operationImageFolder;
            }

            private set
            {
                this._operationImageFolder = value;
                base.OnPropertyChanged();
            }
        }

        /// <summary>
        /// 当前保存操作截图的绝对路径
        /// </summary>
        public string CurOperationImageFolder
        {
            get;
            private set;
        }

        #endregion

        #endregion

        #region 当前语言

        /// <summary>
        /// 当前语言
        /// </summary>
        public LanguageType CurLanguage
        {
            get
            {
                var curLanguage = _configService.GetSysConfigValueByKey("Language");
                if (!string.IsNullOrEmpty(curLanguage) && Enum.IsDefined(typeof(LanguageType), curLanguage))
                    return (LanguageType)Enum.Parse(typeof(LanguageType), curLanguage);
                return LanguageType.Cn;
            }
        }

        #endregion

        #region 当前登录用户信息

        /// <summary>
        /// 当前登录用户【此值无法被修改，只能通过设置改变】
        /// </summary>
        public UserInfoEntityModel CurUserInfo { get; private set; }

        #endregion

        #region 当前DPI

        /// <summary>
        /// 当前屏幕的X
        /// </summary>
        public int DpiX { get; private set; }

        /// <summary>
        /// 当前屏幕的Y
        /// </summary>
        public int DpiY { get; private set; }

        #endregion

        #region 案例

        private Case _currentCase;
        /// <summary>
        /// 当前打开的案例。
        /// </summary>
        public Case CurrentCase
        {
            get=> _currentCase;
            set
            {
                _currentCase = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region 可以用于异步操作的对象
        /// <summary>
        /// 可以用于异步操作的对象，主要用于非UI线程中替换dispatcher
        /// </summary>
        public AsyncOperation AsyncOperation { get; private set; }
        /// <summary>
        /// 加载异步操作对象
        /// </summary>
        public void LoadAsyncOperation()
        {
            AsyncOperation = AsyncOperationManager.CreateOperation(this);
        }
        #endregion

        #region private

        #region 用户信息

        /// <summary>
        /// 防止修改登录信息
        /// </summary>
        private INotifyPropertyChanged _curUserInfoPropChanged;

        /// <summary>
        /// 本地副本
        /// </summary>
        private UserInfoEntityModel _curUserInfoClone { get; set; }

        #endregion

        #region 推荐方案【目前先全部缓存在内存】

        /// <summary>
        /// 推荐方案
        /// </summary>
        private List<StrategyElement> SolutionProposed { get; set; }

        #endregion

        #endregion
    }
}
