using System;

namespace XLY.XDD.ForensicService
{
    /// <summary>
    /// 文件信息接口
    /// </summary>
    public interface IFileX
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 完整路径
        /// </summary>
        string FullPath { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        long Size { get; set; }

        /// <summary>
        /// 扩展名称
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// 创建日期
        /// </summary>
        DateTime CreationDate { get; set; }

        /// <summary>
        /// 最后访问日期
        /// </summary>
        DateTime LastAccessDate { get; set; }

        /// <summary>
        /// 最后编辑日期
        /// </summary>
        DateTime LastWriteDate { get; set; }
    }
}