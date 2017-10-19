/* ==============================================================================
 * 功能描述：PListHelper  
 * 创 建 者：Hogan
 * 创建日期：2016/11/21 17:02:07 
 * 修 改 者：Hogan
 * 修改日期：2016/11/21 17:02:07
 
 * ==============================================================================*/

using Newtonsoft.Json.Linq;

namespace XLY.SF.Project.ScriptEngine.Engine
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
            return Framework.BaseUtility.MacPlistHelper.ReadPlistToJArray(path);
        }

        /// <summary>
        /// 转换指定文件为JObject
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JObject ReadPlistToJObject(string path)
        {
            return Framework.BaseUtility.MacPlistHelper.ReadPlistToJObject(path);
        }

        public static JObject ReadPListToJObject(byte[] data)
        {
            return Framework.BaseUtility.MacPlistHelper.ReadPListToJObject(data);
        }

        /// <summary>
        /// 转换指定文件为Json字符串
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadPlistToJson(string path)
        {
            return Framework.BaseUtility.MacPlistHelper.ReadPlistToJson(path);
        }

        /// <summary>
        /// 转换指定文件为Object对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object ReadPlist(string path)
        {
            return Framework.BaseUtility.MacPlistHelper.ReadPlist(path);
        }

        public static object ReadPlist(byte[] data)
        {
            return Framework.BaseUtility.MacPlistHelper.ReadPlist(data);
        }

        /// <summary>
        /// 解析归档类型的PList
        /// </summary>
        /// <param name="path">plist的路径</param>
        /// <returns></returns>
        public static object ReadArchiverPlist(string path)
        {
            return Framework.BaseUtility.MacPlistHelper.ReadArchiverPlist(path);
        }

        /// <summary>
        /// 解析归档类型的PList
        /// </summary>
        /// <param name="data">plist的数据</param>
        /// <returns></returns>
        public static object ReadArchiverPlist(byte[] data)
        {
            return Framework.BaseUtility.MacPlistHelper.ReadArchiverPlist(data);
        }
    }
}
