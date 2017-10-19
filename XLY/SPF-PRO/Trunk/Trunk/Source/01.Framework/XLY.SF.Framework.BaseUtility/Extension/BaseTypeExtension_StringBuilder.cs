using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/15 17:55:24
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.BaseUtility
{
    public static partial class BaseTypeExtension
    {
        #region AppendKeyValueNewLine
        /// <summary>
        /// 添加键值数据及换行符
        /// </summary>
        public static void AppendKeyValueNewLine(this StringBuilder sb, string key, object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString())) return;
            sb.Append(key).Append("：").Append(value).Append(Environment.NewLine);
        }

        /// <summary>
        /// 添加键值数据及换行符
        /// </summary>
        public static void AppendKeyValueNewLine(this StringBuilder sb, string key, Enum value)
        {
            sb.Append(key).Append("：").Append(value.GetDescription()).Append(Environment.NewLine);
        }

        #endregion

        #region AppendKeyValue
        /// <summary>
        /// 添加键值数据
        /// </summary>
        public static void AppendKeyValue(this StringBuilder builder, string key, object value, string end = " ")
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString())) return;

            builder.AppendFormat("{0}：{1}{2}", key, value, end);
        }

        /// <summary>
        /// 添加键值数据
        /// </summary>
        public static void AppendKeyValue(this StringBuilder sb, string key, Enum value, string end = " ")
        {
            sb.Append(key).Append("：").Append(value.GetDescription()).Append(end);
        }

        #endregion
    }
}
