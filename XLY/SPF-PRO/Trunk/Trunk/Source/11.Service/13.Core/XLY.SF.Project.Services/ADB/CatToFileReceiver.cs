using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Services.ADB
{
    public class CatToFileReceiver : AbstractOutputReceiver
    {
        public delegate void ReceiveDataEvnentHandler(byte[] data, int offset = 0, int length = -1);
        /// <summary>
        /// 数据接收事件
        /// </summary>
        public ReceiveDataEvnentHandler OnReceiveData;

        private FileInfo fileInfo;
        private FileStream fos;
        private bool _IsFirst = true;
        private string _FirstLine = string.Empty;

        public string LocalFile { get; set; }

        #region CatToFileReceiver-构造函数（初始化）

        /// <summary>
        ///  CatFileReceiver-构造函数（初始化）
        /// </summary>
        public CatToFileReceiver(string localfile)
        {
            this.LocalFile = localfile;
        }

        #endregion

        public override void Add(byte[] data, int offset = 0, int length = -1)
        {
            length = length < 0 ? data.Length : length;
            try
            {
                if (_IsFirst)
                {
                    var first = data.Take(length).ToArray().GetString(System.Text.Encoding.UTF8);
                    if (first.Contains("No such file or directory") || first.Contains("Permission denied"))
                    {
                        throw new Exception(first);
                    }
                    this._IsFirst = false;
                    //ini
                    this.fileInfo = new FileInfo(this.LocalFile);
                    this.fos = new FileStream(this.fileInfo.FullName, FileMode.Create, FileAccess.Write);
                }
                //data处理
                data = this.ConvertToLinuxData(data, length);
                length = data.Length;
                fos.Write(data, 0, length);
                //receive event
                if (this.OnReceiveData != null)
                {
                    this.OnReceiveData(data, 0, length);
                }
            }
            catch (IOException e)
            {
                throw new ApplicationException("Cat to file: Writing local file failed!", e);
            }
        }

        #region ConvertToLinuxData

        // \n
        private readonly byte LinuxN = 0x0a;
        // \r\n
        private readonly byte[] WinN = new byte[2] { 0x0d, 0x0a };

        private byte lastbyte;

        /// <summary>
        /// 转换数据中的分隔符为linux格式的数据。
        /// 主要是把\r\n替换为\n,
        /// </summary>
        private byte[] ConvertToLinuxData(byte[] data, int len)
        {
            List<byte> res = new List<byte>();
            for (int i = 0; i < len; i++)
            {
                if (i == 0 && lastbyte == this.WinN[0] && data[i] == this.WinN[1])
                {
                    res.Add(this.LinuxN);
                    continue;
                }
                if (i == len - 1 && data[i] == this.WinN[0])
                {
                    lastbyte = data[i];
                    continue;
                }
                else lastbyte = 0x0;
                var b = data[i];
                if (b == this.WinN[0] && data[i + 1] == this.WinN[1])
                {
                    res.Add(this.LinuxN);
                    i++;
                }
                else
                {
                    res.Add(b);
                }

            }
            return res.ToArray();
        }

        #endregion

        public override void Flush()
        {
            try
            {
                if (fos == null || !fos.CanWrite) return;
                fos.Flush();
            }
            catch (IOException e)
            {
                throw new ApplicationException("Cat to file: Writing local file failed!", e);
            }
            finally
            {
                this.Dispose();
            }
        }

        public void Dispose()
        {
            if (this.fos != null)
            {
                this.fos.Close();
                this.fos.Dispose();
            }
        }
    }
}