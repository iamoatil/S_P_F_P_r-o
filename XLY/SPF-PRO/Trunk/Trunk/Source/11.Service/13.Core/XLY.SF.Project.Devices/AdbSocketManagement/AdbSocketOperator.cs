using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XLY.SF.Framework.Log4NetService;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/4/7 14:55:16
 * 类功能说明：
 * 1. 提供ADB中使用到的Socket
 * 2. 只提供创建、读和写功能，保持单一职能
 *
 *************************************************/

namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    /// <summary>
    /// 提供基础AdbSocket操作
    /// </summary>
    public class AdbSocketOperator : IDisposable
    {
        /// <summary>
        /// ADB默认端口
        /// </summary>
        public const int DefaultPort = 5037;

        /// <summary>
        /// 构建Socket，并自动连接本地Adb服务
        /// </summary>
        /// <param name="port"></param>
        public AdbSocketOperator(int port = DefaultPort)
        {
            Adb_Port = port;

            SocketAddress = new IPEndPoint(IPAddress.Loopback, Adb_Port);

            OpenNewAdbSocket();
        }

        #region 属性定义

        /// <summary>
        /// 套接字连接地址
        /// </summary>
        private IPEndPoint SocketAddress { get; set; }

        /// <summary>
        /// 当前Adb套接字
        /// </summary>
        private Socket CurAdbSocket { get; set; }

        /// <summary>
        /// adb默认端口号
        /// </summary>
        public int Adb_Port { get; private set; }

        /// <summary>
        /// 是否已经打开
        /// </summary>
        public bool IsOpened { get; set; }

        #endregion

        #region 内部方法

        #region 创建Socket

        /// <summary>
        /// 创建Socket，重复创建会释放之前的
        /// </summary>
        public void OpenNewAdbSocket()
        {
            if (CurAdbSocket != null)
                CurAdbSocket.Dispose();
            try
            {
                CurAdbSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveTimeout = ConstCodeHelper.DEFAULT_TIMEOUT,
                    SendTimeout = ConstCodeHelper.DEFAULT_TIMEOUT,
                    ReceiveBufferSize = ConstCodeHelper.DEFAULT_COMMAND_BUFFER_SIZE
                };
                CurAdbSocket.Connect(SocketAddress);
                CurAdbSocket.NoDelay = true;

                IsOpened = true;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);

                IsOpened = false;
            }
        }

        #endregion

        #region 读取数据

        /// <summary>
        /// 从socket中接收读取指定大小的数据流
        /// </summary>
        public void Read(byte[] data, int length = -1)
        {
            int expLen = length < 0 ? data.Length : length;
            expLen = expLen > data.Length ? data.Length : expLen;
            CurAdbSocket.ReceiveBufferSize = expLen;
            int count = -1;
            int totalRead = 0;
            while (count != 0 && totalRead < expLen)
            {
                int left = expLen - totalRead;
                int buflen = left < CurAdbSocket.ReceiveBufferSize ? left : CurAdbSocket.ReceiveBufferSize;
                byte[] buffer = new byte[buflen];

                count = CurAdbSocket.Receive(buffer, buflen, SocketFlags.None);
                if (count > 0)
                {
                    Array.Copy(buffer, 0, data, totalRead, count);
                    totalRead += count;
                }
            }
        }

        /// <summary>
        /// 从socket中接收数据输出到接收器中
        /// </summary>
        public void Read(AbstractOutputReceiver receiver, int bufSize)
        {
            CurAdbSocket.ReceiveBufferSize = bufSize;
            var data = new byte[bufSize];
            int count = -1;
            try
            {
                while (count != 0)
                {
                    count = CurAdbSocket.Receive(data);
                    if (count <= 0)
                    {
                        break;
                    }
                    receiver.Add(data, 0, count);
                }
            }
            finally
            {
                receiver.Flush();
            }
        }

        /// <summary>
        /// 读取所有数据
        /// </summary>
        /// <returns></returns>
        public byte[] ReadToEnd()
        {
            List<byte> result = new List<byte>();
            CurAdbSocket.ReceiveBufferSize = 1024 * 16;
            byte[] value = new byte[1024 * 16];
            try
            {
                while (true)
                {
                    var readLength = CurAdbSocket.Receive(value, value.Length, SocketFlags.None);
                    if (readLength < 1)
                        break;
                    result.AddRange(value.Take(readLength));
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
            return result.ToArray();
        }

        #endregion

        #region 发送和读取

        /// <summary>
        /// 发送字节数据
        /// </summary>
        /// <returns>是否发送成功【成功发送的字节数据小于0，则视为发送失败】</returns>
        public bool Write(byte[] data, int len = 0)
        {
            try
            {
                if (CurAdbSocket.Connected)
                {
                    var count = CurAdbSocket.Send(data, len > 0 ? len : data.Length, SocketFlags.None);
                    if (count > 0)
                        return true;
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
            return false;
        }

        /// <summary>
        /// 读取应答信息
        /// </summary>
        public AdbResponse ReadResponse()
        {
            var reply = new byte[4];
            Read(reply);
            string data = Encoding.UTF8.GetString(reply);
            AdbResponse res = new AdbResponse(data)
            {
                IsOkay = reply[0] == (byte)'O' && reply[1] == (byte)'K'
                         && reply[2] == (byte)'A' && reply[3] == (byte)'Y'
            };
            return res;
        }

        #endregion

        #endregion

        #region 获取内容数据长度（目前之看到回复消息中使用）

        /// <summary>
        /// 获取socket应答的长度
        /// </summary>
        /// <returns></returns>
        public int ReadDataLength(byte readLen)
        {
            var reply = new byte[readLen];
            Read(reply);
            string tmpValue = Encoding.UTF8.GetString(reply);
            if (!int.TryParse(tmpValue, System.Globalization.NumberStyles.HexNumber, null, out int len))
            {
                len = -1;
            }
            return len;
        }

        #endregion

        #region 释放资源

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            CurAdbSocket.Close();
            CurAdbSocket.Dispose();
        }

        #endregion
    }
}
