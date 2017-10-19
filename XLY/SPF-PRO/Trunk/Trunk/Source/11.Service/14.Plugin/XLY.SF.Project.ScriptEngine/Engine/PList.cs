using Newtonsoft.Json;
using System;
using System.Text;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.ScriptEngine.Engine
{
    /// <summary>
    /// PList（BPList）文件读取
    /// </summary>
    public class PList
    {
        /// <summary>
        /// 读取PList文件到Josn字符串。
        /// </summary>
        /// <param name="plistFilePath">Plist文件路经</param>
        /// <returns>返回Josn字符串</returns>
        public string ReadToJsonString(string plistFilePath)
        {
            return MacPlistHelper.ReadPlistToJson(plistFilePath);
        }

        /// <summary>
        /// 新接口读取PList文件到Josn字符串。
        /// </summary>
        /// <param name="plistFilePath">Plist文件路经</param>
        /// <returns>返回Josn字符串</returns>
        public string MacReadToJsonString(string plistFilePath)
        {
            return MacPlistHelper.ReadPlistToJson(plistFilePath);
        }

        /// <summary>
        /// 把Json二级制字符串转换位Json字符串。
        /// </summary>
        /// <param name="jsonString">二进制字符串。</param>
        /// <returns>Json字符串。</returns>
        public string ConvertBufferToJson(string jsonString)
        {
            byte[] buffer = System.Convert.FromBase64String(jsonString);
            return JsonConvert.SerializeObject(MacPlistHelper.ReadPlist(buffer));
        }

        /// <summary>
        /// 新接口把Json二级制字符串转换位Json字符串。
        /// </summary>
        /// <param name="jsonString">二进制字符串。</param>
        /// <returns>Json字符串。</returns>
        public string MacConvertBufferToJson(string jsonString)
        {
            byte[] buffer = System.Convert.FromBase64String(jsonString);
            return JsonConvert.SerializeObject(MacPlistHelper.ReadPlist(buffer));
        }
    }
}
