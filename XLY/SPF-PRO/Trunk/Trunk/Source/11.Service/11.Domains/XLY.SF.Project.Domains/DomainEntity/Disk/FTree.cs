using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 文件结构树
    /// </summary>
    public class FTreeNode
    {
        public FTreeNode()
        {
            this.Files = new Collection<FileX>();
            this.FullPath = string.Empty;
        }

        /// <summary>
        /// 当前节点ID
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public ulong ParentID { get; set; }

        private FTreeCollection _children;
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 当前节点的文件信息
        /// </summary>
        public FileX File { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public FTreeNode Parent { get; set; }

        /// <summary>
        /// 当前节点是否选中
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// 当前节点下的文件信息
        /// </summary>
        public ICollection<FileX> Files { get; set; }

        /// <summary>
        /// 当前节点的全路径（也是文件的全路径）
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 当前节点的子目录
        /// </summary>
        public FTreeCollection Children
        {
            get { return _children ?? (_children = new FTreeCollection { Parent = this, ParentID = this.ParentID }); }
            set { this._children = value; }
        }

        /// <summary>
        /// 是否是分区节点
        /// </summary>
        public bool IsPartition { get; set; }

        /// <summary>
        /// 分区卷名
        /// </summary>
        public string VolName { get; set; }

        /// <summary>
        /// 当前文件所属分区的装载句柄（文件恢复的时候使用）
        /// </summary>
        public IntPtr MountHandle { get; set; }

        /// <summary>
        /// 是否是文件夹
        /// </summary>
        public bool IsDirctory
        {
            get
            {
                return this.File == null || this.File.IsFolder || this.IsPartition;
            }
        }
    }

    /// <summary>
    /// 文件结构树扩展
    /// </summary>
    public class FTreeCollection : List<FTreeNode>
    {
        /// <summary>
        /// 当前节点父节点
        /// </summary>
        public FTreeNode Parent { get; set; }

        /// <summary>
        /// 当前节点的父节点ID;
        /// </summary>
        public ulong ParentID { get; set; }

        /// <summary>
        /// 新增一个节点到目录
        /// </summary>
        /// <param name="node"></param>
        public new void Add(FTreeNode node)
        {
            base.Add(node);
            node.Parent = this.Parent;
            node.ParentID = this.ParentID;
        }

        /// <summary>
        /// 追加树结构集合到当前节点
        /// </summary>
        /// <param name="nodes"></param>
        public new void AddRange(IEnumerable<FTreeNode> nodes)
        {
            if (nodes == null)
            {
                return;
            }

            foreach (var node in nodes)
            {
                this.Add(node);
            }
        }
    }

    /// <summary>
    /// 文件树结构扩展信息
    /// </summary>
    public static class FTreeNodeExtension
    {
        public static void SetChecked(this FTreeNode node, bool isChecked)
        {
            node.IsChecked = isChecked;

            if (node.Children != null && node.Children.Any())
            {
                foreach (var childNode in node.Children)
                {
                    SetChecked(childNode, isChecked);
                }
            }
        }

        /// <summary>
        /// 获取当前节点下所有文件,包括子目录下的文件
        /// </summary>
        /// <param name="node">节点信息</param>
        /// <returns></returns>
        public static IEnumerable<FTreeNode> GetFTreeNodes(this FTreeNode node)
        {
            var nodes = new List<FTreeNode>();
            nodes.Add(node);

            if (node.Children.Count == 0)
            {
                return nodes;
            }

            foreach (var fTreeNode in node.Children)
            {
                nodes.AddRange(fTreeNode.GetFTreeNodes());
            }
            return nodes;
        }

        /// <summary>
        /// 获取当前节点下所有文件,包括子目录下的文件
        /// </summary>
        /// <param name="node">节点信息</param>
        /// <param name="suffixs">后缀信息</param>
        /// <returns></returns>
        public static IEnumerable<FTreeNode> GetFTreeNodes(this FTreeNode node, List<string> suffixs)
        {
            var nodes = new List<FTreeNode>();
            if (node.Name != null)
            {
                var result = suffixs.Find(o => node.Name.EndsWith(o));
                if (result != null)
                {
                    nodes.Add(node);
                }
            }

            if (node.Children.Count == 0)
            {
                return nodes;
            }

            foreach (var child in node.Children)
            {
                nodes.AddRange(child.GetFTreeNodes(suffixs));
            }
            return nodes;
        }
    }
}
