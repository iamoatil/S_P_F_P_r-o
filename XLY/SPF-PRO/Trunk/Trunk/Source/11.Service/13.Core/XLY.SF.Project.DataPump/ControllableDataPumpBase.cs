using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 进度可控制的数据泵基类。
    /// </summary>
    public abstract class ControllableDataPumpBase : DataPumpBase
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.ControllableDataPumpBase 实例。
        /// </summary>
        protected ControllableDataPumpBase()
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
        public override DataPumpExecutionContext CreateContext(Pump metadata, String targetDirectory, SourceFileItem source)
        {
            return new DataPumpControllableExecutionContext(metadata, targetDirectory, source) { Owner = this };
        }

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected sealed override void ExecuteCore(DataPumpExecutionContext context)
        {
            DataPumpControllableExecutionContext contextEx = context as DataPumpControllableExecutionContext;
            if (contextEx == null) throw new InvalidOperationException("context is not DataPumpControllableExecutionContext");
            if (contextEx.Status == AsyncProgressState.Running || contextEx.Status == AsyncProgressState.Starting) return;
            ExecuteCore(contextEx);
        }

        /// <summary>
        /// 初始化当前的执行流程。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected sealed override Boolean InitExecution(DataPumpExecutionContext context)
        {
            return InitExecution((DataPumpControllableExecutionContext)context);
        }

        /// <summary>
        /// 取消执行。
        /// </summary>
        public virtual void Cancel(DataPumpControllableExecutionContext context)
        {
            if (context.Owner != this) return;
            if ((context.Status == AsyncProgressState.Starting) ||
                (context.Status == AsyncProgressState.Running))
            {
                context.Reporter.Stop();
                context.CancellationTokenSource.Cancel();
                context.Reset();
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected abstract void ExecuteCore(DataPumpControllableExecutionContext context);

        /// <summary>
        /// 初始化当前的执行流程。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        protected virtual Boolean InitExecution(DataPumpControllableExecutionContext context)
        {
            return true;
        }

        #endregion

        #endregion
    }
}
