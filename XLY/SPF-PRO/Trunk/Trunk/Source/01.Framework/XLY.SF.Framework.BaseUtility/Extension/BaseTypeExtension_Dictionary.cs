using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 17:22:46
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.BaseUtility
{
    public static partial class BaseTypeExtension
    {
        #region GetSafeValue:从字典中安全获取键Key的值，若Key不存在，则返回默认值

        /// <summary>
        /// 从字典中安全获取键Key的值，若Key不存在，则返回默认值。
        /// </summary>
        /// <param name="dictionary">字段。</param>
        /// <param name="key">key</param>
        /// <param name="defalut">默认值。</param>
        /// <returns>返回value</returns>
        public static TValue GetSafeValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defalut = default(TValue))
        {
            if (dictionary == null || dictionary.Count < 1)
            {
                return defalut;
            }

            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return defalut;
        }

        #endregion
    }
}
