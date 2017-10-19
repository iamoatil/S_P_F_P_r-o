using System;
using XLY.SF.Framework.Core.Base.CoreInterface;

/* ==============================================================================
* Description：DefaultAsyncResult  
* Author     ：Fhjun
* Create Date：2017/4/11 11:26:45
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 默认的异步通知类
    /// </summary>
    public class DefaultAsyncProgress : IAsyncProgress
    {
        public double Progress { get; set; }
        public double Percent
        {
            get { return Total > 0 ? Progress * 100 / Total : 0; }
        }

        public double Total { get; set; }

        public string Message { get; set; }
        public double Completed { get; set; }

        public bool IsCompleted { get; set; }

        public AsyncProgressState State { get; set; }

        public object Data { get; set; }

        public void Start(double total)
        {
            State = AsyncProgressState.Running;
            Total = total;
            Progress = 0;
        }

        public void Advance(double step, string message = null, object sender = null)
        {
            if (State != AsyncProgressState.Running)
                return;
            Progress += step;
            Message = message;
            OnAdvance?.Invoke(step, message);
            if (Progress >= Total)
            {
                State = AsyncProgressState.Completed;
                OnCompleted?.Invoke(AsyncProgressCompleteStatus.Success, null);
            }
        }

        public void Stop(Exception ex = null)
        {
            State = AsyncProgressState.Completed;
            if (OnCompleted != null)
            {
                if (ex == null)
                    OnCompleted(AsyncProgressCompleteStatus.UserStoped, null);
                else
                    OnCompleted(AsyncProgressCompleteStatus.Execption, ex);
            }
        }

        public void AdvanceCompleted(double completed, string message = "")
        {
            Completed = completed;
            Message = message;

            if (OnAdvance != null && !IsCompleted)
            {
                OnAdvance(0, message);
            }

            if (Completed >= Total && !IsCompleted)
            {
                Completed = Total;
                IsCompleted = true;
                //完成时的事件通知
                OnCompleted?.Invoke(AsyncProgressCompleteStatus.Execption, null);
            }
        }

        public OnAdvance OnAdvance { get; set; }

        public OnCompleted OnCompleted { get; set; }

        public string Log { get; set; }

        public bool IsSuccess { get; set; }

    }
}
