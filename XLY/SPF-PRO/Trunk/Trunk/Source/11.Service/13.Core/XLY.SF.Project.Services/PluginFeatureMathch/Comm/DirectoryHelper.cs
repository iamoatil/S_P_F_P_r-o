using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.Services
{
    static class DirectoryHelper
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePathName">文件路径</param>
        public static void TryDeleteFile(string filePathName)
        {
            try
            {
                FileHelper.DeleteFile(filePathName);
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(string.Format("删除文件[{0}]失败,错误信息：{1}", filePathName, ex));
            }
        }

        /// <summary>
        /// 文件夹搜索
        /// </summary>
        /// <param name="path">搜索路径</param>
        /// <param name="searchPattern">搜索参数</param>
        /// <returns></returns>
        public static string[] GetDirectories(string path, string searchPattern)
        {
            try
            {
                var arr = searchPattern.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

                return Directory.GetDirectories(path, arr[arr.Length - 1], SearchOption.AllDirectories).Where((d) => arr.All(s => d.EndsWith(searchPattern) || d.Contains(s))).ToArray();
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(string.Format("文件夹搜索失败,错误信息：{0}", ex));
                return null;
            }
        }

        /// <summary>
        /// 文件搜索(支持正则)
        /// </summary>
        /// <param name="path">搜索路径</param>
        /// <param name="filename">搜索文件名，支持正则</param>
        /// <returns></returns>
        public static string[] GetFiles(string path, string filename)
        {
            try
            {
                var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Select((f) => new FileInfo(f));

                if (filename.Contains('\\'))
                {
                    return files.Where((f) => f.Name == filename || Regex.IsMatch(f.Name, filename)).Select((fi) => fi.FullName).ToArray();
                }
                else
                {
                    return files.Where((f) => f.Name == filename).Select((fi) => fi.FullName).ToArray();
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(string.Format("文件夹搜索失败,错误信息：{0}", ex));
                return null;
            }
        }
    }
}
