using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace XLY.SF.Project.ScriptEngine.Engine
{
    public class PListHelper
    {
        /// <summary>
        /// 读取Plist文件为Json字符串 即将删除慎用
        /// </summary>
        /// <param name="plistFilePath">文件路径</param>
        /// <returns>Json字符串</returns>
        public static string ReadToJsonString(string plistFilePath)
        {
            return Framework.BaseUtility.PListHelper.ReadToJsonString(plistFilePath);
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
            return Framework.BaseUtility.PListHelper.ReadPListToFile(srcPath, savePath);
        }

        /// <summary>
        /// 将Json字节转化为Json字符串 即将删除慎用
        /// </summary>
        /// <param name="jsonBuffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ConvertToJsonString(byte[] jsonBuffer, int size)
        {
            return Framework.BaseUtility.PListHelper.ConvertToJsonString(jsonBuffer, size);
        }

        private static byte[] PtrtoByteArray(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return new byte[0];
            }
            var bytes = new List<byte>();
            unsafe
            {
                int length = 0;
                byte* p = (byte*)ptr;
                while (*p != 0 && length < 10000000)
                {
                    bytes.Add(*p);
                    length++;
                    p++;
                }
            }
            return bytes.ToArray();
        }
    }
}
