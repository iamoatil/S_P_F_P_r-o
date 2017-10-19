using System.Collections;
using System.Collections.Generic;
using System.Linq;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 15:58:54
 * 类功能说明：
 *
 *************************************************/

namespace X86DllServer.Service
{
    public static partial class BaseTypeExtension
    {
        #region IsValid：验证集合是否有效

        /// <summary>
        /// 验证集合是否有效(是否为空，是否包含元素，有效返回true)
        /// 注意：判断是否包含元素，尽量使用Enumerable.Any，而不要用Linq中的Count
        /// </summary>
        public static bool IsValid<T>(this IEnumerable<T> source)
        {
            return source != null && source.Any();
        }

        /// <summary>
        /// 验证集合是否有效(是否为空，是否包含元素，有效返回true)
        /// </summary>
        public static bool IsValid(this IEnumerable @this)
        {
            if (@this == null) return false;
            foreach (var item in @this)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region IsValid：验证集合是否有效

        /// <summary>
        /// 验证集合是否有效(是否为空，是否包含元素，有效返回true)
        /// 注意：判断是否包含元素，尽量使用Enumerable.Any，而不要用Linq中的Count
        /// </summary>
        public static bool IsInvalid<T>(this IEnumerable<T> source)
        {
            return !source.IsValid();
        }

        /// <summary>
        /// 验证集合是否有效(是否为空，是否包含元素，有效返回true)
        /// </summary>
        public static bool IsInvalid(this IEnumerable @this)
        {
            return !@this.IsValid();
        }

        #endregion

    }
}
