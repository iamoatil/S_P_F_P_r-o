/* ==============================================================================
 * 功能描述：PListHelper  
 * 创 建 者：Hogan
 * 创建日期：2016/11/21 17:02:07 
 * 修 改 者：Hogan
 * 修改日期：2016/11/21 17:02:07
 
 * ==============================================================================*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace System.Utility.Helper
{
    /// <summary>
    /// Plist外部接口
    /// </summary>
    public static class MacPlistHelper
    {
        /// <summary>
        /// 转换指定文件为JArray
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JArray ReadPlistToJArray(string path)
        {
            return JsonToJArray(ReadPlistToJson(path));
        }


        /// <summary>
        /// 转换指定文件为JObject
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JObject ReadPlistToJObject(string path)
        {
            return JsonToJObject(ReadPlistToJson(path));
        }

        public static JObject ReadPListToJObject(byte[] data)
        {
            return JsonToJObject(JsonConvert.SerializeObject(ReadPlist(data)));
        }

        /// <summary>
        /// Json字符串转换为JArray
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static JArray JsonToJArray(string json)
        {
            return (JArray)JsonConvert.DeserializeObject(json);
        }


        /// <summary>
        /// Json字符串转换为JObject
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static JObject JsonToJObject(string json)
        {
            return (JObject)JsonConvert.DeserializeObject(json);
        }

        /// <summary>
        /// 转换指定文件为Json字符串
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadPlistToJson(string path)
        {
            return new MacPListParse().ReadPlistToJson(path);
        }

        /// <summary>
        /// 转换指定文件为Object对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object ReadPlist(string path)
        {
            return new MacPListParse().ReadPlistToObject(path);
        }

        public static object ReadPlist(byte[] data)
        {
            return new MacPListParse().ReadPlistToObject(data);
        }

        /// <summary>
        /// 解析归档类型的PList
        /// </summary>
        /// <param name="path">plist的路径</param>
        /// <returns></returns>
        public static object ReadArchiverPlist(string path)
        {
            var plistParser = new MacPListParse();
            var bplistObject = plistParser.ReadPlistToObject(path);

            object archiverObject;
            if (plistParser.TryParseArchiver(bplistObject, out archiverObject))
            {
                return archiverObject;
            }

            return bplistObject;
        }

        /// <summary>
        /// 解析归档类型的PList
        /// </summary>
        /// <param name="data">plist的数据</param>
        /// <returns></returns>
        public static object ReadArchiverPlist(byte[] data)
        {
            var plistParser = new MacPListParse();
            var bplistObject = plistParser.ReadPlistToObject(data);

            object archiverObject;
            if (plistParser.TryParseArchiver(bplistObject, out archiverObject))
            {
                return archiverObject;
            }

            return bplistObject;
        }
    }
}
