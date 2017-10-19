using System;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.Domains
{
    public class MountPoint
    {
        public MountPoint() { }

        public MountPoint(string block, string name, string fs, bool readOnly)
        {
            Block = block;
            Name = name;
            FileSystem = fs;
            IsReadOnly = readOnly;
            //index
            var len = block.Length;
            if (len >= 3)
            {
                try
                {
                    var v = block[len - 3] == 'p' ? block.Substring(len - 2, 2) : block.Substring(len - 1, 1);
                    Index = v.ToSafeInt();
                }
                catch (Exception)
                {
                    Index = -1;
                }
            }
        }

        /// <summary>
        /// Gets the mount point block
        /// 块区
        /// </summary>
        /// <value>The block.</value>
        public String Block { get; set; }

        public string BlockName
        {
            get { return FileHelper.GetFileName(Block, '/'); }
        }

        /// <summary>
        /// Gets the mount point name
        /// 名称
        /// </summary>
        /// <value>The name.</value>
        public String Name { get; set; }

        /// <summary>
        /// Gets the mount point file system
        /// 文件结构
        /// </summary>
        /// <value>The file system.</value>
        public String FileSystem { get; set; }

        /// <summary>
        /// Gets the mount point access
        /// 是否只读
        /// 
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// 块区序号
        /// </summary>
        public int Index;

        /// <summary>
        /// 大小，字节数
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Returns a string representation of the mount point.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", Block, Index, Name, FileSystem, IsReadOnly ? "ro" : "rw",
                string.Format("{0}({1})", Size, FileHelper.GetFileSize(Size)));
        }


    }
}