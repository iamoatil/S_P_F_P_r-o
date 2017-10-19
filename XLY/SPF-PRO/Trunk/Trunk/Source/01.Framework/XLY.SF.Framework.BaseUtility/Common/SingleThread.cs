// ***********************************************************************
// Assembly:XLY.SF.Framework.BaseUtility
// Author:Songbing
// Created:2017-03-28 15:04:10
// Description:线程封装
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// 简单线程封装
    /// </summary>
    public sealed class SingleThread
    {
        /// <summary>
        /// 线程实例
        /// </summary>
        public Thread BackThread { get; private set; }

        /// <summary>
        /// 开始异步执行指定的方法体
        /// </summary>
        /// <param name="fun"></param>
        public void Start(Action fun)
        {
            BackThread = new Thread(new ThreadStart(fun));
            BackThread.IsBackground = true;
            BackThread.Start();
        }

        ///// <summary>
        ///// 暂停
        ///// </summary>
        //public void Pause()
        //{
        //    this.BackThread.Suspend();
        //}

        ///// <summary>
        ///// 继续
        ///// </summary>
        //public void Continue()
        //{
        //    if(BackThread.ThreadState == ThreadState.Suspended)
        //    {
        //        this.BackThread.Resume();
        //    }
        //}

        /// <summary>
        /// 结束正在执行的方法，并执行指定的回调函数
        /// </summary>
        /// <param name="callback"></param>
        public void Stop(Action callback = null)
        {
            if (BackThread == null || !BackThread.IsAlive) return;
            try
            {
                callback?.Invoke();

                if (BackThread.ThreadState == ThreadState.Suspended)
                {
                    BackThread.Resume();
                }

                BackThread.Abort();
            }
            catch (ThreadAbortException)
            {
                //do nothing
            }
            finally
            {
                BackThread.DisableComObjectEagerCleanup();
            }
        }

        /// <summary>
        /// 等待工作完成
        /// </summary>
        public void Wait(Action callback = null)
        {
            while (IsAlive)
            {
                Thread.Sleep(50);
            }

            callback?.Invoke();
        }

        /// <summary>
        /// 判断线程是否存活
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return BackThread != null && BackThread.IsAlive;
            }
        }

    }

}
