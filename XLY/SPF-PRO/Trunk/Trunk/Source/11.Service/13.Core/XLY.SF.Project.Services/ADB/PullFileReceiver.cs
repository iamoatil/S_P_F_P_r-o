using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static XLY.SF.Project.Services.ADB.CatToFileReceiver;

namespace XLY.SF.Project.Services.ADB
{
    /// <summary>
    /// 文件拷贝数据接收器.
    /// </summary>
    public class PullFileReceiver : AbstractOutputReceiver
    {
        
        private string _Local;
        /// <summary>
        /// 数据接收事件
        /// </summary>
        public ReceiveDataEvnentHandler OnReceiveData;

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public int BufferSize = 64 * 1024;

        #region PullFileReceiver-构造函数（初始化）

        /// <summary>
        ///  PullFileReceiver-构造函数（初始化）
        /// </summary>
        public PullFileReceiver(string local)
        {
            this._Local = local;
        }

        #endregion

        /// <summary>
        /// 执行数据接收
        /// </summary>
        /// <param name="socket"></param>
        public void DoReceive(Socket socket)
        {
            //read check result
            byte[] pullResult = new byte[8];
            AdbSocketHelper.Read(socket, pullResult);
            this.CheckPullFileResult(pullResult);
            byte[] data = new byte[this.BufferSize];
            FileInfo f = new FileInfo(this._Local);
            FileStream fos = fos = new FileStream(f.FullName, System.IO.FileMode.Create, FileAccess.Write);
            using (fos)
            {
                while (true)
                {
                    if (AdbSocketHelper.CheckResult(pullResult, Encoding.UTF8.GetBytes("DONE")))
                    {
                        break;
                    }
                    if (AdbSocketHelper.CheckResult(pullResult, Encoding.UTF8.GetBytes("DATA")) == false)
                    {
                        throw new ADBConnectionException();
                    }
                    //get an check buffer length
                    int length = Swap32bitFromArray(pullResult, 4);
                    if (length > 64 * 1024)
                    {
                        throw new ApplicationException("Receiving too much data.");
                    }
                    //read data
                    AdbSocketHelper.Read(socket, data, length);
                    AdbSocketHelper.Read(socket, pullResult);
                    //write file
                    try
                    {
                        fos.Write(data, 0, length);
                    }
                    catch (IOException e)
                    {
                        throw new ApplicationException("Writing local file failed!", e);
                    }
                    //receive event
                    if (this.OnReceiveData != null)
                    {
                        this.OnReceiveData(data, 0, length);
                    }
                }
                //flush
                try
                {
                    fos.Flush();
                    //设置文件的原始创建时间
                    //File.SetCreationTime(f.FullName,);
                }
                catch (IOException e)
                {
                    throw new ApplicationException("Writing local file failed!", e);
                }
            }
        }

        public int Swap32bitFromArray(byte[] value, int offset)
        {
            int v = 0;
            v |= ((int)value[offset]) & 0x000000FF;
            v |= (((int)value[offset + 1]) & 0x000000FF) << 8;
            v |= (((int)value[offset + 2]) & 0x000000FF) << 16;
            v |= (((int)value[offset + 3]) & 0x000000FF) << 24;

            return v;
        }

        public override void Add(byte[] data, int offset = 0, int length = -1)
        {
        }

        /// <summary>
        /// 执行数据解析
        /// </summary>
        public override void DoResolver()
        {
            //do
        }

        /****************** private methods ******************/

        private void CheckPullFileResult(byte[] result)
        {
            if (AdbSocketHelper.CheckResult(result, Encoding.UTF8.GetBytes("DATA")) == false &&
                                AdbSocketHelper.CheckResult(result, Encoding.UTF8.GetBytes("DONE")) == false)
            {
                throw new ADBConnectionException();
            }
        }
    }
}
