using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// 加密处理公共辅助类
    /// </summary>
    public static class CryptographyHelper
    {
        /// <summary>
        /// 默认的KEY值
        /// </summary>
        public const string DESKEY = "#s^XLY_DESKEY_1986,11+15";

        /// <summary>
        /// 用以导出密钥的密钥 salt。
        /// </summary>
        private const string AESSALT = "gsf4jvkyhye5/d7k8OrLgM==";

        /// <summary>
        /// 用于对称算法的初始化向量。
        /// </summary>
        private const string AESRGBIV = "Rkb4jvUy/ye7Cd7k89QQgQ==";

        /// <summary>
        /// 一次处理的明文字节数
        /// </summary>
        private const int ENCRYPTSIZE = 802400;

        /// <summary>
        /// 一次处理的密文字节数
        /// </summary>
        private const int DECRYPTSIZE = ENCRYPTSIZE + 16;

        /// <summary>
        /// 特定Byte位
        /// </summary>
        private static readonly byte[] baseByte = { 88, 76, 89 };

        #region MD5Encrypt：MD5的32位加密
        /// <summary>
        /// MD5的32位加密，不可逆
        /// 默认不添加特定byte位
        /// </summary>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        public static string MD5Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            byte[] source = Encoding.Unicode.GetBytes(value);
            var result = MD5(source, false);
            return result;
        }

        /// <summary>
        /// 生成MD5（大写字母）
        /// </summary>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        public static string MD5FromStringUpper(string value)
        {
            return MD5FromString(value).ToUpper();
        }

        /// <summary>
        /// 生成MD5（小写字母）
        /// </summary>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        public static string MD5FromString(string value)
        {
            if (String.IsNullOrEmpty(value))
                return string.Empty;
            byte[] bytValue;
            bytValue = Encoding.UTF8.GetBytes(value);
            return MD5(bytValue, false);
        }

        /// <summary>
        /// 生成文件MD5（大写字母）
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string MD5FromFileUpper(string filePath)
        {
            return MD5FromFile(filePath).ToUpper();
        }

        /// <summary>
        /// 生成文件MD5（小写字母）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string MD5FromFile(string filePath)
        {

            MD5 md5 = new MD5CryptoServiceProvider();
            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                var retVal = md5.ComputeHash(file);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 文本和文件MD5加密码
        /// </summary>
        /// <param name="value">文本内容</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string MD5FromStringAndFile(string value, string filePath)
        {
            byte[] buffer = CombineByte(value, filePath);
            return MD5(buffer, true);
        }

        /// <summary>
        /// 文件及文本加密
        /// </summary>
        /// <param name="o">要加密的对象可以是 byte[] 或 FileStream</param>
        /// <param name="isXLY">是否添加特定byte位（慎用添加后和其它MD5生成的不相同）</param>
        /// <returns></returns>
        private static string MD5(byte[] buffer, bool isXLY)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal;
            if (isXLY)
            {
                buffer = AddByte(buffer);
            }
            retVal = md5.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 添加特定Byte位
        /// </summary>
        /// <param name="buffer">原使Byte[]</param>
        /// <returns></returns>
        private static byte[] AddByte(byte[] buffer)
        {
            List<byte> result = new List<byte>();
            result.AddRange(baseByte);
            result.AddRange(buffer);
            result.AddRange(baseByte);
            return result.ToArray();
        }

        /// <summary>
        /// 合并文本及文件Byte
        /// </summary>
        /// <param name="value"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static byte[] CombineByte(string value, string filePath)
        {
            if (String.IsNullOrEmpty(value))
                return new byte[0];

            List<byte> result = new List<byte>();
            byte[] valueBuffer, fileBuffer;
            valueBuffer = Encoding.UTF8.GetBytes(value);
            result.AddRange(valueBuffer);
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                fileBuffer = File.ReadAllBytes(filePath);
                result.AddRange(fileBuffer);
            }

            return result.ToArray();
        }
        #endregion

        #region DES加密
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="text">要加密的文本</param>
        /// <param name="key">24位的密钥，范例：#s^un2ye31fcn%|aoXpR,+vh</param>
        public static string EncodeDES(string text, string key = DESKEY)
        {
            //如果文本为空，则返回
            if (String.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            //如果密钥的长度不是24位则返回
            if (key.Length != 24)
            {
                return string.Empty;
            }

            //创建DES加密服务提供程序
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();

            //提供加密键
            provider.Key = Encoding.ASCII.GetBytes(key);

            //提供加密模式
            provider.Mode = CipherMode.ECB;

            //创建加密器
            ICryptoTransform transform = provider.CreateEncryptor();

            //加密文本
            byte[] bytes = Encoding.Default.GetBytes(text);
            byte[] result = transform.TransformFinalBlock(bytes, 0, bytes.Length);

            //释放资源
            transform.Dispose();

            //返回文本结果
            return Convert.ToBase64String(result);
        }
        #endregion

        #region DES解密
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="text">要解密的文本</param>
        /// <param name="key">24位的密钥，范例：#s^un2ye31fcn%|aoXpR,+vh</param>
        public static string DecodeDES(string text, string key = DESKEY)
        {

            //如果文本为空，则返回
            if (String.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            //如果密钥的长度不是24位则返回
            if (key.Length != 24)
            {
                return string.Empty;
            }

            //创建DES加密服务提供程序
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();

            //提供加密键
            provider.Key = Encoding.ASCII.GetBytes(key);

            //提供加密模式
            provider.Mode = CipherMode.ECB;

            //设置填充模式
            provider.Padding = PaddingMode.PKCS7;

            //创建解密器
            ICryptoTransform transform = provider.CreateDecryptor();

            try
            {
                //解密
                byte[] bytes = Convert.FromBase64String(text);
                byte[] result = transform.TransformFinalBlock(bytes, 0, bytes.Length);

                //返回文本结果
                return Encoding.Default.GetString(result);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                transform.Dispose();
            }
        }
        #endregion


    }
}
