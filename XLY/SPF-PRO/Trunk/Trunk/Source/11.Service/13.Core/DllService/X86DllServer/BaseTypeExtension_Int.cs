using System;
using System.Globalization;

namespace X86DllServer.Service
{
    public static partial class BaseTypeExtension
    {
        #region ToSafeInt
        /// <summary>
        /// 转换为安全的Int32类型，默认为0
        /// </summary>
        public static int ToSafeInt(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return 0;
            }

            if (value.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
            {
                return (Int32)value.ToSafeDouble();
            }
            Int32 result;
            if (Int32.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// 转换为长整形数字
        /// </summary>
        public static Int64 ToSafeInt64(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            if (value.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
            {
                return (Int64)value.ToSafeDouble();
            }
            Int64 result;
            if (Int64.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
    }
}
