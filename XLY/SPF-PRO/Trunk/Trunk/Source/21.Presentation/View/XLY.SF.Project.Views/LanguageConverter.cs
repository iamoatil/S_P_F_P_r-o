using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/1 15:57:11
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.Views
{
    public class LanguageConverter
    {
        static LanguageConverter()
        {
            XmlProvider = new XmlDataProvider();
            XmlProvider.Source = new Uri("Pack://application:,,,/XLY.SF.Framework.Language;Component/Language/Language_Cn.xml", UriKind.Absolute);
            XmlProvider.XPath = "LanguageResource";
        }

        public static XmlDataProvider XmlProvider { get; set; }
    }
}
