using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XLY.SF.Project.Models.Logical
{
    /// <summary>
    /// 业务模型泛型基类。
    /// </summary>
    /// <typeparam name="TEntity">业务模型对应的实体模型类型。</typeparam>
    public class LogicalModelBase<TEntity> : LogicalModelBase
        where TEntity : class,new()
    {

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.Models.Logical.LogicalModelBase 实例。
        /// </summary>
        /// <param name="entity">实体模型。</param>
        protected LogicalModelBase(TEntity entity)
            :base(entity)
        {
        }

        /// <summary>
        /// 初始化类型 XLY.SF.Project.Models.Logical.LogicalModelBase 实例。
        /// </summary>
        protected LogicalModelBase()
            : this(new TEntity())
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 对应的实体模型的实例。
        /// </summary>
        public new TEntity Entity => (TEntity)base.Entity;

        #endregion
    }

    /// <summary>
    /// 业务模型非泛型基类。
    /// </summary>
    /// <typeparam name="TEntity">业务模型对应的实体模型类型。</typeparam>
    public class LogicalModelBase : INotifyPropertyChanged
    {
        #region Events

        #region PropertyChanged

        /// <summary>
        /// 属性值改变事件。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 触发属性值改变事件。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        protected void OnPropertyChanged([CallerMemberName]String propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.Models.Logical.LogicalModelBase 实例。
        /// </summary>
        /// <param name="entity">实体模型。</param>
        protected LogicalModelBase(Object entity)
        {
            Entity = entity ?? throw new ArgumentNullException("entity");
        }

        #endregion

        #region Properties

        /// <summary>
        /// 对应的实体模型的实例。
        /// </summary>
        public Object Entity { get; internal set; }

        #endregion
    }
}
