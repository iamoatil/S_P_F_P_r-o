using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    public class FNodeX
    {
        private string _fullPath;
        private string _directory;
        private ulong _ParentKey;
        public ulong _key;

        [Flags]
        internal enum BottomAttributeFlag : short
        {
            Normal = 0X0001,
            Directory = 0X0002,
            Lost = 0X0004,
            Raw = 0X0008,
            PartCover = 0X0010,
            AllCover = 0X0020,
            NTFSCompress = 0X0040,
            NTFSEncrypt = 0X0080,
            TrueCrypt = 0X0100,
            Chain = 0X0200,
            ReBuild = 0X0400,
            ADS = 0X0800,
            INDX = 0X1000,
            TXFAndOther = 0X2000
        }

        /// <summary>
        /// 当前节点Key
        /// </summary>
        public ulong Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// 当前节点的父节点Key
        /// </summary>
        public ulong ParentKey
        {
            get { return _ParentKey; }
            set { _ParentKey = value; }
        }

        /// <summary>
        /// 父节点
        /// </summary>
        public FNodeX Parent { get; set; }

        /// <summary>
        /// 是否是文件夹
        /// </summary>
        public bool IsFolder
        {
            get { return (this.Source.FileAttribute & (short)BottomAttributeFlag.Directory) == (short)BottomAttributeFlag.Directory; }
        }

        /// <summary>
        /// 判断是否是删除文件
        /// </summary>
        public bool IsDelete
        {
            get
            {
                return (this.Source.FileAttribute & (short)BottomAttributeFlag.Normal) != (short)BottomAttributeFlag.Normal;
            }
        }

        /// <summary>
        /// 是不是根目录
        /// </summary>
        public bool IsRoot { get; set; }

        /// <summary>
        /// 判断路径是否以指定字符开头开头
        /// </summary>
        public bool IsPath2DataStart(string charStr = "/data/data")
        {
            return this.FullPath.StartsWith(charStr);
        }

        /// <summary>
        /// 当前节点是否是分区
        /// </summary>
        public bool IsPartition { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public ulong Size
        {
            get { return this.Source.Size; }
        }

        private string _fileName = string.Empty;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                // 20160628 by luochao 文件名中不允许使用的字符使用相似字符进行替换，其它均替换为空
                _fileName = FileHelper.FilterInvalidFileName(value);
            }
        }

        /// <summary>
        /// 文件所属分区装载句柄
        /// </summary>
        public IntPtr Mount { get; set; }

        private NodeCollection _collection;
        /// <summary>
        /// 当前节点下子节点
        /// </summary>
        public NodeCollection Collection
        {
            get { return _collection ?? (_collection = new NodeCollection { Parent = this }); }
        }

        /// <summary>
        /// 当前节点原始对象
        /// </summary>
        public DIR_FILE_NODE_INFO Source { get; set; }

        /// <summary>
        /// 当前节点的FILEX对象
        /// </summary>
        /// <returns></returns>
        public FileX GetFileX()
        {
            return new FileX();
        }


        /// <summary>
        /// 当前节点的全路径
        /// </summary>
        public string FullPath
        {
            get
            {
                try
                {
                    if (this.IsRoot || this.IsPartition)
                    {
                        return this._fullPath;
                    }
                    if (string.IsNullOrWhiteSpace(this._fullPath))
                    {
                        this._fullPath = FileHelper.ConnectPath(this.Directory ?? "", this.FileName);
                    }
                    return this._fullPath;
                }
                catch
                {
                    //LogHelper.Error(ex.Message, ex);
                }
                return this._fullPath;
            }
        }

        /// <summary>
        /// 节点目录
        /// </summary>
        public string Directory
        {
            get { return _directory; }
            set { _directory = value; }
        }
    }

    public class NodeCollection : List<FNodeX>
    {
        public FNodeX Parent { get; set; }

        public new void Add(FNodeX node)
        {
            base.Add(node);
            node.Parent = this.Parent;
        }

        public new void AddRange(IEnumerable<FNodeX> collection)
        {
            if (collection == null)
            {
                return;
            }

            foreach (var dfTreeNode in collection)
            {
                Add(dfTreeNode);
            }
        }
    }

    public static class FNodeXExtension
    {
        /// <summary>
        /// 获取所有条件想匹配的节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<FNodeX> FindAll(this FNodeX node, Predicate<FNodeX> predicate)
        {
            var fnodexs = new Collection<FNodeX>();
            if (predicate(node))
            {
                fnodexs.Add(node);
            }
            if (node.Collection.Count != 0)
            {
                foreach (var n in node.Collection)
                {
                    if (n.IsFolder || n.IsPartition || n.IsRoot)
                    {
                        fnodexs.AddRange(FindAll(n, predicate));
                    }
                }
            }
            return fnodexs;
        }
        public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> values)
        {
            foreach (var v in values)
            {
                @this.Add(v);
            }
        }
    }
}
