using System;
using System.Security.Cryptography;

namespace XLY.XDD.ForensicService
{
    /// <summary>
    /// 校验算法提供服务
    /// </summary>
    internal class VerifyProviderFactory : System.Utility.Patterns.ISimpleFactory<EnumVerifyType, HashAlgorithm>
    {
        /// <summary>
        /// 根据key获取实例
        /// </summary>
        public HashAlgorithm GetInstance(EnumVerifyType key)
        {
            switch (key)
            {
                case EnumVerifyType.MD5:
                    return new MD5CryptoServiceProvider();
                case EnumVerifyType.SHA1:
                    return new SHA1CryptoServiceProvider();
                case EnumVerifyType.SHA256:
                    return new SHA256Managed();
                case EnumVerifyType.SHA512:
                    return new SHA512Managed();
            }
            return null;
        }
    }
}