using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataReport.BcpConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/30 10:55:34
* ==============================================================================*/

namespace XLY.SF.Project.DataReport
{
    /// <summary>
    /// BcpConverter
    /// </summary>
    public class BcpConverter
    {
        /// <summary>
        /// 正则匹配形如"张三(1234)"
        /// </summary>
        private Regex _rgCName = new Regex(@"\(.*?\)");
        private Dictionary<string, Regex> _dicReg = new Dictionary<string, Regex>();
        private object TryConverter(object value, Func<object, object> fun)
        {
            try
            {
                return fun(value);
            }
            catch (Exception)
            {
                return value;
            }
        }

        /// <summary>
        /// 数据状态转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyDataState(object value, object[] args)
        {
            return TryConverter(value, v =>
            {
                //if (v.ToSafeString() == LanguageHelper.Get("LANGKEY_ShanChu_02221")) return 1;
                if (v.ToSafeString() == "Deleted") return 1;
                //if (v.ToSafeString() == LanguageHelper.Get("LANGKEY_ZhengChang_02222")) return 0;
                if (v.ToSafeString() == "Normal") return 0;
                return 0;
            });
        }

        /// <summary>
        /// 正则匹配形如"张三(1234)"，将匹配出“张三”
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyName(object value, object[] args)
        {
            string s = value.ToSafeString();
            if (!s.Contains("(") && !s.Contains(")"))
            {
                return "";
            }
            return TryConverter(value, v =>
            {
                return _rgCName.Replace(s, "");
            });
        }

        /// <summary>
        /// 正则匹配形如"张三(1234)"，将匹配出“1234”
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyID(object value, object[] args)
        {
            string s = value.ToSafeString();
            if (!s.Contains("(") && !s.Contains(")"))
            {
                return s;
            }
            return TryConverter(value, v =>
            {
                return _rgCName.Match(s).Value.Trim('(', ')');
            });
        }

        /// <summary>
        /// 判断动作类型，01接收方、02发送方、99其他
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyAction(object value, object[] args)
        {
            if (args.IsValid() && args[0] != null)
            {
                if (args[0].ToSafeString().Contains(value.ToSafeString()))
                {
                    return "02";
                }
                else
                {
                    return "01";
                }
            }
            return "99";
        }

        /// <summary>
        /// 判断短信动作类型，01接收方、02发送方、99其他
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlySMSAction(object value, object[] args)
        {
            //if (value.ToSafeString() == LanguageHelper.Get("LANGKEY_JieShou_02223"))
            //{
            //    return "01";
            //}
            //else if (value.ToSafeString() == LanguageHelper.Get("LANGKEY_FaSong_02224"))
            //{
            //    return "02";
            //}
            //else
            {
                return "99";
            }
        }

        /// <summary>
        /// 判断短信查看状态，0未读，1已读，9其它
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyViewStatus(object value, object[] args)
        {
            //if (value.ToSafeString() == LanguageHelper.Get("LANGKEY_YiDu_02225"))
            //{
            //    return "1";
            //}
            //else if (value.ToSafeString() == LanguageHelper.Get("LANGKEY_WeiDu_02226"))
            //{
            //    return "0";
            //}
            //else
            {
                return "9";
            }
        }

        /// <summary>
        /// 判断通话状态，0未接、1接通、9其他
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyCallStatus(object value, object[] args)
        {
            //return value.ToSafeString().Contains(LanguageHelper.Get("LANGKEY_WeiJie_02227")) ? "0" : "1";
            return "0";
        }

        /// <summary>
        /// 判断通话动作类型，01接收方、02发送方、99其他
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyCallAction(object value, object[] args)
        {
            //if (value.ToSafeString().Contains(LanguageHelper.Get("LANGKEY_HuChu_02228")))
            //{
            //    return "02";
            //}
            //else if (value.ToSafeString().Contains(LanguageHelper.Get("LANGKEY_WeiZhi_02229")))
            //{
            //    return "99";
            //}
            //else
            {
                return "01";
            }
        }

        /// <summary>
        /// 判断邮件查看状态，0未读，1已读，9其它
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyMailViewStatus(object value, object[] args)
        {
            return this.TryConverter(value, v =>
            {
                //if (value.ToSafeString() == LanguageHelper.Get("LANGKEY_WeiDu_02230"))
                //{
                //    return "0";
                //}
                //else if (value.ToSafeString() == LanguageHelper.Get("LANGKEY_YiDu_02231"))
                //{
                //    return "1";
                //}
                //else
                {
                    return "9";
                }
            });
        }

        /// <summary>
        /// 将时间转换为UTC时间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object XlyUTC(object value, object[] args)
        {
            DateTime? time = value.ToSafeString().ToSafeDateTime();
            if (time != null)
            {
                double intResult = 0;
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                intResult = ((DateTime)time - startTime).TotalSeconds;
                return intResult;
            }
            return 0;
        }

        /// <summary>
        /// 将时间转换为秒数，例如“1分14秒”转换为“74"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object XlySecond(object value, object[] args)
        {
            return TryConverter(value, v =>
            {
                string content = value.ToSafeString();
                string reg = "";
                //if (content.Contains(LanguageHelper.Get("LANGKEY_Shi_02232")))
                //{
                //    reg += @LanguageHelper.Get("LANGKEY_ShiShi_02233");
                //}
                //if (content.Contains(LanguageHelper.Get("LANGKEY_Fen_02234")))
                //{
                //    reg += @LanguageHelper.Get("LANGKEY_FenFen_02235");
                //}
                //if (content.Contains(LanguageHelper.Get("LANGKEY_Miao_02236")))
                //{
                //    reg += @LanguageHelper.Get("LANGKEY_MiaoMiao_02237");
                //}
                int h = 0, m = 0, s = 0;
                if (reg.IsValid())
                {
                    Regex rg1;
                    if (_dicReg.ContainsKey(reg))
                    {
                        rg1 = _dicReg[reg];
                    }
                    else
                    {
                        rg1 = new Regex(reg); //(\w+)时(\w+)分(\w+)秒
                        _dicReg[reg] = rg1;
                    }

                    var m1 = rg1.Match(content);
                    if (m1.Success)
                    {
                        //h = !m1.Groups[LanguageHelper.Get("LANGKEY_Shi_02238")].Success ? 0 : m1.Groups[LanguageHelper.Get("LANGKEY_Shi_02238")].Value.Replace(LanguageHelper.Get("LANGKEY_Shi_02238"), "").ToSafeInt();
                        //m = !m1.Groups[LanguageHelper.Get("LANGKEY_Fen_02241")].Success ? 0 : m1.Groups[LanguageHelper.Get("LANGKEY_Fen_02241")].Value.Replace(LanguageHelper.Get("LANGKEY_Fen_02241"), "").ToSafeInt();
                        //s = !m1.Groups[LanguageHelper.Get("LANGKEY_Miao_02244")].Success ? 0 : m1.Groups[LanguageHelper.Get("LANGKEY_Miao_02244")].Value.Replace(LanguageHelper.Get("LANGKEY_Miao_02244"), "").ToSafeInt();
                    }
                }
                else   //如果没有包含时分秒等文字，默认表示为秒
                {
                    s = value.ToSafeString().ToSafeInt();
                }
                return h * 3600 + m * 60 + s;
            });
        }

        public object XlyCntMember(object value, object[] args)
        {
            string s = value.ToSafeString();
            return TryConverter(value, v =>
            {
                return s.Split(';').Count() - 1;
            });
        }
    }
}
