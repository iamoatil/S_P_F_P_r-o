using System;
using System.Collections.Generic;
using System.Linq;
using XLY.SF.Project.Domains.Contract;

/* ==============================================================================
* Description：AbstractDataSource  
* Author     ：Fhjun
* Create Date：2017/3/17 16:30:01
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据基类
    /// </summary>
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
    public abstract class AbstractDataSource : IDataSource
    {
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// 数据提取时使用的任务路径 ，该属性用于修正任务包路径修改后导致的路径不正确的问题
        /// </summary>
        public string DataExtractionTaskPath { get; set; }

        /// <summary>
        /// 当前任务路径
        /// </summary>
        public string CurrentTaskPath { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        [Newtonsoft.Json.JsonConverter(typeof(DataItemJsonConverter))]
        public IDataItems Items { get; set; }

        /// <summary>
        /// 当前列表数据的类型，如果是C#插件则为Items的Type，如果为脚本插件为自定义类型名称
        /// </summary>
        public object Type { get; set; }

        public IPluginInfo PluginInfo { get ; set ; }

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

        public virtual void BuildParent()
        {
            if (null != Items)
            {
                Items.Commit();
            }
        }

        public virtual void Traverse(Predicate<TreeNode> traverseTreeNode, Predicate<AbstractDataItem> traverseItems)
        {
            if (traverseItems != null && Items != null)
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

        public virtual IEnumerable<T> Filter<T>(params FilterArgs[] args)
        {
            if(Items == null)
            {
                _total = 0;
                return new T[0];
            }
            Items.Filter(args);
            IEnumerable<T> result = Items.View as IEnumerable<T> ?? new T[0];
            _total = Items.Count;
            return result;
        }

        #endregion
    }
}
