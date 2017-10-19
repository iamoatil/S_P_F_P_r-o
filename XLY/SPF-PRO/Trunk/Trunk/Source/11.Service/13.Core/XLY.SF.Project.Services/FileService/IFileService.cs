/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/28 10:36:37 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 文件系统服务
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// 文件系统操作需要的刻度值
        /// </summary>
        int ExternalCount { get; }

        /// <summary>
        /// 当前文件的文件系统
        /// </summary>
        FNodeX FileSystem { get; }

        /// <summary>
        /// 获取所有文件列表
        /// </summary>
        IEnumerable<FNodeX> GetAllFile { get; }

        /// <summary>
        /// 文件后缀归类集合
        /// </summary>
        Dictionary<string, List<FNodeX>> GetKeyFNodeX { get; }

        /// <summary>
        /// 获取文件系统(快速扫描)
        /// </summary>
        /// <param name="device">设备</param>
        /// <param name="iAsync">异步消息</param>
        /// <returns></returns>
        FNodeX GetFileSystem(IFileSystemDevice device, IAsyncProgress iAsync);

        /// <summary>
        /// 获取用户数据分区文件列表
        /// 【注：如果没有找到数据分区就返回所有分区文件列表】
        /// </summary>
        List<FNodeX> GetUserPartitionFiles { get; }

        /// <summary>
        /// 文件导出
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <param name="isCover"></param>
        /// <returns></returns>
        void ExportFile(FNodeX file, string path, bool isCover = false);

        /// <summary>
        /// 文件导出(批量)
        /// </summary>
        /// <param name="fNodeXs"></param>
        /// <param name="path"></param>
        /// <param name="isCover"></param>
        void ExportFile(List<FNodeX> fNodeXs, string path, bool isMedia = false, bool isCover = false);

        /// <summary>
        /// APP应用文件导出
        /// </summary>
        /// <param name="matchPath"></param>
        /// <param name="path"></param>
        /// <param name="isCover"></param>
        void ExportAppFile(IEnumerable<string> matchPath, string path, bool isCover = false);

        /// <summary>
        /// APP应用文件导出
        /// </summary>
        /// <param name="matchPath"></param>
        /// <param name="path"></param>
        /// <param name="isCover"></param>
        void ExportAppFile(string matchPath, string path, bool isCover = false);

        /// <summary>
        /// 媒体文件导出
        /// </summary>
        /// <param name="item"></param>
        /// <param name="separateChar"></param>
        void ExportMediaFile(KeyValueItem item, string path, char separateChar);

        /// <summary>
        /// 停止文件系统操作
        /// </summary>
        void Stop();

        /// <summary>
        /// 关闭所有操作
        /// </summary>
        void Close();

    }
}
