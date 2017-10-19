using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Utility
{
    /// <summary>
    /// 时间转换工具类
    /// NSDate: 2001-01-01 00:00:00作为基准时间，到目标时间的秒数
    /// SystemTime: 1970-01-01 00:00:00作为基准时间，到目标时间的秒数
    /// FileTime: 1601-01-01 00:00:00作为基准时间
    /// UnixTimeStamp: 1970-01-01 00:00:00作为基准时间，到目标时间的秒数 
    /// </summary>
    public class TimeConvertHelper
    {
        /// <summary>
        /// NSDate转系统时间
        /// </summary>
        /// <param name="nsDateSeconds">9位数值，可带小数</param>
        /// <returns></returns>
        public static DateTime NsDateToSystemTime(double nsDateSeconds)
        {
            long ticks = (long)(nsDateSeconds * TimeSpan.TicksPerSecond);
            return DateTime.FromBinary(ticks).AddYears(2000);
        }

        /// <summary>
        /// UNIX时间戳转系统时间
        /// </summary>
        /// <param name="seconds">10位数值</param>
        /// <returns></returns>
        public static DateTime UnixTimestampToSystemTimeUtc(double seconds)
        {
            return DateTime.FromBinary((long)(seconds * TimeSpan.TicksPerSecond)).AddYears(1969).AddDays(-1);
        }

        public static DateTime UnixTimestampToSystemTimeUtc(long seconds)
        {
            return DateTime.FromBinary(seconds * TimeSpan.TicksPerSecond + 621355968000000000);
        }

        /// <summary>
        /// FileTime转系统时间
        /// </summary>
        /// <param name="ticks">16位数值</param>
        /// <returns></returns>
        public static DateTime FileTimeToSystemTime(long ticks)
        {
            return DateTime.FromBinary(ticks).AddYears(1600);
        }

        /// <summary>
        /// FileTime转系统时间
        /// </summary>
        /// <param name="hiDateTime">高位时间</param>
        /// <param name="lowDateTime">低位时间</param>
        /// <returns></returns>
        public static DateTime FileTimeToSystemTime(uint hiDateTime, uint lowDateTime)
        {
            return DateTime.FromFileTimeUtc(((long)hiDateTime) << 32 | lowDateTime);
        }

        /// <summary>
        /// 字符串转时间格式
        /// </summary>
        /// <param name="pDatetime"></param>
        /// <returns></returns>
        public static DateTime? CovertSafeDatetime(object pDatetime)
        {
            try
            {
                return Convert.ToDateTime(pDatetime);
            }
            catch
            {

            }
            return null;
        }
    }
}
