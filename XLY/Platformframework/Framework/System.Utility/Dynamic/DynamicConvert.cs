using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace System
{
    /// <summary>
    /// 动态类型的数据类型安全转换
    /// </summary>
    public static class DynamicConvert
    {
        #region ToSafeInt
        /// <summary>
        /// 转换为安全的Int
        /// </summary>
        public static int ToSafeInt(object value)
        {
            return value.ToSafeString().ToSafeInt();
        }
        #endregion

        #region ToSafeString
        /// <summary>
        /// 转换为安全的string
        /// </summary>
        public static string ToSafeString(object value)
        {
            return value.ToSafeString();
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
                double nsDate =Convert.ToDouble (value);
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

            if(len <= 0)
            {
                return null;
            }

            //如果是13位为毫秒 10000
            //如果是10为秒，10000000
            //DateTime.Now.Ticks 是指从DateTime.MinValue之后过了多少时间，10000000为一秒
            int strlen = len.ToString().Length;


            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(startYear, 1, 1));
            if(strlen == 13)
            {
                len *= 10000;
            }
            else if(strlen <= 10)
            {
                len *= 10000000;
            }
            else if(strlen >= 19)
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

        #region 右移24的日期
        /// <summary>
        /// 转换UTC的时间戳为安全的DateTime，如果转换错误，则会返回DateTime.MaxValue
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public static DateTime? ToSafeDateTimeForMoveRight24(object value, int startYear = 1970)
        {
            var len = value.ToSafeString().ToSafeInt64();

            if (len <= 0)
            {
                return null;
            }

            // 先右移24位
            len = len >> 24;

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

        #region ToDynamicX
        /// <summary>
        /// 转换为Dynamic集合
        /// </summary>
        public static List<dynamic> ToDynamicList(IEnumerable<object> items)
        {
            List<dynamic> res = new List<dynamic>();
            items.ForEach(s => res.Add(s as dynamic));
            return res;
        }
        #endregion

        #region WindowsFiletimeToDateTime
        /// <summary>
        /// 将windows的fileIme时间转化为本地时间
        /// </summary>
        /// <param name="hiDateTime">日期高位码</param>
        /// <param name="lowDateTime">日期地位码</param>
        /// <returns></returns>
        public static DateTime? WindowsFiletimeToDateTime(uint hiDateTime, uint lowDateTime)
        {
            #region 以前
            //long timeutcFormat;

            //// 先将4位高位日期转为数组
            //byte[] highBytes = BitConverter.GetBytes(hiDateTime);

            //// 然后转换为8位
            //Array.Resize(ref highBytes, 8);

            //// 高位字节转为一个4位的long
            //timeutcFormat = BitConverter.ToInt64(highBytes, 0);

            //// 位移移动字节
            //timeutcFormat = timeutcFormat << 32;
            //// 与低位相与，高位有为负的可能
            //timeutcFormat = timeutcFormat | lowDateTime;
            //// 转换成一个utc的日期 
            //var utcDateTime = DateTime.FromFileTimeUtc(timeutcFormat);
            //// 转换成本地的日期
            //return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.Local);
            #endregion
            byte[] tempBytesH = BitConverter.GetBytes(hiDateTime);
            byte[] tempBytesL = BitConverter.GetBytes(lowDateTime);

            List<byte> temp = new List<byte>();
            temp.AddRange(tempBytesL);
            temp.AddRange(tempBytesH);

            long tempValue = BitConverter.ToInt64(temp.ToArray(), 0);

            //DateTime 最长时间 
            DateTime max = DateTime.MaxValue.AddYears(-1600);
            TimeSpan maxSpan = max - (new DateTime(0));
            long maxTick = (long)maxSpan.TotalSeconds * TimeSpan.TicksPerSecond;


            DateTime timeValue = new DateTime();
            if (tempValue >= maxTick)
            {
                timeValue = DateTime.MaxValue;
            }
            else
            {
                DateTime tempTt = new DateTime(tempValue);
                timeValue = TimeZoneInfo.ConvertTimeFromUtc(tempTt.AddYears(1600), TimeZoneInfo.Local);
            }
            return timeValue;
        }
        #endregion

        #region Clone
        /// <summary>
        /// 拷贝对象上的属性、值
        /// </summary>
        public static void Clone(dynamic source, dynamic target)
        {
            if (source == null || target == null)
            {
                return;
            }
            DynamicX s = source as DynamicX;
            DynamicX t = target as DynamicX;
            if (s == null || t == null)
            {
                return;
            }
            foreach (var m in s.Members)
            {
                t.Set(m.Key, m.Value);
            }
        }
        #endregion

        #region Json2Dynamic
        public static dynamic Json2Dynamic(string json)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });
            dynamic dy = jss.Deserialize(json, typeof(object)) as dynamic;
            return dy;
        }
        #endregion


        /// <summary>
        /// 对加密URL进行解密
        /// </summary>
        /// <param name="url">网络地址</param>
        /// <returns></returns>
        public static string ToLocalURL(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        /// <summary>
        /// 对网络进行加密
        /// </summary>
        /// <param name="pCode"></param>
        /// <returns></returns>
        public static string ToNetEncode(string pCode)
        {
            string rtn = "", base64;
            int index = 0;
            Regex regAsis = new Regex(@"\G(?:[\x20-\x25\x27-\x7e])+");
            Regex reg26 = new Regex(@"\G&");
            Regex regEncode = new Regex(@"\G(?:[^\x20-\x7e])+");
            Regex regEq = new Regex(@"=+$");
            Regex regSlash = new Regex(@"\/");
            while (index < pCode.Length)
            {
                Match m;
                m = regAsis.Match(pCode, index);
                if (m.Success)
                {
                    index = index + m.Length;
                    rtn = rtn + m.Value;
                    continue;
                }
                m = reg26.Match(pCode, index);
                if (m.Success)
                {
                    index = index + m.Length;
                    rtn = rtn + "&-";
                    continue;
                }
                m = regEncode.Match(pCode, index);
                if (m.Success)
                {
                    index = index + m.Length;
                    base64 = Convert.ToBase64String(Encoding.GetEncoding("UTF-16BE").GetBytes(m.Value));
                    base64 = regEq.Replace(base64, "");
                    base64 = regSlash.Replace(base64, ",");
                    rtn = rtn + "&" + base64 + "-";
                    continue;
                }
            }
            return rtn;
        }
        /// <summary>
        /// 对通过base64位转码后的中文进行解析
        /// </summary>
        /// <param name="pEncodeCode">加密字符串</param>
        /// <returns></returns>
        public static string ToNetDeconde(string pEncodeCode)
        {
            string rtn = "", base64;
            int index = 0;
            Regex regAsis = new Regex(@"\G([^&]+)");
            Regex reg26 = new Regex(@"\G\&-");
            Regex regDecode = new Regex(@"\G\&([A-Za-z0-9+,]+)-?");
            Regex regComma = new Regex(@",");
            while (index < pEncodeCode.Length)
            {
                Match m;
                m = regAsis.Match(pEncodeCode, index);
                if (m.Success)
                {
                    index = index + m.Length;
                    rtn = rtn + m.Value;
                    continue;
                }
                m = reg26.Match(pEncodeCode, index);
                if (m.Success)
                {
                    index = index + m.Length;
                    rtn = rtn + "&";
                    continue;
                }
                m = regDecode.Match(pEncodeCode, index);
                if (m.Success)
                {
                    index = index + m.Length;
                    base64 = m.Value.Substring(1, m.Value.Length - 2);
                    base64 = regComma.Replace(base64, "/");
                    int mod = base64.Length % 4;
                    if (mod > 0) base64 = base64.PadRight(base64.Length + (4 - mod), '=');

                    base64 = Encoding.GetEncoding("UTF-16BE").GetString(Convert.FromBase64String(base64));
                    rtn = rtn + base64;
                    continue;
                }
            }
            return rtn;
        }

    }
}
