/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/26 10:15:27 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 解密辅助类
    /// </summary>
    public static class DecryptHelper
    {
        /// <summary>
        /// 解密AndroidQQ 消息。
        /// </summary>
        /// <param name="input">输入的密文。</param>
        /// <param name="key">解密Key。</param>
        /// <param name="encoding">编码格式，默认为UTF8</param>
        /// <param name="isNew">是否新（目前暂不用此字段。）</param>
        /// <returns>返回解密后的明文。</returns>
        public static string DecryptAndroidQQMsg(string input, string key, Encoding encoding = null, bool isNew = true)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            byte[] inputs = encoding.GetBytes(input);
            return DecryptAndroidQQMsg(inputs, key, encoding, isNew);
        }

        /// <summary>
        /// 解密AndroidQQ 消息。
        /// </summary>
        /// <param name="inputBytes">输入的密文Byte数组。</param>
        /// <param name="key">解密Key。</param>
        /// <param name="encoding">编码格式，默认为UTF8。</param>
        /// <param name="isNew">是否新（目前暂不用此字段。）</param>
        /// <returns>返回解密后的明文。</returns>
        public static string DecryptAndroidQQMsg(byte[] inputBytes, string key, Encoding encoding = null, bool isNew = true)
        {
            if (inputBytes.IsInvalid())
            {
                return string.Empty;
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            if (key == "00000000000000")
            {
                return encoding.GetString(inputBytes).TrimEnd("\0");
            }

            byte[] keyBytes = encoding.GetBytes(key);
            int keyLength = keyBytes.Length;
            int inputLength = inputBytes.Length;

            if (inputLength > 0 && keyLength > 0)
            {
                var pszTmp = new byte[inputLength + 2];
                int nDecode = 0;
                for (int i = 0; i < inputLength; i++)
                {
                    if (!isNew && inputBytes[i] >= 128)
                    {
                        pszTmp[i] = inputBytes[i];
                        pszTmp[i + 1] = inputBytes[i + 1];
                        i += 2;
                    }

                    pszTmp[i] = (byte)(inputBytes[i] ^ keyBytes[nDecode++ % keyLength]);
                }

                return encoding.GetString(pszTmp).TrimEnd("\0");
            }

            return string.Empty;
        }

    }
}
