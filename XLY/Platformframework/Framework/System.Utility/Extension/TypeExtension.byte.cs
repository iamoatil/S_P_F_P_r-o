using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 对byte类型的扩展
    /// </summary>
    public static partial class TypeExtension
    {
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

        #region ToBinary
        /// <summary>
        /// 转换为2进制字符串
        /// </summary>
        public static string ToBinary(this byte value)
        {
            return Convert.ToString(value, 2);
        }

        /// <summary>
        /// 转换为2进制字符串
        /// </summary>
        public static string ToBinary(this byte[] values, string separator = " ")
        {
            if (values == null || values.Length <= 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var b in values)
            {
                sb.Append(b.ToBinary()).Append(separator);
            }
            return sb.ToString();
        }

        #endregion

        #region To16Bland
        /// <summary>
        /// 转换为16进制字符串
        /// </summary>
        public static string To16Bland(this byte value)
        {
            return value.ToString("X2");
        }

        /// <summary>
        /// 转换为16进制字符串
        /// </summary>
        public static string To16Bland(this byte[] values, string separator = " ")
        {
            if (values == null || values.Length <= 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var b in values)
            {
                sb.Append(b.To16Bland()).Append(separator);
            }
            return sb.ToString();
        }
        #endregion


        #region Add By ShiXing 2017年2月27日13:47:02

        /// <summary>
        /// 从字节数组中的<see cref="Index"/>位置开始读取<see cref="length"/>个字节的数据。
        /// </summary>
        /// <param name="value">原始字节数组</param>
        /// <param name="index">开始位置</param>
        /// <param name="length">长度。</param>
        /// <returns>获取到的数据。</returns>
        public static byte[] GetBytesBlock(this byte[] value, int index, int length)
        {
            if (null == value || index < 0 || length <= 0)
                throw new ArgumentNullException("byte[] 获取块函数的参数错误");

            byte[] result = new byte[length];

            for (int i = index; i < index + length; i++)
            {
                if (i >= value.Length) break;
                result[i - index] = value[i];
            }

            return result;
        }

        /// <summary>
        /// 判断某个字节数组是否已某个数据开头。
        /// </summary>
        /// <param name="value">要判断的字节数组</param>
        /// <param name="startbyte">开始字节</param>
        /// <returns>是否匹配成功。</returns>
        public static bool StartWith(this byte[] value, byte[] startbyte)
        {
            if (null == value || startbyte == null)
                throw new ArgumentNullException("byte[] 是否以\"startbyte\"开始函数的参数错误");

            if (startbyte.Length > value.Length) return false;

            bool result = true;
            for (int i = 0; i < startbyte.Length; i++)
            {
                if (value[i] != startbyte[i])
                {
                    result = false;
                    break;
                }
            }

            return result;
        }


        /// <summary>
        /// 匹配指定byte在二进制数组中是第一次出现 Index。
        /// </summary>
        /// <param name="value">要判断的字节数组</param>
        /// <param name="startbyte">开始字节</param>
        /// <returns>是否匹配成功。</returns>
        public static int StartIndex(this byte[] value, byte startbyte)
        {
            if (null == value || startbyte == null)
                throw new ArgumentNullException("byte[] 是否以\"startbyte\"开始函数的参数错误");
            int result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == startbyte)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 将一个字节数组转换为十六进制字符串。
        /// </summary>
        /// <param name="value">要转换的字节数组</param>
        /// <returns>转换后的值。</returns>
        public static string ToHex(this byte[] value)
        {
            if (null == value) return "";
            StringBuilder sb = new StringBuilder();

            for (int i = 0, count = value.Length; i < count; i++)
            {
                sb.Append(value[i].ToString("x2"));
            }

            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// 将一个Byte数组变为指定长度。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] LenthTo(this byte[] source, int length)
        {
            byte[] result = new byte[length];
            if (source == null)
                return result;

            if (source.Length == length)
            {
                return source;
            }
            else if (source.Length > length)
            {
                return source.Take(length).ToArray();
            }
            else
            {
                source.CopyTo(result, 0);
                return result;
            }
        }
    }

}

