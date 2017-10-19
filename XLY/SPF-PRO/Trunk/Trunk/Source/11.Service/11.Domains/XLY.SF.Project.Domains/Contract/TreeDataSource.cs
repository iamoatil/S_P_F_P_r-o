using System;
using System.Collections.Generic;
using System.Linq;
using XLY.SF.Project.Domains.Contract;

/* ==============================================================================
* Description：TreeDataSource  
* Author     ：Fhjun
* Create Date：2017/3/17 16:47:18
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 复杂数据节点
    /// </summary>
    [Serializable]
    public class TreeDataSource : AbstractDataSource
    {
        /// <summary>
        /// 导航树
        /// </summary>
        public List<TreeNode> TreeNodes { get; set; }

        public TreeDataSource()
        {
            TreeNodes = new List<TreeNode>();
        }

        public override void BuildParent()
        {
            if (this.TreeNodes.Any())
            {
                this.TreeNodes.ForEach((n) =>
                {
                    n.Commit();
                    n.Parent = this;
                    n.BuildParent();
                });
            }

            base.BuildParent();
        }

        public override void Traverse(Predicate<TreeNode> traverseTreeNode, Predicate<AbstractDataItem> traverseItems)
        {
            if(traverseTreeNode != null)
            {
                foreach(TreeNode node in TreeNodes)
                {
                    if (!traverseTreeNode(node))
                    {
                        break;
                    }
                    node.Traverse(traverseTreeNode, traverseItems);
                }
            }
            base.Traverse(traverseTreeNode, traverseItems);
        }

        public override IEnumerable<T> Filter<T>(params FilterArgs[] args)
        {
            IEnumerable<T> result = base.Filter<T>(args);
            IEnumerable<T> temp = null;
            if (this.TreeNodes.Any())
            {
                foreach (var node in TreeNodes)
                {
                    temp = node.Filter<T>(args);
                    if (temp == null) continue;
                    Total += node.Total;
                    result = result.Union(temp);
                }
            }
            return result;
        }
    }
}
