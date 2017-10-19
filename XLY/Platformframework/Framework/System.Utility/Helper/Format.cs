using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Utility.Helper
{
    public static class Format
    {
        #region FormatSecond
        /// <summary>
        /// 把秒格式成{0}时{0}分{1}秒
        /// </summary>
        public static string FormatSecond(int second, string language = "cn")
        {
            if (language == "en" || language == "English")
            {
                if (second < 60)
                {
                    return string.Format("00:00:{0:D2}", second);
                }
                if (second < 3600)
                {
                    return string.Format("00:{0:D2}:{1:D2}", second / 60, second % 60);
                }
                else
                {
                    var h = second/3600;
                    var m = (second%3600)/60;
                    var s = (second%3600)%60;
                    return string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
                }
            }
            else
            {
                if (second < 60)
                {
                    return string.Format("{0}秒", second);
                }
                if (second < 3600)
                {
                    return string.Format("{0}分{1}秒", second / 60, second % 60);
                }
                else
                {
                    var h = second / 3600;
                    var m = (second % 3600) / 60;
                    var s = (second % 3600) % 60;
                    return string.Format("{0}时{1}分{2}秒", h, m, s);
                }
            }
        }
        #endregion
    }
}
