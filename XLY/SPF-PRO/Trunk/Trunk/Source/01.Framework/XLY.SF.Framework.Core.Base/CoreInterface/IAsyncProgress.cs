using System;
using System.Runtime.CompilerServices;

/* ==============================================================================
* Description：IAsyncResult  
* Author     ：Fhjun
* Create Date：2017/4/11 11:16:14
* ==============================================================================*/

namespace XLY.SF.Framework.Core.Base.CoreInterface
{
    /// <summary>
    /// 异步通知类
    /// </summary>
    public interface IAsyncProgress
    {
        /// <summary>
        /// 执行进度
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        void AdvanceCompleted(double completed, string message = "");
        /// <summary>
        /// 记录日志
        /// </summary>
        string Log { get; set; }
        /// <summary>
        /// 是否执行成功，默认执行成功
        /// </summary>
        bool IsSuccess { get; set; }
        /// <summary>
        /// 当前进度
        /// </summary>
        double Progress { get; set; }
        /// <summary>
        /// 当前进度的百分比值
        /// </summary>
        double Percent { get;}
        /// <summary>
        /// 进度总数
        /// </summary>
        double Total { get; set; }
        /// <summary>
        /// 当前进度消息
        /// </summary>
        string Message { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        AsyncProgressState State { get; set; }
        /// <summary>
        /// 附加的数据
        /// </summary>
        object Data { get; set; }
        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="total"></param>
        void Start(double total);
        /// <summary>
        /// 报告进度变化
        /// </summary>
        /// <param name="step"></param>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        void Advance(double step, string message = null, object sender = null);
        /// <summary>
        /// 结束进度
        /// </summary>
        /// <param name="ex"></param>
        void Stop(Exception ex = null);

        /// <summary>
        /// 进度改变事件
        /// </summary>
        OnAdvance OnAdvance { get; set; }
        /// <summary>
        /// 进度完成事件
        /// </summary>
        OnCompleted OnCompleted { get; set; }
    }

    public delegate void OnAdvance(double step, string message);
    public delegate void OnCompleted(AsyncProgressCompleteStatus status, object arg);

    /// <summary>
    /// 异步通知状态
    /// </summary>
    public enum AsyncProgressState
    {
        /// <summary>
        /// 空闲，未开始
        /// </summary>
        Idle,
        /// <summary>
        /// 正在准备开始中
        /// </summary>
        Starting,
        /// <summary>
        /// 正在运行
        /// </summary>
        Running,
        /// <summary>
        /// 正在停止
        /// </summary>
        Stopping,
        /// <summary>
        /// 执行完成
        /// </summary>
        Completed,
    }

    /// <summary>
    /// 异步通知结束状态
    /// </summary>
    public enum AsyncProgressCompleteStatus
    {
        /// <summary>
        /// 正常执行结束
        /// </summary>
        Success,
        /// <summary>
        /// 用户手动停止
        /// </summary>
        UserStoped,
        /// <summary>
        /// 执行中发生异常
        /// </summary>
        Execption,
    }
}
