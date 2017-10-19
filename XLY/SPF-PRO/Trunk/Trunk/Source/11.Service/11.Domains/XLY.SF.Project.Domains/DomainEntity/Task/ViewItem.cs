using System.Collections.Generic;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.Domains
{
    #region ViewItem
    /// <summary>
    /// 视图项，一级为分组，二级才是具体的视图项配置
    /// </summary>
    public class ViewItem : NotifyPropertyBase
    {
        /// <summary>
        /// 与数据源对应的键值
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否是叶子节点
        /// </summary>
        public bool IsLeaf { get; set; }

        #region IsSelected -- 是否选中

        private bool _IsSelected;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get 
            {
                return _IsSelected; 
            }
            set 
            {
                _IsSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
        }
        
        #endregion

        #region IsExpanded -- 是否展开

        private bool _IsExpanded;
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get 
            {
                return _IsExpanded;
            }
            set 
            {
                _IsExpanded = value;
                this.OnPropertyChanged("IsExpanded");
            }
        }
        
        #endregion

        /// <summary>
        /// 有效总数
        /// </summary>
        public int Total
        {
            get
            {
                return _Total;
            }
            set
            {
                _Total = value;
                this.OnPropertyChanged("Total");
            }
        }

        public int _Total = -2;

        /// <summary>
        /// 子视图集合，具体的视图项配置
        /// </summary>
        public List<ViewItem> Items { get; set; }

    }
    #endregion
}
