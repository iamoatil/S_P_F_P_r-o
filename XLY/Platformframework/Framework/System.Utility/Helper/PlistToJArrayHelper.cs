/* ==============================================================================
 * 功能描述：PlistToJarrayHelper  
 * 创 建 者：Administrator
 * 创建日期：2016/11/23 17:12:25 
 * 修 改 者：Administrator
 * 修改日期：2016/11/23 17:12:25
 
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System.Utility.Helper
{
    public class PlistToJArrayHelper
    {

        /// <summary>
        /// 将plist文件转换成jarray对象
        /// </summary>
        /// <param name="plist"></param>
        /// <returns></returns>
        [Obsolete(message: "即将删除慎用", error: false)]
        public static JArray GetJArrayPlist(string plist)
        {
            var json = PListHelper.ReadToJsonString(plist);
            return JsonToObject(json);
        }

        /// <summary>
        /// json字符串转换为JArray对象
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>返回JArray对象</returns>
        public static JArray JsonToObject(string jsonStr)
        {
            return (JArray)JsonConvert.DeserializeObject(jsonStr);
        }

        /// <summary>
        /// Jarray转 Dictionary
        /// </summary>
        /// <param name="jarray"></param>
        /// <returns></returns>
        public static Dictionary<string, object> JarryToDictionary(JArray jarray)
        {
            var result = new Dictionary<string, object>();
            foreach (JToken item in jarray)
            {
                //item.
                var jpro = (JProperty)item.First;
                var obj = jpro.Value;
                string type = obj.GetType().Name;
                var dicitem = new KeyValuePair<string, object>(jpro.Name, obj);
                result.Add(dicitem.Key, dicitem.Value);
            }
            return result;
        }

        /// <summary>
        /// 该方法适用于v3.0.0之后版本调用（After） 
        /// 其他版本尽量避免调用
        /// </summary>
        /// <param name="plistPath"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Obsolete(message: "即将删除慎用", error: false)]
        public static object AnalysisFriend(string plistPath, string key = "NSDictionary")
        {
            var jArray = (JArray)JsonConvert.DeserializeObject(PListHelper.ReadToJsonString(plistPath));
            return GetJToken(key, jArray);
        }

        public static string GetJPropertyValue(string key, IEnumerable<JToken> jArray)
        {
            var data = GetJProperty(key, jArray);
            return (data == null || data.Value.ToSafeString() == "$null") ? null : data.Value.ToSafeString();
        }

        public static JProperty GetJProperty(string key, IEnumerable<JToken> jArray)
        {
            var data = GetJToken(key, jArray);
            if (data is JProperty)
                return data as JProperty;
            return null;
        }

        /// <summary>
        /// 获取一个jToken节点下的所有children;（返回的是jarray）
        /// </summary>
        /// <param name="jToken">一个jToken的对象</param>
        /// <returns></returns>
        public static IEnumerable<JToken> GetJArrayChildren(JToken jToken)
        {
            var itemsProperty = new List<JToken>();
            if (jToken.HasValues)
                ForeachData(jToken, itemsProperty);
            return itemsProperty;
        }

        private static void ForeachData(JToken jToken, List<JToken> itemsProperty)
        {
            if (jToken.HasValues)
            {
                if (jToken.First is JArray)
                {
                    var jarray = jToken.First as JArray;
                    var childrens = jarray.Children();
                    itemsProperty.AddRange(childrens);
                    return;
                }
                ForeachData(jToken.First, itemsProperty);
            }
        }

        public static JToken GetJToken(string key, IEnumerable<JToken> jArray)
        {
            foreach (var arr in jArray)
            {
                var next = arr.DeepClone();
                while (next != null)
                {
                    if (next is JProperty)
                    {
                        var name = (next as JProperty).Name;
                        if (name.Equals(key))
                        {
                            return next;
                        }
                    }
                    if (!next.HasValues)
                    {
                        next = null;
                        continue;
                    }
                    next = next.First;
                }
            }
            return null;
        }

    }
}
