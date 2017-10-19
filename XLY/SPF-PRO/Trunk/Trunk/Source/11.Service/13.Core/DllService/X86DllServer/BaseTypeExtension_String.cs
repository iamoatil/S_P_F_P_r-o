using System;
using System.ComponentModel;
using System.Text;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 16:03:35
 * 类功能说明：
 *
 *************************************************/

namespace X86DllServer.Service
{
    public static partial class BaseTypeExtension
    {
        #region ToSafeString
        /// <summary>
        /// 安全的字符转换
        /// </summary>
        public static string ToSafeString(this object obj)
        {
            return obj == null ? "" : obj.ToString();
        }
        #endregion

        #region ToBytes：把字符转换为指定编码的序列
        /// <summary>
        /// 把字符转换为指定编码的序列
        /// </summary>
        /// <param name="value">字符值</param>
        /// <param name="encode">编码</param>
        /// <returns>序列</returns>
        public static byte[] ToBytes(this string value, Encoding encode)
        {
            if (!string.IsNullOrEmpty(value))
                return encode.GetBytes(value);
            return new byte[0];
        }
        #endregion

        #region Encode：把字符转换为制定的编码格式的字符
        /// <summary>
        /// 从Unicode转换为指定的编码格式
        /// </summary>
        public static string Encode(this string value, System.Text.Encoding encode)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value.Encode(Encoding.Unicode, encode);
            }
            return string.Empty;
        }
        #endregion

        #region Encode：把字符转换为制定的编码格式的字符
        /// <summary>
        /// 把字符转换为制定的编码格式的字符
        /// </summary>
        public static string Encode(this string value, System.Text.Encoding sourceEncode, System.Text.Encoding targetEncode)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var bytes = System.Text.Encoding.Convert(sourceEncode, targetEncode, value.ToBytes(sourceEncode));
                return bytes.GetString(targetEncode);
            }
            return string.Empty;
        }
        #endregion

        #region ToSafeEnum
        /// <summary>
        /// 将枚举常数的名称或数字值的字符串表示安全的转换成等效的枚举对象。
        /// </summary>
        public static T ToSafeEnum<T>(this string value)
            where T : struct
        {
            T result = default(T);
            var tType = typeof(T);
            if (tType.IsEnum)
            {
                Enum.TryParse(value, out result);
            }
            return result;
        }
        #endregion

        #region ToSafeEnum
        /// <summary>
        /// 将枚举常数的名称或数字值的字符串表示安全的转换成等效的枚举对象。
        /// </summary>
        public static T ToSafeEnum<T>(this string value, T defaultValue)
        {
            try
            {
                return value.ToEnum<T>();
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 将一个或多个枚举常数的名称或数字值的字符串表示转换成等效的枚举对象。
        /// </summary>
        public static T ToEnum<T>(this string name)
        {
            Type type = typeof(T);
            if (type.IsEnum)
            {
                return (T)Enum.Parse(typeof(T), name, true);
            }
            throw new InvalidCastException("必须是枚举类型才能转换。");
        }
        #endregion

        public static string ConnectLinuxPath(string path1, string path2)
        {
            return ConnectPath('/', path1, path2);
        }

        public static string ConnectPath(char separate, params string[] path)
        {
            if (path == null)
                return string.Empty;
            if (path.Length == 2)
                return string.Format("{0}{1}{2}", path[0].TrimEnd(separate), separate, path[1].TrimStart(separate));
            if (path.Length == 1)
                return path[0];
            StringBuilder sb = new StringBuilder(32);
            foreach (var p in path)
            {
                sb.Append(p.TrimEnd(separate).TrimStart(separate)).Append(separate);
            }
            return sb.ToString().TrimEnd(separate);
        }
    }
}
