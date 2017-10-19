using System;
using System.Globalization;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 16:34:20
 * 类功能说明：
 *
 *************************************************/

namespace X86DllServer.Service
{
    public static partial class BaseTypeExtension
    {
        #region ToDateTime：将字符串按照格式转换为日期时间对象
        /// <summary>
        /// 将字符串按照格式和当前时区转换为日期时间对象，若转换失败返回默认时间
        /// </summary>
        public static DateTime ToDateTime(this string value, string format)
        {
            DateTime result;
            if (DateTime.TryParseExact(value, format, CultureInfo.CurrentCulture,
                System.Globalization.DateTimeStyles.None, out result))
            {
                return result;
            }
            else
            {
                return default(DateTime);
            }
        }
        #endregion

        #region ToDateTime：转换为日期时间
        /// <summary>
        /// 转换为日期时间
        /// </summary>
        public static DateTime ToDateTime(this string dateTime)
        {
            DateTime result;
            if (DateTime.TryParse(dateTime, out result))
            {
                return result;
            }
            return default(DateTime);
        }
        #endregion

        #region ToSafeDateTime：安全的转换为日期时间
        /// <summary>
        /// 转换为日期时间
        /// </summary>
        public static DateTime? ToSafeDateTime(this string dateTime)
        {
            DateTime result;
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"^\d+$");
            if (re.IsMatch(dateTime) && dateTime.Length == 18)
            {
                // 如果为18位的文件时间，那么在这里处理
                var tempDate = System.DateTime.FromFileTime(Convert.ToInt64(dateTime));
                return tempDate;
            }
            else if (DateTime.TryParse(dateTime, out result))
            {
                return result;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 转换Linux的时间戳为安全的DateTime，如果转换错误，则会返回DateTime.MaxValue
        /// </summary>
        public static DateTime? ToSafeDateTime(object value, int startYear = 1970)
        {
            var len = value.ToSafeString().ToSafeInt64();

            if (len <= 0)
            {
                return null;
            }

            //如果是13位为毫秒 10000
            //如果是10为秒，10000000
            //DateTime.Now.Ticks 是指从DateTime.MinValue之后过了多少时间，10000000为一秒
            int strlen = len.ToString().Length;


            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(startYear, 1, 1));
            if (strlen == 13)
            {
                len *= 10000;
            }
            else if (strlen <= 10)
            {
                len *= 10000000;
            }
            else if (strlen >= 19)
            {
                len /= 1000;
            }

            try
            {
                TimeSpan toNow = new TimeSpan(len);
                var dt = dtStart.Add(toNow);
                return dt;
            }
            catch
            {
                return null;
            }
        }

        #region IsValid：是否为有效日期
        /// <summary>
        /// 是否为有效日期（此方法没什么用）
        /// </summary>
        public static bool IsValid(this DateTime value)
        {
            return value != default(DateTime);
        }
        #endregion


        #region IsValid：是否为有效日期
        /// <summary>
        /// 是否为有效日期（此方法没什么用）
        /// </summary>
        public static bool IsValid(this DateTime? value)
        {
            return value.HasValue && value.Value.IsValid();
        }
        #endregion

        /// <summary>
        /// 转换为日期字符串，格式：yyyy-MM-dd。
        /// 如果值非法（null,min,max）返回emtpy
        /// </summary>
        public static string ToDateString(this DateTime value, string format = "yyyy-MM-dd")
        {
            if (value.IsValid() && !string.IsNullOrEmpty(format))
                return value.ToString(format);
            return string.Empty;
        }

        /// <summary>
        /// 转换为日期字符串，默认格式：yyyy-MM-dd。
        /// 如果值非法（null,min,max）返回emtpy
        /// </summary>
        public static string ToDateString(this DateTime? value, string format = "yyyy-MM-dd")
        {
            if (value.IsValid() && !string.IsNullOrEmpty(format))
                return value.Value.ToString(format);
            return string.Empty;
        }
        
        /// <summary>
        /// 转换为日期时间字符串，格式：yyyy-MM-dd HH:mm:ss，若时分秒为0则只返回日期
        /// 如果值非法（null,min,max）返回emtpy
        /// </summary>
        public static string ToDateTimeString(this DateTime value)
        {
            if (!value.IsValid())
            {
                return string.Empty;
            }
            else if (value.Year == 1970 && value.Month == 1 && value.Day == 1 && value.Hour == 0 && value.Minute == 0 && value.Second == 0)
            {
                return string.Empty;
            }
            else if (value.Hour == 0 && value.Minute == 0 && value.Second == 0)
            {
                return value.ToDateString();
            }
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #region ToDateTimeString
        /// <summary>
        /// 转换为日期字符串，格式：yyyy-MM-dd HH:mm:ss
        /// 如果值非法（null,min,max）返回emtpy
        /// </summary>
        public static string ToDateTimeString(this DateTime? value)
        {
            if (value.IsValid())
            {
                return value.Value.ToDateTimeString();
            }
            return string.Empty;
        }
        #endregion
    }
}
