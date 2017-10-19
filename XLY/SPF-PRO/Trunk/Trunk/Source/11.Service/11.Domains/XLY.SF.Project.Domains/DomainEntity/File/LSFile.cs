using System;
using System.Linq;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// LS指令输出的文件信息
    /// </summary>
    public class LSFile
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string FullPath
        {
            get { return string.Format("{0}/{1}", this.Path.TrimEnd('/'), this.Name); }
        }

        public DateTime CreateDate { get; set; }
        public DateTime LastAccessDate { get; set; }
        public DateTime LastWriteData { get; set; }

        public int Size { get; set; }

        public bool IsFolder { get; set; }

        public string Permission { get; set; }

        public string Type { get; set; }

        public string LinkPath { get; set; }

        /// <summary>
        /// true:根文件
        /// </summary>
        public bool IsRootFile { get; set; }

        /// <summary>
        /// 如果是文件，则为文件的MD5值
        /// </summary>
        public string MD5 { get; set; }

        /// <summary>
        /// ture：有读取权限
        /// </summary>
        public bool HasPermission
        {
            get { return this.Permission.Count(s => s == 'r') >= 3; }
        }
        public EnumDataState DataState { get; set; }
        public EnumColumnType ColumnType { get; set; }

        public LSFile()
        {
            this.Permission = string.Empty;
            this.Size = 0;
            this.IsFolder = false;
            this.IsRootFile = false;
        }
    }
}
