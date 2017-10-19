using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Language.PageLanguage;
using XLY.SF.Framework.Core.Base;
using System.Collections;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/27 11:14:19
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Language
{
    public class LanguageHelperSingle
    {
        #region Single

        private volatile static LanguageHelperSingle _instance;
        private static object _objLock = new object();

        private LanguageHelperSingle()
        {
            _flyWeightLanguages = new Dictionary<string, string>();
            _xmlDoc = new XmlDocument();
        }

        public static LanguageHelperSingle Instance
        {
            get
            {
                if (_instance == null)
                    lock (_objLock)
                        if (_instance == null)
                            _instance = new LanguageHelperSingle();
                return _instance;
            }
        }

        #endregion

        #region 共享语言实例

        /// <summary>
        /// 当前共享的语言
        /// </summary>
        private Dictionary<string,string> _flyWeightLanguages;

        #endregion

        /// <summary>
        /// 语言XML文件
        /// </summary>
        private XmlDocument _xmlDoc;
        
        #region 公用方法

        /// <summary>
        /// 切换语言
        /// </summary>
        public void SwitchLanguage(LanguageType langType = LanguageType.Cn, bool isLoadPageLanguage = false)
        {
            lock (_flyWeightLanguages)
            {
                _flyWeightLanguages.Clear();
            }
            switch (langType)
            {
                case LanguageType.En:
                    PageLanguageHelper.LoadPageLanguage(new Uri("Pack://application:,,,/XLY.SF.Framework.Language;Component/Language/Language_En.xml", UriKind.RelativeOrAbsolute));
                    _xmlDoc.LoadXml(Resource1.Language_En); break;
                case LanguageType.Cn:
                    PageLanguageHelper.LoadPageLanguage(new Uri("Pack://application:,,,/XLY.SF.Framework.Language;Component/Language/Language_Cn.xml", UriKind.RelativeOrAbsolute));
                    _xmlDoc.LoadXml(Resource1.Language_Cn); break;
                default:
                    PageLanguageHelper.LoadPageLanguage(new Uri("Pack://application:,,,/XLY.SF.Framework.Language;Component/Language/Language_Cn.xml", UriKind.RelativeOrAbsolute));
                    _xmlDoc.LoadXml(Resource1.Language_Cn);break;
            }
        }

        /// <summary>
        /// 根据Key获取对应的实体类说明
        /// </summary>
        /// <param name="propertyKey">Key</param>
        /// <returns></returns>
        public string GetEntityDescByKey(string propertyKey)
        {
            if (!string.IsNullOrEmpty(propertyKey))
            {
                return _xmlDoc.SelectSingleNode(propertyKey).InnerText;
            }
            return string.Empty;
        }

        #endregion

        /// <summary>
        /// 根据Key获取指定语言【Languagekeys】
        /// </summary>
        /// <returns></returns>
        public string GetLanguageByKey(string languageKey)
        {
            string result = string.Empty;
            lock (_flyWeightLanguages)
            {
                if (!string.IsNullOrWhiteSpace(languageKey))
                {
                    if (!_flyWeightLanguages.ContainsKey(languageKey))
                    {
                        try
                        {
                            result = _xmlDoc.SelectSingleNode(languageKey).InnerText;
                            _flyWeightLanguages.Add(languageKey, result);
                        }
                        catch (Exception ex)
                        {
                            Log4NetService.LoggerManagerSingle.Instance.Error(ex, string.Format("获取{0}语言失败", languageKey));
                            return string.Empty;
                        }
                    }
                }
                else
                    result = _flyWeightLanguages[languageKey];
            }
            return result;
        }
    }
}
