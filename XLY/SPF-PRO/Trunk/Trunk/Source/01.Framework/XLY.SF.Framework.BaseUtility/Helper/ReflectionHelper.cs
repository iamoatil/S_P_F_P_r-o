using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// 反射操作辅助类
    /// </summary>
    public class ReflectionHelper
    {
        #region 动态设置对象属性的值
        /// <summary>
        /// 设置对象属性的值
        /// </summary>
        public static void SettingObjectValue<T>(T obj, string propertyName, object value)
        {
            //1.validation
            if (obj == null || string.IsNullOrEmpty(propertyName) || value == null)
            {
                return;
            }
            //2.find properties 
            System.Reflection.PropertyInfo[] pis = obj.GetType().GetProperties();
            int len = pis == null ? 0 : pis.Length;
            //3.setting value
            for (int i = 0; i < len; i++)
            {
                if (string.Equals(pis[i].Name, propertyName))
                {
                    pis[i].SetValue(obj, value, null);
                    return;
                }
            }
        }
        #endregion

        #region 批量动态设置对象属性的值
        /// <summary>
        /// 批量设置对象属性的值
        /// </summary>
        public static void SettingObjectValues<T>(T obj, string[] propertyNames, object[] values)
        {
            //1.validation
            if (obj == null || propertyNames==null||!propertyNames.Any() || values==null||!values.Any())
            {
                return;
            }
            //2.find properties 
            System.Reflection.PropertyInfo[] pis = obj.GetType().GetProperties();
            if (pis==null||!pis.Any()) 
                return;
            var plen = propertyNames.Length;
            //3.set values
            for (int i = 0; i < plen; i++)
            {
                var fpi = pis.Where(s => string.Equals(s.Name, propertyNames[i]));
                if (fpi==null||!fpi.Any()) 
                    continue;
                var pi = fpi.FirstOrDefault();
                pi.SetValue(propertyNames[i], values[i], null);
            }
        }
        #endregion

        #region 动态获取指定属性的值
        /// <summary>
        /// 获取指定属性的值
        /// </summary>
        public static object GetObjectValue<T>(T obj, string propertyName)
        {
            //1.validation
            if (obj == null || string.IsNullOrEmpty(propertyName))
            {
                return null;
            }
            //2.find properties 
            System.Reflection.PropertyInfo[] pis = obj.GetType().GetProperties();
            int len = pis == null ? 0 : pis.Length;
            //3.get value
            for (int i = 0; i < len; i++)
            {
                if (string.Equals(pis[i].Name, propertyName))
                {
                    var item = pis[i].GetValue(obj, null);
                    return item;
                }
            }
            return null;
        }

        #endregion

        #region 动态获取资源文件

        /// <summary>
        /// 从程序集的资源文件中获取文本内容
        /// </summary>
        /// <param name="fileUrl">The file URL.</param>
        public static string GetResourceFile(string fileUrl)
        {
            using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(fileUrl)))
            {
                return sr.ReadToEnd();
            }
        }

        #endregion
    }
}
