using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.Model;

namespace ProjectExtend.Context
{
    public partial class SystemContext
    {
        #region 初始化信息

        /// <summary>
        /// 初始化
        /// </summary>
        public bool InitSysInfo()
        {
            SysStartDateTime = DateTime.Now;
            //加载系统DPI
            LoadCurrentScreenDPI();
            //全局存储路径
            SysSaveFullPath = CreatePath(SysSaveFullPath);
            //如果全局路径创建失败则不再尝试其他路径创建直接返回，退程序
            if (!string.IsNullOrWhiteSpace(SysSaveFullPath) && Path.IsPathRooted(SysSaveFullPath))
            {
                //案例存储路径
                CaseSaveFullPath = CreatePath(CaseSaveFullPath);
                //操作日志截图文件夹
                OperationImageFolderName = _configService.GetSysConfigValueByKey("OperationImageFolderName");
                CurOperationImageFolder = CreatePath(Path.Combine(SysSaveFullPath, OperationImageFolderName, SysStartDateTime.Date.ToString("yyyyMMdd")));
            }
            return !string.IsNullOrWhiteSpace(SysSaveFullPath) &&
                !string.IsNullOrWhiteSpace(CaseSaveFullPath) &&
                !string.IsNullOrWhiteSpace(OperationImageFolderName) &&
                !string.IsNullOrWhiteSpace(CurOperationImageFolder) &&
                LoadConfig();
        }

        #endregion

        #region 当前用户

        /// <summary>
        /// 设置成功登录的用户
        /// </summary>
        /// <param name="user"></param>
        public void SetLoginSuccessUser(UserInfoEntityModel user)
        {
            if (user != null)
            {
                _curUserInfoClone = user.ToReadOnly();
                CurUserInfo = user.ToReadOnly();
                //设置监听事件，防止外部修改
                if (_curUserInfoPropChanged != null)
                    _curUserInfoPropChanged.PropertyChanged -= _curUserInfoPropChanged_PropertyChanged;
                _curUserInfoPropChanged = CurUserInfo as INotifyPropertyChanged;
                _curUserInfoPropChanged.PropertyChanged += _curUserInfoPropChanged_PropertyChanged;
            }
        }

        private void _curUserInfoPropChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CurUserInfo = _curUserInfoClone.ToReadOnly();
        }

        #endregion

        #region 添加操作日志

        /// <summary>
        /// 添加操作日志到数据库
        /// </summary>
        /// <param name="opEmt">操作内容</param>
        public void AddOperationLog(OperationLogParamElement opEmt)
        {
            OperationLogEntityModel log = new OperationLogEntityModel()
            {
                OperationContent = opEmt.OperationContent,
                OperationDateTime = DateTime.Now,
                ScreenShotPath = opEmt.ScreenShotPath,
                OperationUser = this.CurUserInfo
            };
            _dbService.Add(log);
        }

        /// <summary>
        /// 保存操作图片，整个窗体
        /// </summary>
        /// <param name="control">截图目标</param>
        /// <returns>截图成功后的绝对路径</returns>
        public string SaveOperationImageByWindow(FrameworkElement control)
        {
            if (control != null)
            {
                var curWin = Window.GetWindow(control);
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)curWin.Width, (int)curWin.Height, this.DpiX, this.DpiY, PixelFormats.Pbgra32);
                string imageFullPath = Path.Combine(CurOperationImageFolder, GetOperationImageSaveName());
                using (FileStream fs = new FileStream(imageFullPath, FileMode.Create))
                {
                    rtb.Render(curWin);
                    PngBitmapEncoder pbe = new PngBitmapEncoder();
                    var bitTmp = BitmapFrame.Create(rtb);
                    pbe.Frames.Add(bitTmp);
                    pbe.Save(fs);
                    fs.Flush();
                }
                return imageFullPath;
            }
            return string.Empty;
        }

        #endregion

        #region 推荐方案

        /// <summary>
        /// 加载所有推荐方案
        /// </summary>
        /// <param name="solutionContentFromXml">推荐方案内容</param>
        public bool LoadProposedSolution(string solutionContentFromXml)
        {
            List<StrategyElement> result = new List<StrategyElement>();
            try
            {
                if (!string.IsNullOrWhiteSpace(solutionContentFromXml))
                {
                    var xml = new System.Xml.Serialization.XmlSerializer(typeof(List<StrategyElement>));
                    using (TextReader tr = new StringReader(solutionContentFromXml))
                    {
                        SolutionProposed = xml.Deserialize(tr) as List<StrategyElement>;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "加载推荐方案失败");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取所有推荐方案
        /// </summary>
        /// <returns></returns>
        public StrategyElement[] GetAllProposedSolution()
        {
            return SolutionProposed.ToArray();
        }

        #endregion

        #region 设置路径

        /// <summary>
        /// 设置当前系统存储路径
        /// </summary>
        /// <param name="sysSaveFullPath"></param>
        /// <param name="inConfig">是否写入配置文件</param>
        public void SetSysSaveFullPath(string sysSaveFullPath, bool inConfig = false)
        {
            if (string.IsNullOrWhiteSpace(sysSaveFullPath) && Directory.Exists(sysSaveFullPath))
            {
                SysSaveFullPath = sysSaveFullPath;
                if (inConfig)
                    _configService.SetSysConfigValueBykey("SysSaveFullPath", SysSaveFullPath);
            }
        }

        /// <summary>
        /// 设置当前案例存储路径
        /// </summary>
        /// <param name="caseSaveFullPath"></param>
        /// <param name="inConfig">是否写入配置文件</param>
        public void SetCaseSaveFullPath(string caseSaveFullPath, bool inConfig = false)
        {
            if (string.IsNullOrWhiteSpace(caseSaveFullPath) && Directory.Exists(caseSaveFullPath))
            {
                CaseSaveFullPath = caseSaveFullPath;
                if (inConfig)
                    _configService.SetSysConfigValueBykey("CaseSaveFullPath", CaseSaveFullPath);
            }
        }

        #endregion
    }
}
