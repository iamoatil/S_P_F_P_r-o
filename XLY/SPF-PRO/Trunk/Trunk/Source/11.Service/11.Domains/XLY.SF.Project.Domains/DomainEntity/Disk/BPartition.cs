using System;
using System.Collections.Generic;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 分区信息
    /// </summary>
    public class BPartition:Disk
    {
        public BPartition()
        {
            this.DevType = 0x11;
            this.PartitionType = 0;
        }
        /// <summary>
        /// 打开句柄
        /// </summary>
        public IntPtr MountHandle { get; set; }

        /// <summary>
        /// 卷名
        /// </summary>
        public string VolName { get; set; }

        /// <summary>
        /// 文件系统
        /// </summary>
        public FileSystemType FileSystem { get; set; }

        /// <summary>
        /// 分区类型(0:正常分区,1:raw分区)
        /// </summary>
        public byte PartitionType { get; set; }
        
        /// <summary>
        /// 设备类型
        /// </summary>
        public byte DevType { get; set; }

        /// <summary>
        /// 安装的应用。
        /// </summary>
        public IEnumerable<AppEntity> InstallApps { get; set; }

        /// <summary>
        /// 界面图片呈现
        /// </summary>
        public string IconImage
        {
            get { return string.Format("{0}/{1}", GetPhysicalPath("icons"), "part.png"); }
        }

        #region 获取文件相对路径映射的物理路径
        /// <summary>
        /// 获取文件相对路径映射的物理路径，若文件为绝对路径则直接返回
        /// </summary>
        /// <param name="relativePath">文件的相对路径</param>        
        public static string GetPhysicalPath(string relativePath)
        {
            //有效性验证
            if (string.IsNullOrEmpty(relativePath))
            {
                return string.Empty;
            }
            //~,~/,/,\
            relativePath = relativePath.Replace("/", @"\").Replace("~", string.Empty).Replace("~/", string.Empty);
            relativePath = relativePath.StartsWith("\\") ? relativePath.Substring(1, relativePath.Length - 1) : relativePath;
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var fullPath = System.IO.Path.Combine(path, relativePath);
            return fullPath;
        }
        #endregion
    }
}
