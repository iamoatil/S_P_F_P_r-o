
using System;

namespace XLY.SF.Project.ScriptEngine.Engine
{
    /// <summary>
    /// 效率源脚本引擎
    /// </summary>
    public class XLYEngine : IDisposable
    {
        /// <summary>
        /// 调试输出
        /// </summary>
        public Debug Debug { get; set; }

        /// <summary>
        /// sqlite操作
        /// </summary>
        public Sqlite Sqlite { get; set; }

        /// <summary>
        /// 数据转换
        /// </summary>
        public Convert Convert { get; set; }

        /// <summary>
        /// 文件操作
        /// </summary>s
        public File File { get; set; }

        /// <summary>
        /// PList文件操作
        /// </summary>
        public PList PList { get; set; }

        /// <summary>
        /// 二进制操作
        /// </summary>s
        public BlobHelper Blob { get; set; }

        /// <summary>
        /// 位域操作
        /// </summary>
        public BitHelper Bit { get; set; }

        /// <summary>
        /// 加密算法
        /// </summary>
        public Encrypt Encrypt { get; set; }
        /// <summary>
        /// 底层接口
        /// </summary>
        public DllHelper Dll { get; set; }

        /// <summary>
        /// 输出日志信息
        /// </summary>
        /// <param name="mes"></param>
        public void Log(string mes)
        {
            this.Debug.Write(mes);
        }

        public XLYEngine()
        {
            this.Debug = new Debug();
            this.Sqlite = new Sqlite();
            this.Convert = new Convert();
            this.File = new File();
            this.PList = new PList();
            this.Blob = new BlobHelper();
            this.Bit = new BitHelper();
            this.Encrypt = new Encrypt();
            this.Dll = new DllHelper();
        }

        public void Dispose()
        {
            this.Blob.Dispose();
            this.Dll.Dispose();
        }
    }
}
