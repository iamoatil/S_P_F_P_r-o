using System;
using System.Collections.Generic;
using System.Linq;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 提取项集合
    /// </summary>
    public class ExtractItemCollection : List<ExtractItem>
    {
        /// <summary>
        /// 获取提取项总数
        /// </summary>
        public int ExtractItemCount
        {
            get { return this.Sum(s => s.Count); }
        }

        /// <summary>
        /// 获取被选中的提取项总数.
        /// </summary>
        public int SelectedExtractItemCount
        {
            get { return this.Sum(s => s.SelectedCount); }
        }

        /// <summary>
        /// 获取所有选中的数据提取项
        /// </summary>
        /// <returns></returns>
        public List<ExtractItem> GetAllCheckedExtractItem()
        {
            List<ExtractItem> list = new List<ExtractItem>();

            foreach (var ei in this)
            {
                list.AddRange(ei.Items.Where(i => i.Checked.HasValue && i.Checked.Value));
            }

            return list;
        }

        /// <summary>
        /// 从插件集合中加载提取项
        /// </summary>
        /// <param name="plugins"></param>
        public void Load(IEnumerable<DataParsePluginInfo> plugins)
        {
            Clear();
            if (!plugins.IsValid())
            {
                return;
            }

            var gs = plugins.OrderBy(s => s.GroupIndex).Select(s => s.Group).Distinct();

            if (!gs.IsValid())
            {
                return;
            }

            foreach (var g in gs)
            {
                LoadItem(plugins, g);
            }
        }

        private void LoadItem(IEnumerable<DataParsePluginInfo> plugins, string g)
        {
            ExtractItem group, item;
            @group = new ExtractItem();
            @group.Name = g;
            @group.Items = new List<ExtractItem>();
            //find child items.
            var ns = plugins.Where(s => s.Group == g).OrderBy(s => s.OrderIndex).Select(s => s.Name).Distinct();
            if (!ns.IsValid())
            {
                return;
            }

            foreach (var n in ns)
            {
                var pn = plugins.Where(s => s.Name == n).FirstOrDefault();

                item = new ExtractItem();
                item.Name = n;
                item.Parent = @group;
                item.Icon = pn.Icon;
                item.AppName = pn.AppName;
                @group.Items.Add(item);
            }
            Add(@group);
        }
    }

    #region ExtractItem

    /// <summary>
    /// 提取数据项
    /// </summary>
    public class ExtractItem : NotifyPropertyBase
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// App名称
        /// </summary>
        public string AppName { get; set; }

        #region Checked -- 选择状态

        /// <summary>
        /// 是否执行联动
        /// </summary>
        private bool _DoLinkage = true;

        private bool? _Checked;
        /// <summary>
        /// 选择状态
        /// </summary>
        public bool? Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                _Checked = value;
                OnPropertyChanged("Checked");
                // 设置父子联动
                SetCheckState(value);
            }
        }

        #endregion

        /// <summary>
        /// 父级节点
        /// </summary>
        public ExtractItem Parent { get; set; }

        /// <summary>
        /// 支持的提取方式
        /// </summary>
        public EnumPump Pump { get; set; }

        /// <summary>
        /// 是否可用，true可用
        /// （注意此属性根据提取方式生成）
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 是否提取完成
        /// </summary>
        public bool IsFinish { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 子项的数量
        /// </summary>
        public int Count
        {
            get { return Items != null && Items.Any() ? Items.Count : 0; }
        }

        /// <summary>
        /// 子项选中的数量
        /// </summary>
        public int SelectedCount
        {
            get { return Items != null && Items.Any() ? Items.Count(s => s.Checked ?? false) : 0; }
        }

        /// <summary>
        /// 子项集合
        /// </summary>
        public List<ExtractItem> Items { get; set; }

        public ExtractItem()
        {
            Checked = false;
            Name = string.Empty;
            GUID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        public ExtractItem DeepClone()
        {
            ExtractItem item = new ExtractItem();
            item.Name = Name;
            item.Checked = Checked;
            item.Pump = Pump;
            item.Parent = Parent;
            item.Icon = Icon;
            item.AppName = AppName;
            if (Items != null && Items.Any())
            {
                item.Items = new List<ExtractItem>();
                Items.ForEach(n =>
                    {
                        ExtractItem ni = n.DeepClone();
                        ni.Parent = item;
                        item.Items.Add(ni);
                    });
            }
            return item;
        }

        /****************** public methods ******************/

        /// <summary>
        /// 遍历该节点和该节点的所有子节点
        /// </summary>
        /// <param name="action">遍历行为</param>
        public void Traversal(Action<ExtractItem> action)
        {
            _Traversal(this, action);
        }

        private void _Traversal(ExtractItem item, Action<ExtractItem> action)
        {
            action(item);
            if (item.Items != null && item.Items.Any())
            {
                foreach (ExtractItem i in item.Items)
                {
                    _Traversal(i, action);
                }
            }
        }

        /****************** private methods ******************/

        private void SetCheckState(bool? value)
        {
            if (_DoLinkage && Items != null && Items.Any())
            {
                foreach (var i in Items.Where(i => i.Checked != value))
                {
                    i.Checked = value;
                }
            }

            // 联动父级
            if (Parent == null || Parent.Items != null || Parent.Items.Any())
            {
                return;
            }

            if (Parent.Items.All(i => i.Checked == true))
            {
                if (Parent.Checked == false || Parent.Checked == null)
                {
                    Parent.Checked = true;
                }
            }
            else if (Parent.Items.All(i => i.Checked == false))
            {
                if (Parent.Checked == true || Parent.Checked == null)
                {
                    Parent.Checked = false;
                }
            }
            else
            {
                if (Parent.Checked != null)
                {
                    Parent._DoLinkage = false;
                    Parent.Checked = null;
                    Parent._DoLinkage = true;
                }
            }
        }
    }
    #endregion
}
