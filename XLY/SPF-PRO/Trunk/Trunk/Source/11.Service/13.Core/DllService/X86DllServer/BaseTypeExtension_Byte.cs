using System;
using System.Linq;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 13:47:55
 * 类功能说明：
 *
 *************************************************/

namespace X86DllServer.Service
{
    public static partial class BaseTypeExtension
    {
        #region GetString： 把byte数组转换为系统默认编码(System.Text.Encoding.Default)类型的字符串
        /// <summary>
        /// 把byte数组转换为系统默认编码(System.Text.Encoding.Default)类型的字符串
        /// </summary>
        /// <param name="bytes">值</param>
        /// <returns></returns>
        public static string GetString(this byte[] bytes)
        {
            return bytes.GetString(System.Text.Encoding.Default);
        }
        #endregion

        #region GetString： 把byte数组转换为制定编码类型的字符串
        /// <summary>
        /// 把byte数组转换为制定编码类型的字符串
        /// </summary>
        /// <param name="bytes">值</param>
        /// <param name="encode">编码类型</param>
        /// <returns></returns>
        public static string GetString(this byte[] bytes, System.Text.Encoding encode)
        {
            if (bytes == null || bytes.Count() <= 0)
            {
                return string.Empty;
            }
            return encode.GetString(bytes);
        }
        #endregion
        
        #region 2进制

        /// <summary>
        /// 转换为2进制字符串
        /// </summary>
        public static string ToBinary(this byte value)
        {
            return Convert.ToString(value, 2);
        }

        #endregion
        
        #region 16进制

        /// <summary>
        /// 转换为16进制字符串（两位）
        /// </summary>
        public static string ToHex(this byte value)
        {
            return value.ToString("X2");
        }

        #endregion
    }
}
