// ***********************************************************************
// Assembly:XLY.SF.Project.Plugin.Adapter.FeatureMathch
// Author:Songbing
// Created:2017-04-12 11:19:10
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.Plugin.Adapter.FeatureMathch
{
    /// <summary>
    /// 文件系统辅助类
    /// </summary>
    internal static class DirectoryHelper
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePathName"></param>
        public static void TryDeleteFile(string filePathName)
        {
            try
            {
                FileHelper.DeleteFile(filePathName);
            }
            catch(Exception ex)
            {
                LoggerManagerSingle.Instance.Warn(ex);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void TryDeleteDirectory(string path)
        {
            try
            {
                FileHelper.DeleteDirectory(path);
            }
            catch(Exception ex)
            {
                LoggerManagerSingle.Instance.Warn(ex);
            }
        }

        /// <summary>
        /// 文件夹搜索
        /// </summary>
        /// <param name="sourcePath">搜索路径</param>
        /// <param name="searchPattern">搜索参数</param>
        /// <returns></returns>
        public static string[] GetDirectories(string sourcePath, string searchPattern)
        {
            try
            {
                var arr = searchPattern.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

                return Directory.GetDirectories(sourcePath, arr[arr.Length - 1], SearchOption.AllDirectories).
                                    Where((d) => arr.All(s => d.Contains(searchPattern) || d.EndsWith(s))).ToArray();
            }
            catch(Exception ex)
            {
                LoggerManagerSingle.Instance.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// 文件夹搜索
        /// </summary>
        /// <param name="sourcePath">搜索路径</param>
        /// <param name="searchPattern">搜索参数</param>
        /// <returns></returns>
        public static bool FindDirectories(string sourcePath, string searchPattern)
        {
            try
            {
                var arr = searchPattern.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

                return Directory.GetDirectories(sourcePath, arr[arr.Length - 1], SearchOption.AllDirectories).
                                    Any((d) => arr.All(s => d.Contains(searchPattern) || d.EndsWith(s)));
            }
            catch(Exception ex)
            {
                LoggerManagerSingle.Instance.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// 文件搜索(支持正则)
        /// </summary>
        /// <param name="sourcePath">搜索路径</param>
        /// <param name="fileName">搜索文件名，支持正则</param>
        /// <returns></returns>
        public static string[] GetFiles(string sourcePath, string fileName)
        {
            throw new NotImplementedException();
        }

    }
}
