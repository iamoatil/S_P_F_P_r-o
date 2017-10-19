using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using XLY.SF.Framework.BaseUtility.BaseUtilityEnum;
using XLY.SF.Framework.BaseUtility;


namespace XLY.SF.Project.ScriptEngine.Engine
{
    /// <summary>
    /// 数据转换
    /// </summary>
    public class Convert
    {
        /// <summary>
        /// 转换为长整型
        /// </summary>
        public string ToLong(object value)
        {
            return System.Convert.ToInt64(value.ToString()).ToString();
        }

        /// <summary>
        /// 转换为整数类型
        /// </summary>
        public string ToInt(object value)
        {
            return System.Convert.ToInt64(value.ToString()).ToString();
        }

        /// <summary>
        /// 转换为浮点数（带小数的数字）
        /// </summary>
        public string ToDouble(object value)
        {
            return System.Convert.ToDouble(value.ToString()).ToString();
        }

        /// <summary>
        /// 转换为安全的字符串值
        /// </summary>
        public string ToString(object value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 转换为数据类型的枚举值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ToDataState(object value)
        {
            var e = DynamicConvert.ToEnumByValue(value, EnumDataState.Normal);
            return e.ToString();
        }

        /// <summary>
        /// xml内容转换为json对象
        /// </summary>
        public string XMLToJSON(object value)
        {
            try
            {
                XmlDocument n = new XmlDocument();
                n.LoadXml(value.ToString());
                return Newtonsoft.Json.JsonConvert.SerializeXmlNode(n);
            }
            catch 
            {
                return string.Empty;
            }
        }

        #region Encode（字符转码）

        /// <summary>
        /// 字符转码，sourcecode表示原理的编码，targetcode表示转换的目标编码
        /// 基本编码：UnicodeFFFE, UTF-8, UTF-7, UTF-16, UTF-32, UTF-16BE, UTF-16LE, UTF-32BE, ASCII, ISO-8859-1, GB2312, BIG5, GBK, GB18030
        /// 其他编码：quoted-printable，BASE64
        /// </summary>
        public string Encode(object value, string sourcecode, string targetcode)
        {
            try
            {
                sourcecode = sourcecode.ToUpper();
                Encoding tc = Encoding.GetEncoding(targetcode);
                var str = value.ToString();
                switch (sourcecode)
                {
                    case "QUOTED-PRINTABLE":
                        return this.QuotedPrintableEncode(str, tc);
                    case "BASE64":
                        return this.BASE64Encode(str, tc);
                    default:
                        Encoding sc = Encoding.GetEncoding(sourcecode);

                        var bytes = System.Text.Encoding.Convert(sc, tc, sc.GetBytes(str));
                        return sc.GetString(bytes);
                }
            }
            catch
            {
                return value.ToString();
            }
        }

        private string BASE64Encode(string value, Encoding target)
        {
            var data = System.Convert.FromBase64String(value);
            return GetString(data,target);
        }

        public string GetString( byte[] bytes, System.Text.Encoding encode)
        {
            if (bytes == null || bytes.Count() <= 0)
            {
                return string.Empty;
            }
            return encode.GetString(bytes);
        }

        private string QuotedPrintableEncode(string value, Encoding target)// QP编码
        {
            ArrayList vBuffer = new ArrayList();

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '=')
                {
                    i++;
                    if (value[i] != '\r')
                    {
                        byte vByte;
                        if (byte.TryParse(value.Substring(i, 2),
                            NumberStyles.HexNumber, null, out vByte))
                            vBuffer.Add(vByte);
                    }
                    i++;
                }
                else if (value[i] != '\n') vBuffer.Add((byte)value[i]);
            }
            return target.GetString((byte[])vBuffer.ToArray(typeof(byte)));

        }

        #endregion

        #region UrlDecode（URL解码）

        /// <summary>
        /// URL解码
        /// </summary>
        public string UrlDecode(object value)
        {
            return UrlDecode(value.ToString());
        }

        /// <summary>
        /// URL编码
        /// </summary>
        public string UrlEncode(object value)
        {
            return UrlEncode(value.ToString());
        }

        /// <summary>
        /// Url地址字符编码
        /// </summary>
        public string UrlEncode(string source, EnumEncodingType encodingType = EnumEncodingType.UTF_8)
        {
            if (!string.IsNullOrEmpty(source))
            {
                var chs = Encoding.GetEncoding(GetDescription(encodingType));
                return System.Web.HttpUtility.UrlEncode(source, chs);
            }
            return string.Empty;
        }
        private static ConcurrentDictionary<Enum, string> _CacheDescriptions = new ConcurrentDictionary<Enum, string>();
        public  string GetDescription(Enum @this)
        {
            return _CacheDescriptions.GetOrAdd(@this, (key) =>
            {
                var type = key.GetType();
                var field = type.GetField(key.ToString());
                //如果field为null则应该是组合位域值，
                return field == null ? GetDescriptions(key) : GetDescription(field);
            });
        }
        /// <summary>
        /// 获取位域枚举的描述，多个按分隔符组合
        /// </summary>
        public  string GetDescriptions(Enum @this, string separator = ",")
        {
            var names = @this.ToString().Split(',');
            string[] res = new string[names.Length];
            var type = @this.GetType();
            for (int i = 0; i < names.Length; i++)
            {
                var field = type.GetField(names[i].Trim());
                if (field == null) continue;
                res[i] = GetDescription(field);
            }
            return string.Join(separator, res);
        }

        private static string GetDescription(FieldInfo field)
        {
            var att = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute), false);
            return att == null ? string.Empty : ((DescriptionAttribute)att).Description;
        }
        /// <summary>
        /// Url 地址字符解码
        /// </summary>
        public string UrlDecode(string source, EnumEncodingType encodingType = EnumEncodingType.UTF_8)
        {
            if (!string.IsNullOrEmpty(source))
            {
                var chs = Encoding.GetEncoding(GetDescription(encodingType));
                return System.Web.HttpUtility.UrlDecode(source, chs);
            }
            return string.Empty;
        }

        #endregion

        /// <summary>
        /// 转换长整型的linux时间数字值为时间格式
        /// </summary>
        public string LinuxToDateTime(object value)
        {
            var dt =DateTime.Parse(value.ToString());
            return dt != null ? ToDateTimeString(dt) : string.Empty;
        }
        public string ToDateTimeString(DateTime? value)
        {
            if (value!=null)
            {
                return ToDateTimeString(value.Value);
            }
            return string.Empty;
        }
        public string ToDateTimeString(DateTime value)
        {
            if (value==null)
            {
                return string.Empty;
            }
            if (value.Year == 1970 && value.Month == 1 && value.Day == 1 && value.Hour == 0 && value.Minute == 0 && value.Second == 0)
            {
                return string.Empty;
            }
            if (value.Hour == 0 && value.Minute == 0 && value.Second == 0)
            {
                return ToDateString(value);
            }
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public  string ToDateString(DateTime value)
        {
            return IsValid(value) ? value.ToString("yyyy-MM-dd") : string.Empty;
        }
        public string ToSinaDateTime(object value)
        {
            if (value != null)
            {
                string time = value.ToString();
                if (time.Length >= 30)
                {
                    string newt = time.Substring(8, 2) + " " + time.Substring(4, 3) + " " + time.Substring(26, 4) + " " + time.Substring(11, 8);
                    var dt = DateTime.Parse(newt);
                    return IsValid(dt) ? dt.ToString("yyyy-MM-dd hh:mm:ss") : "";
                }
            }
            return "";
        }

        /// <summary>
        /// 转换长整型的GoogleChrome时间数字值为时间格式
        /// </summary>
        public string GoogleChromeToDateTime(object value)
        {
            var dt = ToSafeDateTimeForGoogleChrome(value);
            return IsValid(dt.Value) ? ToDateTimeString(dt) : string.Empty;
        }
        public DateTime? ToSafeDateTimeForGoogleChrome(object value, int startYear = 1601)
        {
            var len = System.Convert.ToInt64(value.ToString());

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
        /// 转换字符串为时间格式，fromat为格式，如yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string ToDateTime(object value, string format)
        {
            return DateTime.Parse(value.ToString()).ToString(format);
        }
        public bool IsValid(DateTime value)
        {
            if (DateTime.MinValue == value || DateTime.MaxValue == value)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 转换时间戳到当前时间
        /// </summary>
        /// <param name="year">起始年份</param>
        /// <param name="month">起始月份</param>
        /// <param name="day">起始日</param>
        /// <param name="seconds">时间戳(单位:秒)</param>
        /// <returns></returns>
        public string ToDateTime(int year, int month, int day, double seconds)
        {
            return new DateTime(year, month, day, 0, 0, 0, 0).AddSeconds(Math.Round(seconds)).ToLocalTime().ToString();
        }

        /// <summary>
        /// 计算字符串Md5值
        /// </summary>
        /// <param name="content">需要计算Md5字符串</param>
        /// <returns>返回字符串Md5值</returns>
        public string CalculateMd5(object content)
        {
            return MD5Encrypt(content.ToString());
        }
        public string MD5Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            byte[] source = Encoding.Unicode.GetBytes(value);
            var result = MD5(source, false);
            return result;
        }
        private static string MD5(byte[] buffer, bool isXLY)
        {
            //Type objType = o.GetType();
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal;
            if (isXLY)
            {
                buffer = AddByte(buffer);
            }
            retVal = md5.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        private static readonly byte[] baseByte = { 88, 76, 89 };
        private static byte[] AddByte(byte[] buffer)
        {
            List<byte> result = new List<byte>();
            result.AddRange(baseByte);
            result.AddRange(buffer);
            result.AddRange(baseByte);
            return result.ToArray();
        }
    }

    public enum EnumDataState
    {

        /// <summary>
        /// 未知
        /// </summary>
        None = 0,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 2,

        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = 1,

        /// <summary>
        /// 碎片
        /// </summary>
        Fragment = 4,
    }
}
