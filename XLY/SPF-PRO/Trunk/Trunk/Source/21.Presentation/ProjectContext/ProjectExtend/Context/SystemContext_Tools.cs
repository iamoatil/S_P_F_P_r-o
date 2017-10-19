using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.ViewDomain.Model;

namespace ProjectExtend.Context
{
    public partial class SystemContext
    {
        #region 路径创建

        /// <summary>
        /// 创建路径并保存到配置
        /// </summary>
        /// <param name="path">要验证的全路径</param>
        /// <returns></returns>
        private string CreatePath(string path)
        {
            string resultPath = path;
            string folder = SaveDefaultFolderName;
            string pathRootTmp;
            if (string.IsNullOrWhiteSpace(path))
            {
                //寻找最大空闲磁盘并创建
                while (GetSavePathRoot(out pathRootTmp))
                {
                    try
                    {
                        resultPath = Path.Combine(pathRootTmp, folder);
                        Directory.CreateDirectory(resultPath);
                        break;
                    }
                    catch (Exception ex)
                    {
                        //记录已使用的盘符
                        _usedPathRoot.Add(pathRootTmp);
                        resultPath = string.Empty;
                        LoggerManagerSingle.Instance.Warn(ex, string.Format("创建数据存储目录【{0}】失败", pathRootTmp));
                    }
                }
            }
            else
            {
                if (Path.IsPathRooted(path) && !Directory.Exists(path))
                {
                    folder = path.Remove(0, Path.GetPathRoot(path).Length);
                    //寻找最大空闲磁盘并创建
                    while (GetSavePathRoot(out pathRootTmp))
                    {
                        try
                        {
                            resultPath = Path.Combine(pathRootTmp, folder);
                            Directory.CreateDirectory(resultPath);
                            break;
                        }
                        catch (Exception ex)
                        {
                            //记录已使用的盘符
                            _usedPathRoot.Add(pathRootTmp);
                            resultPath = string.Empty;
                            LoggerManagerSingle.Instance.Warn(ex, string.Format("创建数据存储目录【{0}】失败", pathRootTmp));
                        }
                    }
                }
            }
            return resultPath;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="fullPath">文件夹路径</param>
        /// <returns></returns>
        private bool CreateDirecotry(string fullPath)
        {
            bool result = false;
            if (!string.IsNullOrWhiteSpace(fullPath) && !Directory.Exists(fullPath))
            {
                try
                {
                    Directory.CreateDirectory(fullPath);
                    result = true;
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error(ex, string.Format("创建文件夹【{0}】失败", fullPath));
                }
            }
            return result;
        }

        #endregion

        #region 盘符获取

        /// <summary>
        /// 获取可以存储的根目录
        /// </summary>
        /// <param name="curPathRoot">当前根目录</param>
        /// <param name="pathRoot">返回当前计算机空闲容量最大的磁盘</param>
        /// <returns>是否成功</returns>
        private bool GetSavePathRoot(out string pathRoot)
        {
            pathRoot = string.Empty;
            long freeSizeTmp = 0;
            foreach (var dirTmp in DriveInfo.GetDrives())
            {
                if (dirTmp.DriveType == DriveType.Fixed &&
                    dirTmp.IsReady &&
                    !_usedPathRoot.Contains(dirTmp.Name))
                {
                    if (freeSizeTmp < dirTmp.TotalFreeSpace)
                    {
                        freeSizeTmp = dirTmp.TotalFreeSpace;
                        pathRoot = dirTmp.Name;
                    }
                }
            }
            return !string.IsNullOrEmpty(pathRoot);
        }
        #endregion

        #region 创建

        /// <summary>
        /// 获取操作截图保存的文件名称
        /// </summary>
        /// <returns></returns>
        private string GetOperationImageSaveName()
        {
            return string.Format("Op_{0:yyyyMMdd HHmmss}.png", DateTime.Now);
        }

        #endregion

        #region 系统相关

        /// <summary>
        /// 加载当前DPI
        /// </summary>
        private void LoadCurrentScreenDPI()
        {
            using (ManagementClass mc = new ManagementClass("Win32_DesktopMonitor"))
            {
                using (ManagementObjectCollection moc = mc.GetInstances())
                {
                    int PixelsPerXLogicalInch = 0; // dpi for x
                    int PixelsPerYLogicalInch = 0; // dpi for y

                    foreach (ManagementObject each in moc)
                    {
                        PixelsPerXLogicalInch = int.Parse((each.Properties["PixelsPerXLogicalInch"].Value.ToString()));
                        PixelsPerYLogicalInch = int.Parse((each.Properties["PixelsPerYLogicalInch"].Value.ToString()));
                    }

                    //设置当前DPI
                    this.DpiX = PixelsPerXLogicalInch;
                    this.DpiY = PixelsPerYLogicalInch;
                }
            }
        }

        #endregion

        #region 从配置文件读取对应信息

        /// <summary>
        /// 从配置文件读取对应信息
        /// </summary>
        private bool LoadConfig()
        {
            return true;
        }

        #endregion        
    }
}
