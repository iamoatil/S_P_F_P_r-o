using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains.Contract;
using XLY.SF.Project.Domains.Contract.DataItemContract;

/* ==============================================================================
* Description：AbstractDataItem  
* Author     ：Fhjun
* Create Date：2017/3/17 16:58:36
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 单行数据的基类
    /// </summary>
    [Serializable]
    public abstract class AbstractDataItem : IDataState, INotifyPropertyChanged
    {
        protected AbstractDataItem()
        {
        }

        #region Base Property
        /// <summary>
        /// 数据状态，正常还是删除
        /// </summary>
        [Display(Visibility = EnumDisplayVisibility.ShowInDatabase)]
        public EnumDataState DataState { get; set; } = EnumDataState.Normal;

        /// <summary>
        /// 数据状态描述，正常还是删除
        /// </summary>
        [Display(Visibility = EnumDisplayVisibility.ShowInUI)]
        public string DataStateDesc => DataState.GetDescription();

        private string _md5 = null;

        /// <summary>
        /// 单行数据的MD5值
        /// </summary>
        [Display]
        public string MD5
        {
            get
            {
                if (_md5 == null)
                {
                    _md5 = BuildMd5();
                }
                return _md5;
            }
            set { _md5 = value; }
        }

        private int _bookMarkId = -1;
        /// <summary>
        /// 加入书签的编号，小于0则未加入书签
        /// </summary>
        [Display]
        public int BookMarkId { get => _bookMarkId; set { _bookMarkId = value; OnPropertyChanged(); } }

        private bool _isVisible = true;
        /// <summary>
        /// 是否可见，如果不满足过滤条件，则为false
        /// </summary>
        [Display]
        public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(); } }

        private bool _isSensitive = false;
        /// <summary>
        /// 是否是敏感数据，比如包含了“东突”等暴恐信息
        /// </summary>
        public bool IsSensitive { get => _isSensitive; set { _isSensitive = value; OnPropertyChanged(); } }
        #endregion

        #region MD5
        /// <summary>
        /// 实现MD5值的生成算法
        /// </summary>
        /// <returns></returns>
        public virtual string BuildMd5()
        {
            return ExpressionHelper.Md5(this, typeof(DisplayAttribute));
        }
        #endregion

        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
