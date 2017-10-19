using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 资源项
    /// </summary>
    public class ResourceBrowserItem : ViewModelBase
    {
        /// <summary>
        /// 构建一个资源项
        /// </summary>
        /// <param name="isFile">是否是文件</param>
        /// <param name="path">路径</param>
        /// <param name="icon">图标</param>
        /// <param name="name">标题</param>
        public ResourceBrowserItem(string path, bool? isFile = null, Icon icon = null, string name = null)
        {
            this.FullPath = path;
            this.Icon = icon;
            if (isFile == null)
            {
                this.IsFile = ResourceBrowserHelper.IsFile(path);
            }
            else
            {
                this.IsFile = isFile.Value;
            }
            if (this.IsFile)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(this.FullPath);
                if (name == null)
                {
                    this.Name = fi.Name;
                }
                else
                {
                    this.Name = name;
                }
                this.Token = fi.Name.Replace("\\", string.Empty);
                if (icon == null)
                {
                    this.Icon = ResourceBrowserHelper.GetFileIcon(fi.FullName);
                }
            }
            else
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(this.FullPath);
                if (name == null)
                {
                    this.Name = di.Name;
                }
                else
                {
                    this.Name = name;
                }
                this.Token = di.Name.Replace("\\", string.Empty).ToLower();
                if (icon == null)
                {
                    this.Icon = ResourceBrowserHelper.GetDirectoryIcon();
                }
            }
        }

        #region Icon -- 图标

        private Icon _Icon;
        /// <summary>
        /// 图标路径
        /// </summary>
        public Icon Icon
        {
            get
            {
                return _Icon;
            }
            set
            {
                _Icon = value;
                this.OnPropertyChanged("Icon");
            }
        }

        #endregion

        #region Name -- 名称

        private string _Name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        #endregion

        #region FullPath -- 完整路径

        private string _FullPath;
        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath
        {
            get { return _FullPath; }
            set { _FullPath = value; this.OnPropertyChanged("FullPath"); }
        }

        #endregion

        #region IsFile -- 当前项是否是文件

        private bool _IsFile;
        /// <summary>
        /// 当前项是否是文件
        /// </summary>
        public bool IsFile
        {
            get { return _IsFile; }
            set { _IsFile = value; this.OnPropertyChanged("IsFile"); }
        }

        #endregion

        #region Parent -- 父级信息

        private ResourceBrowserItem _Parent;
        /// <summary>
        /// 父级信息
        /// </summary>
        public ResourceBrowserItem Parent
        {
            get { return _Parent; }
            set { _Parent = value; this.OnPropertyChanged("Parent"); }
        }

        #endregion

        #region Items -- 子项集合

        private System.Collections.ObjectModel.ObservableCollection<ResourceBrowserItem> _Items;
        /// <summary>
        /// 子项集合
        /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<ResourceBrowserItem> Items
        {
            get { return _Items; }
            set { _Items = value; this.OnPropertyChanged("子项集合"); }
        }

        #endregion

        #region Token -- 节点块标志

        private string _Token;
        /// <summary>
        /// 节点块标志
        /// </summary>
        public string Token
        {
            get { return _Token; }
            set { _Token = value; this.OnPropertyChanged("Token"); }
        }

        #endregion

        #region IsRoot -- 当前节点是否是逻辑根节点

        private bool _IsLogicRoot;
        /// <summary>
        /// 当前节点是否是逻辑根节点
        /// </summary>
        public bool IsLogicRoot
        {
            get { return _IsLogicRoot; }
            set { _IsLogicRoot = value; }
        }

        #endregion
    }
}
