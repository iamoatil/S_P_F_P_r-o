using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/* ==============================================================================
* Description：Encrypt  
* Author     ：fenghj
* Create Date：2016/12/21 15:22:18
* ==============================================================================*/

namespace XLY.SF.Project.ScriptEngine.Engine
{
    /// <summary>
    /// 处理文件加密
    /// </summary>
    public class Encrypt
    {
        /// <summary>
        /// 对字节数组计算md5值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string MD5(int[] data)
        {
            byte[] retVal = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                retVal[i] = (byte)data[i];
            }
            return MD5(retVal);
        }

        /// <summary>
        /// 对字符串计算MD5
        /// </summary>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string MD5(string data, string code)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = Encoding.GetEncoding(code).GetBytes(data);
            return MD5(retVal);
        }

        /// <summary>
        /// 对文件计算MD5
        /// </summary>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string MD5(string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                var retVal = md5.ComputeHash(file);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private string MD5(byte[] data)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var retVal = md5.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
