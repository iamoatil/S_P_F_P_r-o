using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.BaseUtility
{
    public static class DynamicConvert
    {
        #region ToSafeInt
        /// <summary>
        /// 转换为安全的Int
        /// </summary>
        public static int ToSafeInt(dynamic value)
        {
            return Int32.Parse(value == null ? "" : value.ToString());
        }
        #endregion

        #region ToSafeString
        /// <summary>
        /// 转换为安全的string
        /// </summary>
        public static string ToSafeString(dynamic value)
        {
            return value == null ? "" : value.ToString();
        }
        #endregion

        #region ToSafeBool
        /// <summary>
        /// 转换为安全的bool
        /// </summary>
        public static bool ToSafeBool(object value)
        {
            return value.ToSafeString().ToSafeBoolean();
        }
        #endregion

        #region ToSafeLong
        /// <summary>
        /// 转换为安全的long
        /// </summary>
        public static long ToSafeLong(object value)
        {
            return value.ToSafeString().ToSafeInt64();
        }
        #endregion

        #region ToSafeDecimal
        /// <summary>
        /// 转换为安全的Decimal
        /// </summary>
        public static decimal ToSafeDecimal(object value)
        {
            return value.ToSafeString().ToSafeDecimal();
        }
        #endregion

        #region ToSafeDouble
        /// <summary>
        /// 转换为安全的Double
        /// </summary>
        public static Double ToSafeDouble(object value)
        {
            return value.ToSafeString().ToSafeDouble();
        }
        #endregion

        #region ToSafeInt16
        /// <summary>
        /// 转换为安全的Int16
        /// </summary>
        public static short ToSafeInt16(object value)
        {
            return value.ToSafeString().ToSafeInt16();
        }
        #endregion

        #region ToEnumByValue
        /// <summary>
        /// 转换为枚举对象
        /// </summary>
        public static T ToEnumByValue<T>(object value, T defaultvalue)
        {
            if (value == null) return defaultvalue;
            return value.ToSafeString().ToEnum<T>();
        }
        #endregion

        #region ToSafeDateTime

        /// <summary>
        /// 字符串转时间格式
        /// </summary>
        /// <param name="pDatetime"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(object pDatetime)
        {
            try
            {
                DateTime parseDateTime;
                DateTime.TryParse(pDatetime.ToSafeString(), out parseDateTime);
                return parseDateTime;
            }
            catch
            {

            }
            return null;
        }

        /// <summary>
        /// NSDate转系统时间
        /// </summary>
        /// <param name="nsDateSeconds">9位数值，可带小数</param>
        /// <returns></returns>
        public static DateTime? ToNsDateToSystemTime(object value, int startYear = 2000)
        {
            try
            {
                double nsDate = Convert.ToDouble(value);
                long ticks = (long)(nsDate * TimeSpan.TicksPerSecond);
                return DateTime.FromBinary(ticks).AddYears(startYear);
            }
            catch
            {

            }
            return null;
        }
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

        /// <summary>
        /// 转换Google Chrome的时间戳为安全的DateTime，如果转换错误，则会返回DateTime.MaxValue
        /// </summary>
        public static DateTime? ToSafeDateTimeForGoogleChrome(object value, int startYear = 1601)
        {
            var len = value.ToSafeString().ToSafeInt64();

            if (len <= 0)
            {
                return null;
            }

            //如果是17位为毫秒 10000
            //如果是14为秒，10000000
            //DateTime.Now.Ticks 是指从DateTime.MinValue之后过了多少时间，10000000为一秒
            int strlen = len.ToString().Length;

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(startYear, 1, 1));

            try
            {
                TimeSpan toNow = strlen == 17 ? TimeSpan.FromMilliseconds(len / 1000) : TimeSpan.FromSeconds(len);
                var dt = dtStart.Add(toNow);
                return dt;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 转换java的时间戳为安全的DateTime，如果转换错误，则会返回DateTime.MaxValue
        /// </summary>
        public static DateTime? ToSafeDateTimeForJava(object value, int startYear = 1970)
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

            //DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0);
            if (strlen == 13)
            {
                len *= 10000;
            }
            else if (strlen <= 10)
            {
                len *= 10000000;
            }
            try
            {
                long tricks_1970 = dtStart.Ticks;
                long time_tricks = tricks_1970 + len;
                var dt = new DateTime(time_tricks).AddHours(8);
                return dt;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 转换UTC的时间戳为安全的DateTime，如果转换错误，则会返回DateTime.MaxValue
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static DateTime? ToSafeDateTimeForUTC(object value, int startYear = 1970)
        {
            var len = value.ToSafeString().ToSafeInt64();

            if (len <= 0)
            {
                return null;
            }


            //如果是17位 则 需要UTC 乘10 转换
            //如果是13位为毫秒 10000
            //如果是10为秒，10000000
            //DateTime.Now.Ticks 是指从DateTime.MinValue之后过了多少时间，10000000为一秒
            int strlen = len.ToString().Length;

            if (strlen == 17)
            {
                // Windows file time UTC is in nanoseconds, so multiplying by 10
                DateTime gmtTime = DateTime.FromFileTimeUtc(10 * len);

                // Converting to local time
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);

                return localTime;
            }

            if (strlen == 18)
            {

                DateTime localTime = DateTime.FromFileTime(len).AddHours(-8);

                return localTime;
            }

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(startYear, 1, 1));
            if (strlen == 13)
            {
                len *= 10000;
            }
            else if (strlen <= 10)
            {
                len *= 10000000;
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

        /// <summary>
        /// 转换UTC的时间戳为安全的DateTime，如果转换错误，则会返回DateTime.MaxValue
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static DateTime? ToSafeDateTimeForUTCNoTimeZone(object value, int startYear = 1970)
        {
            var len = value.ToSafeString().ToSafeInt64();

            if (len <= 0)
            {
                return null;
            }


            //如果是17位 则 需要UTC 乘10 转换
            //如果是13位为毫秒 10000
            //如果是10为秒，10000000
            //DateTime.Now.Ticks 是指从DateTime.MinValue之后过了多少时间，10000000为一秒
            int strlen = len.ToString().Length;

            if (strlen == 17)
            {
                // Windows file time UTC is in nanoseconds, so multiplying by 10
                DateTime gmtTime = DateTime.FromFileTimeUtc(10 * len);

                // Converting to local time
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(gmtTime, TimeZoneInfo.Local);

                return localTime;
            }

            if (strlen == 18)
            {

                DateTime localTime = DateTime.FromFileTime(len).AddHours(-8);

                return localTime;
            }

            var dtStart = new DateTime(startYear, 1, 1);

            if (strlen == 13)
            {
                len *= 10000;
            }
            else if (strlen <= 10)
            {
                len *= 10000000;
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

        #endregion

        #region 16位的日期
        /// <summary>
        /// 解析16位的日期
        /// </summary>
        /// <param name="value">16位数字</param>
        /// <returns>日期</returns>
        public static DateTime? ToSafeFromUnixTime(object value, int length = 1000000)
        {
            var len = value.ToSafeString().ToSafeInt64();
            if (len <= 0)
            {
                return null;
            }

            var unixTime = len / length;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime).AddHours(8);
        }
        #endregion

    }
}
