using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/16 13:36:38
 * 类功能说明：分页查询基础。
 * 1. 分页请求信息
 * 2. 分页返回数据结果，包含数据集
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.EntityBase
{
    /// <summary>
    /// 分页请求结果
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class QueryOfPaging<TModel> : NotifyPropertyBase
        where TModel : DbModelBase
    {
        #region 总页数

        private int _pageCount;
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                return this._pageCount;
            }

            set
            {
                this._pageCount = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 当前页码

        private int _curPageNo;
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurPageNo
        {
            get
            {
                return this._curPageNo;
            }

            set
            {
                this._curPageNo = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 一页数据量

        private int _pageItemCount;
        /// <summary>
        /// 一页数据量
        /// </summary>
        public int PageItemCount
        {
            get
            {
                return this._pageItemCount;
            }

            set
            {
                this._pageItemCount = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        /// <summary>
        /// 页面数据
        /// </summary>
        public ObservableCollection<TModel> PageSource { get; set; }
    }

    /// <summary>
    /// 分页请求
    /// </summary>e
    public class PagingRequest
    {
        /// <summary>
        /// 分页请求数据
        /// </summary>
        /// <param name="requestPageNo">请求的页码</param>
        /// <param name="requestPageItemCount"></param>
        public PagingRequest(int requestPageNo, int requestPageItemCount)
        {
            CurPageNo = requestPageNo;
            PageItemCount = requestPageItemCount;
        }

        /// <summary>
        /// 当前页码，1开始
        /// </summary>
        public int CurPageNo { get; private set; }

        /// <summary>
        /// 一页数据量
        /// </summary>
        public int PageItemCount { get; private set; }
    }
}
