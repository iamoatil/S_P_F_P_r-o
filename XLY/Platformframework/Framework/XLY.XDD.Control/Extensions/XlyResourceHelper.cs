using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 资源辅助类
    /// </summary>
	public static class XlyResourceHelper
	{
        /// <summary>
        /// 获取资源字典
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static ResourceDictionary GetResourceDictionary(Uri uri)
        {
            ResourceDictionary r = new ResourceDictionary();
            r.Source = uri;
            return r;
        }

        /// <summary>
        /// 获取资源字典
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static ResourceDictionary GetResourceDictionary(string uri)
        {
            return GetResourceDictionary(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// 获取资源文件中的资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="uri">资源文件Uri</param>
        /// <param name="key">资源键值</param>
        /// <returns>资源</returns>
        public static T GetResourceFromResourceDictionary<T>(string uri, object key)
        {
            return (T)GetResourceFromResourceDictionary(uri, key);
        }

        /// <summary>
        /// 获取资源文件中的资源
        /// </summary>
        /// <param name="uri">资源文件Uri</param>
        /// <param name="key">资源键值</param>
        /// <returns>资源</returns>
        public static object GetResourceFromResourceDictionary(string uri, object key)
        {
            ResourceDictionary r = GetResourceDictionary(uri);
            return r[key];
        }
	}
}
