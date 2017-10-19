using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Utility.Interface
{
    /// <summary>
    /// 执行进度接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProgress<in T>
    {
        void Report(T value);
    }
}
