using System;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 手机应用APP实体
    /// </summary>
    [Serializable]
    public class AppEntity : AbstractDataItem
    {
        /// <summary>
        /// 图标路径(相对路径)
        /// </summary>
        [Display(ColumnType = EnumColumnType.Image, Alignment = EnumAlignment.Center)]
        public string Icon { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 应用类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 应用编号
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 版本信息
        /// </summary>
        public Version Version { get; set; }

        private string _VersionDesc;

        /// <summary>
        /// 版本描述
        /// </summary>
        public string VersionDesc
        {
            get { return _VersionDesc; }
            set
            {
                _VersionDesc = value;
                Version = value.ToSafeVersion();
            }
        }

        /// <summary>
        /// 应用描述
        /// </summary>
        public string Descritpion { get; set; }

        /// <summary>
        /// 安装路径
        /// </summary>
        public string InstallPath { get; set; }

        /// <summary>
        /// 数据存储路径
        /// </summary>
        public string DataPath { get; set; }

        /// <summary>
        /// 安装日期
        /// </summary>
        public DateTime? InstallDate { get; set; }

        public string _InstallDate
        {
            get { return InstallDate.ToDateTimeString(); }
        }

    }
}
