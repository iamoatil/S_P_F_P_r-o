using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.DataPump.PumpHelper
{
    /// <summary>
    /// 加密处理公共辅助类
    /// </summary>
    public class Cryptography
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
        /// 生成文件MD5（大写字母）
        /// 默认添加特定byte位
        /// </summary>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        public static string MD5FromStringUpper(string value)
        {
            return MD5FromString(value).ToUpper();
        }
        /// <summary>
        /// 生成文本MD5（小写字母）
        /// 默认添加特定byte位
        /// </summary>
        /// <param name="value">文本内容</param>
        /// <returns></returns>
        public static string MD5FromString(string value)
        {
            byte[] bytValue;
            bytValue = Encoding.UTF8.GetBytes(value);
            try
            {
                return MD5(bytValue, true);
            }
            catch (Exception ex)
            {
                throw new Exception("MD5FromString fail,error:" + ex.Message);
            }
        }
        /// <summary>
        /// 生成文件MD5（大写字母）
        /// 默认添加特定byte位
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string MD5FromFileUpper(string filePath)
        {
            return MD5FromFile(filePath).ToUpper();
        }
        /// <summary>
        /// 生成文件MD5（小写字母）
        /// 默认添加特定byte位
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string MD5FromFile(string filePath)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
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
            catch (Exception ex)
            {
                throw new Exception("MD5FromFile fail,error:" + ex.Message);
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
            //Type objType = o.GetType();
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
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
        /// 获得文件Byte[]
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns></returns>
        private static byte[] GetFileStreamByte(FileStream fs)
        {
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Seek(0, SeekOrigin.Begin);
            return buffer;
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
            List<byte> result = new List<byte>();
            //Logger.LogHelper.Error(string.Format("CombineByte value:{0}--filePath:{1}", value, filePath));
            byte[] valueBuffer, fileBuffer;
            valueBuffer = Encoding.UTF8.GetBytes(value);
            try
            {
                result.AddRange(valueBuffer);
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    FileStream file = new FileStream(filePath, FileMode.Open);
                    fileBuffer = GetFileStreamByte(file);
                    file.Close();
                    result.AddRange(fileBuffer);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CombineByte fail,error:" + ex.Message);
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
            if (String.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            //如果密钥的长度不是24位则返回
            if (key.Length != 24)
            {
                return string.Empty;
            }

            try
            {
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
            catch (Exception ex)
            {
                throw ex;
            }
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
            catch
            {
                return string.Empty;
            }
            finally
            {
                //释放资源
                transform.Dispose();
            }
        }
        #endregion

        #region AES加解密服务 wangxi 2015-04-30 16:09:33

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AESEncrypt(string encryptString, string key)
        {
            var srouceByte = Encoding.Default.GetBytes(encryptString);
            // 加密
            var encryptByte = AESEncrypt(srouceByte, key);
            // 将密文转换成BASE64编码;
            return Convert.ToBase64String(encryptByte);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptByte">加密明文</param>
        /// <param name="key">加密key</param>
        /// <returns></returns>
        public static byte[] AESEncrypt(byte[] encryptByte, string key)
        {
            return CreateICrypto(encryptByte, key, CryptoType.Encrypt);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AESDecrypt(string encryptString, string key)
        {
            var srouceByte = Convert.FromBase64String(encryptString);
            // 加密
            var encryptByte = AESDecrypt(srouceByte, key);
            // 将密文转换成BASE64编码;
            return Encoding.Default.GetString(encryptByte);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="encryptByte">加密密文</param>
        /// <param name="key">解密key</param>
        /// <returns></returns>
        public static byte[] AESDecrypt(byte[] encryptByte, string key)
        {
            return CreateICrypto(encryptByte, key, CryptoType.Decrypt);
        }

        /// <summary>
        /// 创建加解密服务
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="iCrypto"></param>
        /// <returns></returns>
        private static byte[] CreateICrypto(byte[] source, string key, CryptoType iCrypto)
        {
            if (source==null)
            {
                throw (new Exception("加解密数据源不可以为空!"));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw (new Exception("密钥不可以为空"));
            }

            byte[] strSource;
            var aesProvider = Rijndael.Create();
            try
            {

                var mStream = new MemoryStream();
                var pdb = new PasswordDeriveBytes(key, Convert.FromBase64String(AESSALT));
                ICryptoTransform transform = null;
                if (iCrypto == CryptoType.Encrypt)
                {
                    transform = aesProvider.CreateEncryptor(pdb.GetBytes(32), Convert.FromBase64String(AESRGBIV));
                }
                if (iCrypto == CryptoType.Decrypt)
                {
                    transform = aesProvider.CreateDecryptor(pdb.GetBytes(32), Convert.FromBase64String(AESRGBIV));
                }

                var mCsstream = new CryptoStream(mStream, transform, CryptoStreamMode.Write);
                mCsstream.Write(source, 0, source.Length);
                mCsstream.FlushFinalBlock();
                strSource = mStream.ToArray();

                mStream.Close();
                mStream.Dispose();

                mCsstream.Close();
                mCsstream.Dispose();

            }

            catch (IOException ex)
            {
                throw ex;
            }

            catch (CryptographicException ex)
            {
                throw ex;
            }

            catch (ArgumentException ex)
            {
                throw ex;
            }

            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                aesProvider.Clear();
            }

            return strSource;
        }

        /// <summary>
        /// AES加密文件
        /// </summary>
        /// <param name="encryptpath">加密文件路径</param>
        /// <param name="key">加密Key</param>
        /// <param name="isHiddenTempFile">是否隐藏当前加密的文件</param>
        public static string AESEncryptFile(string encryptpath, string key, bool isHiddenTempFile = false)
        {
            // 加密后的文件路径
            var encryptFullname = encryptpath + ".xly";
            if (System.IO.File.Exists(encryptFullname))
            {
                System.IO.File.Delete(encryptFullname);
            }
            try
            {
                using (var fs = new FileStream(encryptpath, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length == 0)
                    {
                        throw new Exception("不允许对无内容文件加密!");
                    }
                    using (var fsnew = new FileStream(encryptFullname, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        if (isHiddenTempFile)
                        {
                            if (IsValid(encryptFullname))
                            {
                                File.SetAttributes(encryptFullname, FileAttributes.Hidden);
                            }
                        }
                        // 计算要做几次加密处理,大文件分几次操作
                        var blockCount = ((int)fs.Length - 1) / ENCRYPTSIZE + 1;
                        for (var i = 0; i < blockCount; i++)
                        {
                            var size = ENCRYPTSIZE;
                            if (i == blockCount - 1)
                            {
                                size = (int)(fs.Length - i * ENCRYPTSIZE);
                            }
                            var buffer = new byte[size];
                            fs.Read(buffer, 0, size);
                            var result = AESEncrypt(buffer, key);
                            fsnew.Write(result, 0, result.Length);
                            fsnew.Flush();
                        }
                        fsnew.Close();
                        fsnew.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return encryptFullname;
        }

        public static bool IsValid(string file)
        {
            if (String.IsNullOrWhiteSpace(file))
                return false;
            if (!System.IO.File.Exists(file))
                return false;
            System.IO.FileInfo info = new FileInfo(file);
            if (info.Length <= 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// AES解密文件
        /// </summary>
        /// <param name="decryptpath">解密文件的路径</param>
        /// <param name="key">解密Key</param>
        /// <returns></returns>
        public static string AESDecryptFile(string decryptpath, string key)
        {
            var decryptFile = decryptpath.TrimEnd(new char[] { '.', 'x', 'l', 'y' });
            try
            {
                // int a = 0;
                // decryptFile=counter(decryptFile,a);
                using (var fs = new FileStream(decryptpath, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length > 0)
                    {
                        using (var fsnew = new FileStream(decryptFile, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            int blockCount = ((int)fs.Length - 1) / DECRYPTSIZE + 1;
                            for (int i = 0; i < blockCount; i++)
                            {
                                int size = DECRYPTSIZE;
                                if (i == blockCount - 1) size = (int)(fs.Length - i * DECRYPTSIZE);
                                var bArr = new byte[size];
                                fs.Read(bArr, 0, size);
                                byte[] result = AESDecrypt(bArr, key);
                                fsnew.Write(result, 0, result.Length);
                                fsnew.Flush();
                            }
                            fsnew.Close();
                            fsnew.Dispose();
                        }
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

        /////<summary>
        ///// 文件判断是否重复递归
        ///// </summary>
        //public static string counter(string decryptFile,int a)
        //{
        //    if (File.IsValid(decryptFile))
        //    {
        //        a = a+1;
        //        string p = File.GetFilePath(decryptFile);
        //        var name= File.GetFileName(decryptFile);
        //        var k = File.GetExtension(decryptFile);
        //        int b = a - 1;
        //        if (name.Contains("("+b.ToSafeString()+")"))
        //        {
        //            var c=name.TrimEnd("(" + b.ToSafeString() + ")"+".txt");
        //            name = c + ".txt";
        //        }
        //        name = name.TrimEnd(".txt") + "(" + a.ToSafeString() + ")" + "."+k;
        //        decryptFile = p + name;
        //        return counter(decryptFile,a);
        //    }
        //    else
        //    {
        //        return decryptFile;
        //    }
        // }

        /// <summary>
        /// 加解密类型
        /// </summary>
        public enum CryptoType
        {
            /// <summary>
            /// 加密
            /// </summary>
            Encrypt,
            /// <summary>
            /// 解密
            /// </summary>
            Decrypt
        }

        #endregion
    }
}
