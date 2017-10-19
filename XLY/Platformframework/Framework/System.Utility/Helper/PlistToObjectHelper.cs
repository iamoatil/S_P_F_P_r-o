using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Utility.Helper
{
    /// <summary>
    /// Plist 转 Object 即将删除慎用
    /// </summary>
    public class PlistToObjectHelper
    {
        /// <summary>
        /// json字符串转换为JArray对象 即将删除慎用
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>返回JArray对象</returns>
        [Obsolete(message: "即将删除慎用", error: false)]
        public static JObject JsonToObject(string jsonStr)
        {
            return (JObject)JsonConvert.DeserializeObject(jsonStr);
        }

        /// <summary>
        /// json字符串转换为JArray对象 即将删除慎用
        /// </summary>
        /// <param name="path">全路径</param>
        /// <returns>返回JArray对象</returns>
        [Obsolete(message: "即将删除慎用", error: false)]
        public static JObject JsonToObjectByPath(string path)
        {
            return (JObject)JsonConvert.DeserializeObject(ReadToJsonString(path));
        }
        /// <summary>
        /// 读取Plist文件为Json字符串 即将删除慎用
        /// </summary>
        /// <param name="plistFilePath">文件路径</param>
        /// <returns>Json字符串</returns>
        [Obsolete(message: "即将删除慎用", error: false)]
        public static string ReadToJsonString(string plistFilePath)
        {
            try
            {
                int mountResult = BPListToObjectDllCore.OpenPFile(plistFilePath);
                if (mountResult != 0)
                {
                    return string.Empty;
                }

                IntPtr result = IntPtr.Zero;
                int resultCode = BPListToObjectDllCore.GetJsonBuffer(ref result);
                if (resultCode != 0)
                {
                    return string.Empty;
                }

                string jsonString = result.ToGB2312String();
                BPListToObjectDllCore.Close();
                return jsonString;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// 读取Plist文件为Json字符串 即将删除慎用
        /// </summary>
        /// <param name="srcPath">文件路径</param>
        /// <param name="savePath">保存路径</param>
        /// <returns>Json字符串</returns>
        [Obsolete(message: "即将删除慎用", error: false)]
        public static bool ReadPListToFile(string srcPath, string savePath)
        {
            try
            {
                if (!System.IO.File.Exists(savePath))
                {
                    return false;
                }

                int mountResult = BPListToObjectDllCore.OpenPFile(srcPath);
                if (mountResult != 0)
                {
                    return false;
                }

                int result = BPListToObjectDllCore.GetJsonFile(savePath);
                if (result != 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            finally
            {
                BPListToObjectDllCore.Close();
            }
        }
        /// <summary>
        /// byte array to String 即将删除慎用
        /// </summary>
        /// <param name="jsonBuffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [Obsolete(message: "即将删除慎用", error: false)]
        public static string ConvertToJsonString(byte[] jsonBuffer,int size)
        {
            try
            {
                int openCode = BPListToObjectDllCore.OpenPBuffer(jsonBuffer, size);

                if (openCode != 0)
                {
                    return string.Empty;
                }

                IntPtr result = IntPtr.Zero;
                int resultCode = BPListToObjectDllCore.GetJsonBuffer(ref result);
                if (resultCode != 0)
                {
                    return string.Empty;
                }

                string jsonString = result.ToGB2312String();
                BPListToObjectDllCore.Close();
                return jsonString;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// (B)PList DLL 核心处理。
    /// </summary>
    internal static class BPListToObjectDllCore
    {
        private const string DllPath = @"ObjectPList\parser2.dll";

        #region 装载PList

        [DllImport(DllPath, EntryPoint = "OpenPFile")]
        public static extern int OpenPFile(string plistFilePath);

        /// <summary>
        /// 数据库中有的字段存储的二进制BPlist，独处该二进制文件后，需要获取它的大小
        /// 最后把该二进制文件转换为Josn字符串。
        /// 本方法现在还没有实验过，后续可能会进一步测试
        /// </summary>
        /// <param name="plistBuffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [DllImport(DllPath, EntryPoint = "OpenPBuffer")]
        public static extern int OpenPBuffer(byte[] plistBuffer, int size);

        #endregion

        #region Plist结果

        /// <summary>
        /// 获取json字符串
        /// </summary>
        /// <param name="returnJosn"></param>
        /// <returns></returns>
        [DllImport(DllPath, EntryPoint = "GetJsonBuffer")]
        public static extern int GetJsonBuffer(ref IntPtr returnJosn);

        /// <summary>
        /// 存储为文件
        /// </summary>
        /// <param name="saveFilePath"></param>
        /// <returns></returns>
        [DllImport(DllPath, EntryPoint = "GetJsonFile")]
        public static extern int GetJsonFile(string saveFilePath);

        [DllImport(DllPath, EntryPoint = "ParserBase64")]
        public static extern int ParserBase64(string pBuffer, string outResult, ref int size);

        #endregion

        [DllImport(DllPath, EntryPoint = "Close")]
        public static extern int Close();
    }
}
