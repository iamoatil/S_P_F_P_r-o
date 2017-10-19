using System.Collections.Generic;

/* ==============================================================================
* Description：ITree  
* Author     ：Fhjun
* Create Date：2017/3/20 9:44:58
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 复杂树接口
    /// </summary>
    public interface ITree
    {
        /// <summary>
        /// 导航树
        /// </summary>
        List<TreeNode> TreeNodes { get; set; }
    }
}
