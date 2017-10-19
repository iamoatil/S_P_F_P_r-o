using System;
using System.ComponentModel;
using System.Security.Cryptography;

namespace XLY.XDD.ForensicService
{
    /// <summary>
    /// 哈希校验算法类型
    /// </summary>
    public enum EnumVerifyType
    {
        [Description("MD5")]
        MD5,
        [Description("SHA-1")]
        SHA1,
        [Description("SHA-256")]
        SHA256,
        [Description("SHA-512")]
        SHA512,
    }
}