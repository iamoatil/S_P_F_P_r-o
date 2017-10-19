using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/2 15:50:56
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Language.PageLanguage
{
    public class PageLanguageHelper
    {
        static PageLanguageHelper()
        {
            XmlProvider = new XmlDataProvider();
            XmlProvider.Source = new Uri("Pack://application:,,,/XLY.SF.Framework.Language;Component/Language/Language_Cn.xml", UriKind.RelativeOrAbsolute);
            XmlProvider.XPath = "LanguageResource";
        }

        /// <summary>
        /// 加载界面语言
        /// </summary>
        public static void LoadPageLanguage(Uri packUri)
        {
            if (packUri != null)
            {
                XmlProvider.Source = packUri;
                XmlProvider.XPath = "LanguageResource";
            }
        }

        public static XmlDataProvider XmlProvider { get; private set; }
    }
}
