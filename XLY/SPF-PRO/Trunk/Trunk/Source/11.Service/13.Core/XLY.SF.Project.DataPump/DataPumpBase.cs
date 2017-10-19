using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 数据泵基类。
    /// </summary>
    public abstract class DataPumpBase
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.DataPumpServiceBase 实例。
        /// </summary>
        protected DataPumpBase()
        {
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 创建执行数据泵时所需的上下文对象。
        /// </summary>
        /// <param name="metadata">元数据。</param>
        /// <param name="targetDirectory">数据保存目录。</param>
        /// <param name="source">数据源。</param>
        /// <returns>执行上下文。</returns>
        public virtual DataPumpExecutionContext CreateContext(Pump metadata, String targetDirectory, SourceFileItem source)
        {
            return new DataPumpExecutionContext(metadata, targetDirectory, source) { Owner = this };
        }

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        public void Execute(DataPumpExecutionContext context)
        {
            if (context.IsInit)
            {
                ExecuteCore(context);
            }
            else if (InitExecution(context))
            {
                context.IsInit = true;
                ExecuteCore(context);
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected abstract void ExecuteCore(DataPumpExecutionContext context);

        /// <summary>
        /// 初始化当前的执行流程。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected virtual Boolean InitExecution(DataPumpExecutionContext context)
        {
            return true;
        }

        /// <summary>
        /// 设置上下文的自定义数据。
        /// </summary>
        /// <param name="name">数据名称。</param>
        /// <param name="value">数据值。</param>
        /// <param name="context">上下文。</param>
        protected void SetContextData(DataPumpExecutionContext context, String name, Object value)
        {
            context[name] = value;
        }

        #endregion

        #endregion
    }
}
