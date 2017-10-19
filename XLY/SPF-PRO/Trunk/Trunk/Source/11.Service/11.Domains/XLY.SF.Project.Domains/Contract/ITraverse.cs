using System;

/* ==============================================================================
* Description：ITraverse  
* Author     ：Fhjun
* Create Date：2017/4/10 16:47:25
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 提供对数据的遍历接口
    /// </summary>
    public interface ITraverse
    {
        /// <summary>
        /// 遍历所有的数据节点
        /// </summary>
        /// <param name="traverseTreeNode"></param>
        void Traverse(Predicate<TreeNode> traverseTreeNode, Predicate<AbstractDataItem> traverseItems);
    }
}
