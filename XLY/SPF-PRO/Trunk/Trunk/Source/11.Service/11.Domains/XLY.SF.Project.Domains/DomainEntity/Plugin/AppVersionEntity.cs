using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 手机应用版本号实体
    /// </summary>
    public class AppVersionEntity
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public Version Version { get; set; }
    }

    /// <summary>
    /// 版本号 配置实体
    /// </summary>
    public class AppVersionConfigEntity
    {
        #region 属性

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 版本号存储字段
        /// </summary>
        public string VersionNode { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public AppversionFileType FileType { get; set; }

        /// <summary>
        /// 字符串
        /// </summary>
        public string Sqltring { get; set; }


        #endregion

        #region public 是否有效

        /// <summary>
        /// 是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !String.IsNullOrEmpty(this.AppName) & !String.IsNullOrEmpty(this.FilePath) & !String.IsNullOrEmpty(this.FileName);
        }

        #endregion
    }

    /// <summary>
    /// 文件类型
    /// </summary>
    public enum AppversionFileType
    {
        xml,
        db,
        dat
    }

    public interface IAppVersionGet
    {
        bool AppNameMatch(string _appName);
        Version GetVersion();
    }
}
