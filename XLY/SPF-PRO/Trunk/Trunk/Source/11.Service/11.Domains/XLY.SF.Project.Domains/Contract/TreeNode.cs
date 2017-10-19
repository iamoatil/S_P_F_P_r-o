using System;
using System.Collections.Generic;
using System.Linq;
using XLY.SF.Project.Domains.Contract;
using XLY.SF.Project.Domains.Contract.DataItemContract;

/* ==============================================================================
* Description：TreeNode  
* Author     ：Fhjun
* Create Date：2017/3/17 16:49:26
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 树节点
    /// </summary>
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
    public class TreeNode : IDataState, ITraverse
    {
        public TreeNode()
        {
            this.TreeNodes = new List<TreeNode>();
        }

        /// <summary>
        /// 数据状态
        /// </summary>
        public EnumDataState DataState { get; set; }

        /// <summary>
        /// 树节点Id。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 显示的内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<TreeNode> TreeNodes { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public object Parent { get; internal set; }

        /// <summary>
        /// 数据集合
        /// </summary>
        [Newtonsoft.Json.JsonConverter(typeof(DataItemJsonConverter))]
        public IDataItems Items { get; set; }

        private object _type = null;
        /// <summary>
        /// Items的数据类型，脚本中须配置
        /// </summary>
        public object Type
        {
            get
            {
                if (_type == null && Items != null)
                {
                    if (Items.GetType().IsGenericType)
                    {
                        _type = Items.GetType().GetGenericArguments()[0];
                    }
                    else
                    {
                        _type = Items.GetType();
                    }
                }
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// 是否隐藏子节点（在数据展示页面有效），为true则不显示子节点，此时子节点显示由数据展示插件控制
        /// </summary>
        public bool IsHideChildren { get; set; }

        /// <summary>
        /// 是否计算到总数上去，为false则表示节点下的数据不会被计入上层节点的总数
        /// </summary>
        public bool IsIncludeInTotal { get; set; } = true;

        private int _total = -1;
        public int Total
        {
            get
            {
                if (_total == -1)
                {
                    Filter<dynamic>();
                }
                return _total;
            }
            protected set
            {
                _total = value;
            }
        }

        public void Commit()
        {
            if (null != Items)
            {
                Items.Commit();
            }
        }

        public void BuildParent()
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
        }

        public override string ToString()
        {
            return this.Text;
        }

        public void Traverse(Predicate<TreeNode> traverseTreeNode, Predicate<AbstractDataItem> traverseItems)
        {
            if (traverseTreeNode != null)
            {
                foreach (TreeNode node in TreeNodes)
                {
                    if (!traverseTreeNode(node))
                    {
                        return;
                    }
                    node.Traverse(traverseTreeNode, traverseItems);
                }
            }
            if(traverseItems != null && Items != null)
            {
                foreach (AbstractDataItem item in Items.View)
                {
                    if (!traverseItems(item))
                    {
                        break;
                    }
                }
            }
        }

        #region 数据查询

        public IEnumerable<T> Filter<T>(params FilterArgs[] args)
        {
            IEnumerable<T> temp = null;
            IEnumerable<T> result = new T[0];
            Int32 subNodeTotal = 0;
            if (this.TreeNodes.Any())
            {
                foreach (var node in TreeNodes)
                {
                    temp = node.Filter<T>(args);
                    if (temp == null) continue;
                    subNodeTotal += node.Total;
                    result = result.Union(temp);
                }
            }
            if (Items != null)
            {
                Items.Filter(args);
                result = result.Union(Items.View as IEnumerable<T> ?? new T[0]);
                _total = Items.Count + subNodeTotal;
            }
            else
            {
                _total = subNodeTotal;
            }

            return result;
        }
        #endregion

    }
}
