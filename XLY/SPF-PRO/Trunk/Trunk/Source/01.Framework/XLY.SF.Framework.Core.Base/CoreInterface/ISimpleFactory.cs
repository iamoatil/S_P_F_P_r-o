// ***********************************************************************
// Assembly:XLY.SF.Framework.Core.CommonInterfaces.Task
// Author:Songbing
// Created:2017-03-27 11:08:23
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

namespace XLY.SF.Framework.Core.Base.CoreInterface
{
    /// <summary>
    /// 简单工厂的抽象接口
    /// </summary>
    /// <typeparam name="TKey">key键值类型</typeparam>
    /// <typeparam name="TValue">工厂实例类型</typeparam>
    public interface ISimpleFactory<TKey, TValue>
    {
        /// <summary>
        /// 根据key获取实例
        /// </summary>
        TValue GetInstance(TKey key);
    }
}
