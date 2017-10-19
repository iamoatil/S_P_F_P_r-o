using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 文件预览配置
    /// </summary>
    public class FileViewerConfig
    {
        private FileViewerConfig()
        {
            this.Items = new List<FileViewerConfigItem>();
        }

        public List<FileViewerConfigItem> Items { get; set; }

        public XElement ToXElement()
        {
            XElement element = new XElement("FileViewerConfig");
            foreach (FileViewerConfigItem i in this.Items)
            {
                element.Add(i.ToXElement());
            }
            return element;
        }

        public void Load(XElement element)
        {
            foreach (XElement e in element.Elements("FileViewerConfigItem"))
            {
                FileViewerConfigItem i = new FileViewerConfigItem();
                i.Load(e);
                this.Items.Add(i);
            }
        }

        /// <summary>
        /// 判断某个视图类型的某种扩展名是否支持
        /// </summary>
        /// <param name="type">视图类型</param>
        /// <param name="extension">扩展名</param>
        /// <returns></returns>
        public bool IsSupport(FileViewerType type, string extension)
        {
            if (type == FileViewerType.None)
                return true;
            if (extension.IsNullOrEmptyOrWhiteSpace())
                return false;
            var q = from i in this.Items
                    from e in i.Extensions
                    where i.FileViewer == type && e.Extension.ToLower().Trim().Equals(extension.Trim().ToLower())
                    orderby e.Priority
                    select e;
            return q.Count() > 0;
        }

        /// <summary>
        /// 最佳支持的视图（根据视图支持优先级确定）
        /// </summary>
        /// <param name="extension">扩展名</param>
        /// <param name="type">打开方式</param>
        /// <returns></returns>
        public FileViewerType BestSupport(string extension, FileViewerArgsType type)
        {
            if (extension.IsNullOrEmptyOrWhiteSpace())
                return FileViewerType.None;
            var q = from i in this.Items
                    from e in i.Extensions
                    where e.Extension.ToLower().Trim().Equals(extension.Trim().ToLower())
                          && (((type == FileViewerArgsType.Stream || type == FileViewerArgsType.Buffer) && i.IsSupportStream) || type == FileViewerArgsType.Path)
                    orderby e.Priority
                    select i.FileViewer;
            if (q.Count() == 0)
            {
                if (!extension.IsNullOrEmptyOrWhiteSpace() && type == FileViewerArgsType.Path)
                    return FileViewerType.Hex;
                else
                    return FileViewerType.None;
            }
            else
            {
                return q.First();
            }
        }

        /// <summary>
        /// 在指定的视图viewers中选择最佳的支持视图
        /// </summary>
        /// <param name="extension">扩展名</param>
        /// <param name="type">打开方式</param>
        /// <param name="viewers">备选的视图</param>
        /// <returns></returns>
        public FileViewerType BestSupport(string extension, FileViewerArgsType type, params FileViewerType[] viewers)
        {
            if (extension.IsNullOrEmptyOrWhiteSpace())
            {
                if (viewers.Contains(FileViewerType.Hex))
                    return FileViewerType.Hex;
                else
                    return FileViewerType.None;
            }
            var q = from i in this.Items
                    from e in i.Extensions
                    where viewers.Contains(i.FileViewer) && e.Extension.ToLower().Trim().Equals(extension.Trim().ToLower())
                          && (((type == FileViewerArgsType.Stream || type == FileViewerArgsType.Buffer) && i.IsSupportStream) || type == FileViewerArgsType.Path)
                    orderby e.Priority
                    select i.FileViewer;
            if (q.Count() == 0)
            {
                if (viewers.Contains(FileViewerType.Hex))
                    return FileViewerType.Hex;
                else
                    return FileViewerType.None;
            }
            else
                return q.First();
        }

        private static FileViewerConfig _Config;
        /// <summary>
        /// 多视图预览配置
        /// </summary>
        public static FileViewerConfig Config
        {
            get
            {
                if (_Config == null)
                {
                    FileViewerConfig.Load();
                }
                return _Config;
            }
        }

        /// <summary>
        /// 加载默认视图配置信息
        /// </summary>
        public static void Load()
        {
            FileViewerConfig config = new FileViewerConfig();
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("XLY.XDD.Control.Control.FileViewer.Config.FileViewerConfig.xml"))
            {
                XElement element = XElement.Load(stream);
                config.Load(element);
                _Config = config;
            }
        }

        /// <summary>
        /// 加载视图配置信息
        /// </summary>
        /// <param name="path">配置文件路径</param>
        public static void Load(string path)
        {
            FileViewerConfig config = new FileViewerConfig();
            XElement element = XElement.Load(path);
            config.Load(element);
            _Config = config;
        }
    }
}
