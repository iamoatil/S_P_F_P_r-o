using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Language;

namespace XLY.SF.Framework.BaseUtility
{
    public static class FormatHelper
    {
        #region 把秒格式成{0}时{0}分{1}秒
        /// <summary>
        /// 把秒格式成{0}时{0}分{1}秒
        /// </summary>
        public static string FormatSecond(int second)
        {
            if (second < 60)
            {
                //return string.Format("{0}" + LanguageHelperSingle.Instance.Language.OtherLanguage.DateTimeFormat_Second, second);
                return string.Format("{0}" + LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_DateTimeFormat_Second), second);
            }
            if (second < 3600)
            {
                //return string.Format("{0}" + LanguageHelperSingle.Instance.Language.OtherLanguage.DateTimeFormat_Minite + "{1}" +
                //    LanguageHelperSingle.Instance.Language.OtherLanguage.DateTimeFormat_Second, second / 60, second % 60);
                return string.Format("{0}" + LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_DateTimeFormat_Minite) + "{1}" +
                    LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_DateTimeFormat_Second), second / 60, second % 60);
            }
            else
            {
                var h = second / 3600;
                var m = (second % 3600) / 60;
                var s = (second % 3600) % 60;
                //return string.Format("{0}" + LanguageHelperSingle.Instance.Language.OtherLanguage.DateTimeFormat_Hour + "{1}" +
                //    LanguageHelperSingle.Instance.Language.OtherLanguage.DateTimeFormat_Minite + "{2}" +
                //    LanguageHelperSingle.Instance.Language.OtherLanguage.DateTimeFormat_Second, h, m, s);
                return string.Format("{0}" + LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_DateTimeFormat_Hour) + "{1}" +
                    LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_DateTimeFormat_Minite) + "{2}" +
                    LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_DateTimeFormat_Second), h, m, s);
            }
        }
        #endregion
    }
}
