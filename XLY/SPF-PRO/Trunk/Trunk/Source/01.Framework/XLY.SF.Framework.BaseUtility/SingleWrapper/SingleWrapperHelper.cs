using System;

/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 10:55:47
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// 单例包装器
    /// </summary>
    public class SingleWrapperHelper<TSingleWrapper>
        where TSingleWrapper : class
    {
        #region 单例定义

        private static TSingleWrapper _instance;
        private static object _objLock = new object();

        #endregion

        #region 实例

        public static TSingleWrapper GetInstance()
        {
            return Instance;
        }

        /// <summary>
        /// 类型实例（私有构造函数）
        /// </summary>
        public static TSingleWrapper Instance
        {
            get
            {
                if (_instance == null)
                    lock (_objLock)
                        if (_instance == null)
                        {
                            _instance = GetInstanceOfPublic();
                            if (_instance == null)
                                _instance = GetInstanceOfPrivate();
                            if (_instance == null)
                                throw new NotSupportedException("未发现公有或私有无参构造函数");
                        }
                return _instance;
            }
        }

        #endregion

        #region 获取单一实例方法

        /// <summary>
        /// 根据公有构造函数获取实例
        /// </summary>
        /// <returns></returns>
        private static TSingleWrapper GetInstanceOfPublic()
        {
            TSingleWrapper instance = default(TSingleWrapper);
            Type type = typeof(TSingleWrapper);
            var createMothod = type.GetConstructor(new Type[0]);
            if (createMothod != null)
                instance = createMothod.Invoke(null) as TSingleWrapper;
            return instance;
        }

        /// <summary>
        /// 根据私有构造函数获取实例
        /// </summary>
        /// <returns></returns>
        private static TSingleWrapper GetInstanceOfPrivate()
        {
            TSingleWrapper instance = default(TSingleWrapper);
            Type type = typeof(TSingleWrapper);
            System.Reflection.ConstructorInfo[] constructorInfoArray = type.GetConstructors(System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            System.Reflection.ConstructorInfo noParameterConstructorInfo = null;
            foreach (System.Reflection.ConstructorInfo constructorInfo in constructorInfoArray)
            {
                System.Reflection.ParameterInfo[] parameterInfoArray = constructorInfo.GetParameters();
                if (0 == parameterInfoArray.Length)
                {
                    noParameterConstructorInfo = constructorInfo;
                    break;
                }
            }
            if (noParameterConstructorInfo != null)
                instance = (TSingleWrapper)noParameterConstructorInfo.Invoke(null);
            return instance;
        }

        #endregion
    }
}
