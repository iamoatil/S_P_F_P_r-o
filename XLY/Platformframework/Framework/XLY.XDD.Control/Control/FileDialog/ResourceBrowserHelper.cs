using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 资源管理器辅助类
    /// </summary>
    public static class ResourceBrowserHelper
    {
        #region GetIcon -- 获取图标

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        /// <summary>   
        /// 返回系统设置的图标   
        /// </summary>   
        /// <param name="pszPath">文件路径 如果为"" 返回文件夹的</param>   
        /// <param name="dwFileAttributes">0</param>   
        /// <param name="psfi">结构体</param>   
        /// <param name="cbSizeFileInfo">结构体大小</param>   
        /// <param name="uFlags">枚举类型</param>   
        /// <returns>-1失败</returns>   
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref   SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        public enum SHGFI
        {
            SHGFI_ICON = 0x100,
            SHGFI_LARGEICON = 0x0,
            SHGFI_USEFILEATTRIBUTES = 0x10
        }

        /// <summary>
        /// 是否在获取文件图标的过程中出错过
        /// </summary>
        private static bool IsGetFileIconError = false;

        private static Icon S_FileIcon;

        /// <summary>   
        /// 获取文件图标
        /// </summary>   
        /// <param name="p_Path">文件全路径</param>   
        /// <returns>图标</returns>   
        public static Icon GetFileIcon(string p_Path)
        {
            if (IsGetFileIconError)
            {
                return S_FileIcon;
            }
            try
            {
                SHFILEINFO _SHFILEINFO = new SHFILEINFO();
                IntPtr _IconIntPtr = SHGetFileInfo(p_Path, 0, ref _SHFILEINFO, (uint)Marshal.SizeOf(_SHFILEINFO), (uint)(SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON | SHGFI.SHGFI_USEFILEATTRIBUTES));
                if (_IconIntPtr.Equals(IntPtr.Zero)) return null;
                Icon _Icon = System.Drawing.Icon.FromHandle(_SHFILEINFO.hIcon);
                return _Icon;
            }
            catch
            {
                using (Stream stream = typeof(ResourceBrowserHelper).Assembly.GetManifestResourceStream("XLY.XDD.Control.Icons.file.ico"))
                {
                    S_FileIcon = new Icon(stream);
                    IsGetFileIconError = true;
                    return S_FileIcon;
                }
            }
        }

        private static Icon S_DirectoryIcon;

        /// <summary>   
        /// 获取文件夹图标 
        /// </summary>   
        /// <returns>图标</returns>   
        public static Icon GetDirectoryIcon()
        {
            if (S_DirectoryIcon != null)
                return S_DirectoryIcon;
            try
            {
                SHFILEINFO _SHFILEINFO = new SHFILEINFO();
                IntPtr _IconIntPtr = SHGetFileInfo(@"", 0, ref _SHFILEINFO, (uint)Marshal.SizeOf(_SHFILEINFO), (uint)(SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON));
                if (_IconIntPtr.Equals(IntPtr.Zero)) return null;
                Icon _Icon = System.Drawing.Icon.FromHandle(_SHFILEINFO.hIcon);
                S_DirectoryIcon = _Icon;
            }
            catch
            {
                using (Stream stream = typeof(ResourceBrowserHelper).Assembly.GetManifestResourceStream("XLY.XDD.Control.Icons.folder.ico"))
                {
                    S_DirectoryIcon = new Icon(stream);
                }
            }

            return S_DirectoryIcon;
        }

        #endregion

        /// <summary>
        /// 判断某个路径是否是文件路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool IsFile(string path)
        {
            if (System.IO.File.Exists(path))
                return true;
            else if (System.IO.Directory.Exists(path))
                return false;
            else
                throw new System.IO.DirectoryNotFoundException("没有找到路径 " + path);
        }

        /// <summary>
        /// 获取文件夹下的所有文件夹和文件
        /// </summary>
        /// <param name="directoryPath">目录</param>
        /// <param name="filter">筛选条件</param>
        /// <returns></returns>
        public static ObservableCollection<ResourceBrowserItem> GetDirectorysAndFiles(string directoryPath, List<ResourceBrowserFilterItem> filter)
        {
            if (directoryPath.IsNullOrEmptyOrWhiteSpace())
                return null;

            ObservableCollection<ResourceBrowserItem> items = new ObservableCollection<ResourceBrowserItem>();

            DirectoryInfo info = new DirectoryInfo(directoryPath);

            // 获取所有的文件夹

            try
            {
                foreach (DirectoryInfo directory in info.GetDirectories())
                {
                    try
                    {
                        ResourceBrowserItem i = new ResourceBrowserItem(directory.FullName, false);
                        items.Add(i);
                    }
                    catch { }
                }
            }
            catch { }

            // 获取所有文件

            try
            {
                if (filter == null || filter.Count == 0)
                {
                    foreach (FileInfo fi in info.GetFiles())
                    {
                        try
                        {
                            ResourceBrowserItem i = new ResourceBrowserItem(fi.FullName, true);
                            items.Add(i);
                        }
                        catch { }
                    }
                }
                else
                {
                    foreach (ResourceBrowserFilterItem f in filter)
                    {
                        foreach (string str_f in f.Filters)
                        {
                            foreach (FileInfo fi in info.GetFiles(str_f))
                            {
                                try
                                {
                                    ResourceBrowserItem i = new ResourceBrowserItem(fi.FullName, true);
                                    items.Add(i);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { }

            return items;
        }

        /// <summary>
        /// 获取文件夹下的所有文件夹
        /// </summary>
        /// <param name="directoryPath">路径</param>
        /// <returns></returns>
        public static ObservableCollection<ResourceBrowserItem> GetDirectorys(string directoryPath)
        {
            if (directoryPath.IsNullOrEmptyOrWhiteSpace())
                return null;

            ObservableCollection<ResourceBrowserItem> items = new ObservableCollection<ResourceBrowserItem>();

            DirectoryInfo info = new DirectoryInfo(directoryPath);

            // 获取所有的文件夹

            try
            {
                foreach (DirectoryInfo directory in info.GetDirectories())
                {
                    try
                    {
                        ResourceBrowserItem i = new ResourceBrowserItem(directory.FullName, false);
                        items.Add(i);
                    }
                    catch { }
                }
            }
            catch { }

            return items;
        }

        /// <summary>
        /// 获取文件夹下的所有文件
        /// </summary>
        /// <param name="directoryPath">路径</param>
        /// <param name="filter">筛选条件</param>
        /// <returns></returns>
        public static ObservableCollection<ResourceBrowserItem> GetFiles(string directoryPath, List<ResourceBrowserFilterItem> filter)
        {
            if (directoryPath.IsNullOrEmptyOrWhiteSpace())
                return null;

            ObservableCollection<ResourceBrowserItem> items = new ObservableCollection<ResourceBrowserItem>();

            DirectoryInfo info = new DirectoryInfo(directoryPath);

            // 获取所有文件

            try
            {
                if (filter == null || filter.Count == 0)
                {
                    foreach (FileInfo fi in info.GetFiles())
                    {
                        try
                        {
                            ResourceBrowserItem i = new ResourceBrowserItem(fi.FullName, true);
                            items.Add(i);
                        }
                        catch { }
                    }
                }
                else
                {
                    foreach (ResourceBrowserFilterItem f in filter)
                    {
                        foreach (string f_str in f.Filters)
                        {
                            foreach (FileInfo fi in info.GetFiles(f_str))
                            {
                                try
                                {
                                    ResourceBrowserItem i = new ResourceBrowserItem(fi.FullName, true);
                                    items.Add(i);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { }

            return items;
        }

        /// <summary>
        /// 根据提供的路径查找
        /// </summary>
        /// <param name="root">根</param>
        /// <param name="path">要查找的路径</param>
        /// <returns></returns>
        public static ResourceBrowserItem FindItem(IEnumerable root, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;
            string[] tokens = path.Split(new string[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);
            ResourceBrowserItem result = null;
            foreach (ResourceBrowserItem item in root)
            {
                result = _FindItem(item, 0, tokens);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// 查找底第index位序的token节点
        /// </summary>
        /// <param name="root">开始查找的节点</param>
        /// <param name="index">标志为序</param>
        /// <param name="tokens">标志集合</param>
        /// <returns></returns>
        private static ResourceBrowserItem _FindItem(ResourceBrowserItem root, int index, string[] tokens)
        {
            if (tokens == null || index >= tokens.Length || root == null)
                return null;

            string token = tokens[index].ToLower();
            if (root.Token.Equals(token))
            {
                if (index == tokens.Length - 1)
                    return root;
                if (root.Items == null)
                    return null;
                foreach (ResourceBrowserItem item in root.Items)
                {
                    ResourceBrowserItem result = _FindItem(item, index + 1, tokens);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        /// <summary>
        /// 跳过root的FullPath查找路径节点
        /// </summary>
        /// <param name="root">根</param>
        /// <param name="path">要查找的路径</param>
        /// <returns></returns>
        public static ResourceBrowserItem FindItemEx(ResourceBrowserItem root, string path)
        {
            if (root == null || string.IsNullOrWhiteSpace(path))
                return null;
            if (root.FullPath.ToLower().Equals(path.ToLower()))
                return root;
            if (root.FullPath.Length > path.Length)
                return null;

            string[] tokens = path.ToLower().Replace(root.FullPath.ToLower(), string.Empty).Split(new string[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);
            ResourceBrowserItem result = null;
            if (root.Items == null)
            {
                return null;
            }
            foreach (ResourceBrowserItem item in root.Items)
            {
                result = _FindItem(item, 0, tokens);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// 获取筛选项
        /// </summary>
        /// <param name="filter">筛选条件</param>
        /// <returns></returns>
        public static List<ResourceBrowserFilterItem> GetFilterItem(string filter)
        {
            if (filter.IsNullOrEmptyOrWhiteSpace())
            {
                return ResourceBrowserHelper.GetDefaultFilterItem();
            }
            string[] tokens = filter.Split('|');
            if (tokens.Length % 2 != 0)
            {
                return ResourceBrowserHelper.GetDefaultFilterItem();
            }
            List<ResourceBrowserFilterItem> list = new List<ResourceBrowserFilterItem>();
            for (int i = 0; i < tokens.Length; i += 2)
            {
                ResourceBrowserFilterItem item = new ResourceBrowserFilterItem();
                item.Label = tokens[i];
                string str_types = tokens[i + 1];
                string[] types = str_types.Split(';');
                item.Filters = new List<string>();
                item.Filters.AddRange(types);
                item.DisplayLabel = item.Label + "(" + str_types + ")";
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 获取默认的筛选项
        /// </summary>
        /// <returns></returns>
        public static List<ResourceBrowserFilterItem> GetDefaultFilterItem()
        {
            List<ResourceBrowserFilterItem> list = new List<ResourceBrowserFilterItem>();
            ResourceBrowserFilterItem _default = new ResourceBrowserFilterItem();
            _default.Label = "所有文件";
            _default.Filters = new List<string>();
            _default.Filters.Add("*.*");
            _default.DisplayLabel = _default.Label + "(" + "*.*" + ")";
            list.Add(_default);
            return list;
        }

        /// <summary>
        /// 是否是逻辑根节点下边的节点
        /// </summary>
        /// <param name="root">查询开始节点</param>
        /// <param name="item">要查询的节点</param>
        /// <returns></returns>
        public static bool IsLogicRootChildrenItem(IEnumerable root, ResourceBrowserItem item)
        {
            ResourceBrowserItem parent = item.Parent;
            List<ResourceBrowserItem> list = new List<ResourceBrowserItem>();
            foreach (ResourceBrowserItem i in root)
            {
                if (i.IsLogicRoot)
                    list.Add(i);
            }
            while (parent != null || list.Contains(parent))
            {
                parent = parent.Parent;
            }
            if (parent == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// item1 是否是 item2 的父亲
        /// </summary>
        /// <param name="item1">要判断的节点1</param>
        /// <param name="item2">要判断的节点2</param>
        /// <returns></returns>
        public static bool IsParent(ResourceBrowserItem item1, ResourceBrowserItem item2)
        {
            if (item1 == null || item2 == null)
                return false;
            if (item1 == item2)
                return true;

            ResourceBrowserItem parent = item2.Parent;
            while (parent != null)
            {
                if (parent == item1)
                    return true;

                parent = parent.Parent;
            }

            return false;
        }

        /// <summary>
        /// 查找某个节点的根节点
        /// </summary>
        /// <param name="item">要查找的节点</param>
        /// <returns></returns>
        public static ResourceBrowserItem FindRoot(ResourceBrowserItem item)
        {
            if (item.Parent == null)
                return item;

            ResourceBrowserItem temp = item.Parent;
            while (temp.Parent != null)
            {
                temp = temp.Parent;
            }

            return temp;
        }
    }
}
