using System;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Security.Cryptography;

namespace XLY.XDD.ForensicService
{
    /// <summary>
    /// 数据校验服务，使用完成请Dispose
    /// 注意：不支持多线程调用同一个服务；同一服务同一时刻只支持一个文件/数据流
    /// </summary>
    public class VerifyService : IDisposable
    {
        /// <summary>
        /// 哈希算法服务
        /// </summary>
        private HashAlgorithm _Provider;

        /// <summary>
        /// 校验类型
        /// </summary>
        public EnumVerifyType Type { get; private set; }

        //64K
        private static readonly int _PageSize = 65536;
        private byte[] _Buf = new byte[65536];

        #region VerifyService-构造函数（初始化）

        /// <summary>
        ///  VerifyService-构造函数（初始化）
        /// </summary>
        public VerifyService(EnumVerifyType type)
        {
            this.Type = type;
        }

        #endregion

        /****************** public methods ******************/

        /// <summary>
        /// 生成指定数据流的校验码。
        /// buf:数据流
        /// offset:偏移量，默认0；
        /// len:为有效长度，默认为buf长度。
        /// </summary>
        public string GenerateVerifyCode(byte[] buf, int offset = 0, int len = 0)
        {
            this._Provider = Singleton<VerifyProviderFactory>.GetInstance().GetInstance(this.Type);
            try
            {
                byte[] bytes = this._Provider.ComputeHash(buf, offset, len == 0 ? buf.Length : len);
                return this.ConvertToCode(bytes);
            }
            finally
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// 生成指定数据流的校验码。
        /// buf:数据流
        /// offset:偏移量，默认0；
        /// len:为有效长度，默认为buf长度。
        /// </summary>
        public string GenerateVerifyCode(byte[] buf)
        {
            this._Provider = Singleton<VerifyProviderFactory>.GetInstance().GetInstance(this.Type);
            try
            {
                byte[] bytes = this._Provider.ComputeHash(buf, 0, buf.Length);
                return this.ConvertToCode(bytes);
            }
            finally
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// 生成指定字符串的校验码
        /// </summary>
        /// <param name="value">字符串值</param>
        public string GenerateVerifyCode(string value)
        {
            try
            {
                return this.GenerateVerifyCode(System.Text.Encoding.Default.GetBytes(value));
            }
            finally
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// 根据字节流数据生成校验码
        /// </summary>
        /// <param name="stream">节流数</param>
        /// <returns>校验码</returns>
        public string GenerateVerifyCode(Stream stream)
        {
            if (stream == null || stream.Length <= 0) return string.Empty;
            this._Provider = Singleton<VerifyProviderFactory>.GetInstance().GetInstance(this.Type);
            try
            {
                if (stream.Length <= _PageSize)
                {
                    return this.GenerateVerifyCodeBySteam(stream);
                }
                else
                {
                    return this.GenerateVerifyCodeByBigStream(stream);
                }
            }
            finally
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// 生成指定文件的校验码。
        /// 若文件不存在，操作异常都返回空字符。
        /// </summary>
        public string GenerateVerifyCodeByFile(string fileName)
        {
            if (fileName.IsInvalid()) return string.Empty;
            this._Provider = Singleton<VerifyProviderFactory>.GetInstance().GetInstance(this.Type);
            FileStream stream = null;
            try
            {
                // ModifyBy:chenjing 2015-8-17 16:39:57 解决sqlite占用问题
                //FileShare.Read=>FileShare.ReadWrite
                stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                return this.GenerateVerifyCode(stream);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            finally
            {
                if (stream != null) stream.Dispose();
                this.Dispose();
            }
        }

        /// <summary>
        /// 追加数据流。与GenerateVerifyCodeByAppendBuffer一起用于生成连续数据流集的校验码
        /// </summary>
        public void AppendBuffer(byte[] buf, int offset = 0, int len = 0)
        {
            if (this._Provider == null)
                this._Provider = Singleton<VerifyProviderFactory>.GetInstance().GetInstance(this.Type);
            this._Provider.TransformBlock(buf, offset, len == 0 ? buf.Length : len, buf, 0);
        }

        /// <summary>
        /// 生成校验码，只适用于追加数据流（AppendBuffer）的方式。
        /// </summary>
        /// <returns></returns>
        public string GenerateVerifyCodeByAppendBuffer()
        {
            try
            {
                this._Provider.TransformFinalBlock(this._Buf, 0, 0);
                return this.ConvertToCode(this._Provider.Hash);
            }
            finally
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this._Provider != null)
            {
                this._Provider.Clear();
                this._Provider.Dispose();
                this._Provider = null;
            }
        }

        /****************** private methods ******************/

        /// <summary>
        /// 根据字节流数据生成校验码：小于等于PageSize(64K)的字节流
        /// </summary>
        /// <param name="stream">节流数</param>
        /// <returns>校验码</returns>
        private string GenerateVerifyCodeBySteam(Stream stream)
        {
            var len = stream.Read(this._Buf, 0, _PageSize);
            return this.GenerateVerifyCode(this._Buf, 0, len);
        }

        /// <summary>
        /// 根据字节流数据生成校验码：大于PageSize(64K)的字节流
        /// </summary>
        /// <param name="stream">节流数</param>
        /// <returns>校验码</returns>
        private string GenerateVerifyCodeByBigStream(Stream stream)
        {
            int len = _PageSize;
            while (len > 0)
            {
                len = stream.Read(this._Buf, 0, _PageSize);
                this._Provider.TransformBlock(this._Buf, 0, len, this._Buf, 0);
            }
            this._Provider.TransformFinalBlock(this._Buf, 0, 0);
            return this.ConvertToCode(this._Provider.Hash);
        }

        private string ConvertToCode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}