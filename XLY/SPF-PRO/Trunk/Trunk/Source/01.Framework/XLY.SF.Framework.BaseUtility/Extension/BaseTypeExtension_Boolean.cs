using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 13:44:15
 * 类功能说明：Bool类型扩展
 *
 *************************************************/

namespace XLY.SF.Framework.BaseUtility
{
    public static partial class BaseTypeExtension
    {
        #region String:ToBoolean
        /// <summary>
        /// 转换为boolean类型
        /// </summary>
        public static bool ToBoolean(this string value)
        {
            bool result;
            if (Boolean.TryParse(value, out result))
            {
                return result;
            }
            throw new InvalidCastException("\"" + value + "\"不是有效的boolean，请确认。");
        }
        #endregion

        #region String:ToBoolean
        /// <summary>
        /// 转换为安全的boolean类型
        /// </summary>
        public static bool ToSafeBoolean(this string value, bool defaultValue = false)
        {
            bool result;
            if (Boolean.TryParse(value, out result))
            {
                return result;
            }
            return defaultValue;
        }
        #endregion

        #region ToCNString

        /// <summary>
        /// 转换为中文
        /// </summary>
        public static string ToCNString(this Boolean value)
        {
            return value ? "是" : "否";
        }

        /// <summary>
        /// 转换为中文
        /// </summary>
        public static string ToCNString(this bool? value)
        {
            return value.HasValue ? value.ToString() : string.Empty;
        }
        #endregion
    }
}
