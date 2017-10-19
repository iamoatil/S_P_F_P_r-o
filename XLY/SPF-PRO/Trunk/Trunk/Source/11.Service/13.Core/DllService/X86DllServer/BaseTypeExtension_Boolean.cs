/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 13:44:15
 * 类功能说明：Bool类型扩展
 *
 *************************************************/

namespace X86DllServer.Service
{
    public static partial class BaseTypeExtension
    {
        #region String:ToBoolean

        /// <summary>
        /// 转换为安全的boolean类型
        /// </summary>
        public static bool ToSafeBoolean(this string value, bool defaultValue = false)
        {
            bool result;
            if (bool.TryParse(value, out result))
            {
                return result;
            }
            return defaultValue;
        }

        #endregion
    }
}
