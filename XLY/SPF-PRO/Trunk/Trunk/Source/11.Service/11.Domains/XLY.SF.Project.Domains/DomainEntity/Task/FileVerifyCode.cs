using System;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 文件校验码
    /// </summary>
    [Serializable]
    public class FileVerifyCode
    {
        #region 文件验证码构造函数

        /// <summary>
        ///  FileVerifyCode-构造函数（初始化）
        /// </summary>
        public FileVerifyCode()
        {

        }

        public FileVerifyCode(string file, string code)
        {
            this.FilePath = file;
            this.VerifyCode = code;
        }

        #endregion

        public string FilePath { get; set; }

        public string VerifyCode { get; set; }
    }
}