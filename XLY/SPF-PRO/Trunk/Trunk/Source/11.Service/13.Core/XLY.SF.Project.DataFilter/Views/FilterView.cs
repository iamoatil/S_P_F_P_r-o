using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.DataFilter.Views
{
    /// <summary>
    /// 针对某种特定的视图的过滤器。
    /// </summary>
    /// <typeparam name="T">返回的结果的类型。</typeparam>
    public abstract class FilterView<T>
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataFilter.FilterView 实例。
        /// </summary>
        /// <param name="source">实现了 IFilterable 接口的类型实例。</param>
        protected FilterView(IFilterable source)
        {
            Source = source;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 数据提供器。
        /// </summary>
        public IFilterable Source { get; }

        /// <summary>
        /// 表达式。
        /// </summary>
        public Expression Expression { get; set; }

        /// <summary>
        /// 过滤后的视图。
        /// </summary>
        public IEnumerable<T> View { get; private set; }

        /// <summary>
        /// 视图的大小。
        /// </summary>
        public Int32 Count { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 执行一次过滤。
        /// </summary>
        public virtual void Filter()
        {
            View = Source.Provider.Query<T>(Expression,out Int32 count);
            Count = count;
        }

        #endregion

        #endregion
    }
}
