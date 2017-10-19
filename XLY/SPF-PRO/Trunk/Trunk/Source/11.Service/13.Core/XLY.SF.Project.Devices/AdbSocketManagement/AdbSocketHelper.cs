using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.BaseUtility;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/4/7 14:03:30
 * 类功能说明：
 * 1. Socket中使用到的帮助服务类
 *
 *************************************************/

namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    /// <summary>
    /// 提供Socket帮助
    /// </summary>
    public static class AdbSocketHelper
    {
        #region 构建adb请求的byte数据

        /// <summary>
        /// 将命令字符串转换为字节流
        /// </summary>
        /// <param name="req">命令字符串</param>
        /// <returns></returns>
        public static byte[] CmdToBytes(string req)
        {
            if (!string.IsNullOrEmpty(req))
            {
                string resultStr = string.Format("{0}{1}\n", req.Length.ToString("X4"), req);
                try
                {
                    return Encoding.UTF8.GetBytes(resultStr);
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error(ex);
                    return new byte[0];
                }
            }
            return new byte[0];
        }

        /// <summary>
        /// 将命令字符串转换为字节流（shell指令）
        /// </summary>
        /// <param name="req">命令字符串</param>
        public static byte[] CmdToBytesByShell(string req)
        {
            if (!string.IsNullOrEmpty(req))
            {
                if (!req.Contains("shell:"))
                    //添加Shell前缀
                    req = req.Insert(0, "shell:");

                string resultStr = string.Format("{0}{1}\n", req.Length.ToString("X4"), req);
                try
                {
                    return Encoding.UTF8.GetBytes(resultStr);
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error(ex);
                    return new byte[0];
                }
            }
            return new byte[0];
        }

        #endregion

        #region Read（从socket中接收读取指定大小的数据流）
        /// <summary>
        /// 从socket中接收读取指定大小的数据流
        /// </summary>
        public static void Read(Socket socket, byte[] data, int length = -1)
        {
            int expLen = length < 0 ? data.Length : length;
            expLen = expLen > data.Length ? data.Length : expLen;
            socket.ReceiveBufferSize = expLen;
            int count = -1;
            int totalRead = 0;
            while (count != 0 && totalRead < expLen)
            {
                int left = expLen - totalRead;
                int buflen = left < socket.ReceiveBufferSize ? left : socket.ReceiveBufferSize;
                byte[] buffer = new byte[buflen];
                count = socket.Receive(buffer, buflen, SocketFlags.None);
                if (count < 0)
                {
                    throw new ApplicationException("read: channel EOF");
                }
                else if (count == 0)
                {
                    throw new DoneWithReadException();
                }
                else
                {
                    Array.Copy(buffer, 0, data, totalRead, count);
                    totalRead += count;
                }
            }

        }
        #endregion

        #region Read（从socket中接收数据输出到接收器中）

        /// <summary>
        /// 从socket中接收数据输出到接收器中
        /// </summary>
        public static void Read(Socket socket, AbstractOutputReceiver receiver, int bufSize)
        {
            socket.ReceiveBufferSize = bufSize;
            var data = new byte[bufSize];
            int count = -1;
            try
            {
                while (count != 0)
                {
                    count = socket.Receive(data);
                    if (count <= 0)
                    {
                        break;
                    }
                    receiver.Add(data, 0, count);
                }
            }
            catch (SocketException sex)
            {
                throw new ApplicationException(String.Format("No Data to read: {0}", sex.Message));
            }
            finally
            {
                receiver.Flush();
            }
        }

        #endregion

        #region ReadResponse（读取应答信息）
        /// <summary>
        /// 读取应答信息
        /// </summary>
        public static AdbResponse ReadResponse(Socket socket)
        {
            var reply = new byte[4];
            Read(socket, reply);
            AdbResponse res = new AdbResponse(reply.GetString(Encoding.UTF8));
            res.IsOkay = reply[0] == (byte)'O' && reply[1] == (byte)'K'
                         && reply[2] == (byte)'A' && reply[3] == (byte)'Y';
            return res;
        }
        #endregion

        #region 向adb socket发送数据

        /// <summary>
        /// 向adb socket发送数据（Writes the specified data to the specified socket.）
        /// </summary>
        public static void Write(Socket socket, byte[] data, int len = 0)
        {
            try
            {
                var count = socket.Send(data, len > 0 ? len : data.Length, SocketFlags.None);
                if (count < 0)
                {
                    throw new ApplicationException("ADB TCP Channel EOF");
                }
                else if (count == 0)
                {
                    throw new ApplicationException("ADB TCP Channel EOF");
                }
            }
            catch (SocketException)
            {
                throw;
            }
            catch (IOException)
            {
                throw;
            }
        }

        #endregion

        /// <summary>
        /// 创建发送文件的请求
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static byte[] CreateSendFileRequest(byte[] command, byte[] path)
        {
            //((int)((FileMode)0644) & 0777
            String modeString = String.Format(",{0}", 512);
            byte[] modeContent = null;
            try
            {
                modeContent = Encoding.Default.GetBytes(modeString);
            }
            catch (EncoderFallbackException)
            {
                return null;
            }

            byte[] array = new byte[8 + path.Length + modeContent.Length];
            Array.Copy(command, 0, array, 0, 4);
            (path.Length + modeContent.Length).Swap32bitsToArray(array, 4);
            Array.Copy(path, 0, array, 8, path.Length);
            Array.Copy(modeContent, 0, array, 8 + path.Length, modeContent.Length);
            return array;
        }

        /// <summary>
        /// 8字节请求：
        /// </summary>
        public static byte[] CreateRequest(byte[] command, int value)
        {
            byte[] array = new byte[8];
            Array.Copy(command, 0, array, 0, 4);
            value.Swap32bitsToArray(array, 4);
            return array;
        }

        /// <summary>
        /// 结构检测，返回true，结果一致。
        /// </summary>
        public static bool CheckResult(byte[] result, byte[] code)
        {
            if (result.Length >= code.Length)
            {
                for (int i = 0; i < code.Length; i++)
                {
                    if (result[i] != code[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 构建文件拷贝请求
        /// </summary>
        public static byte[] FormPullFileRequest(string target)
        {
            var pathbytes = target.ToBytes(Encoding.UTF8);
            var com = Encoding.UTF8.GetBytes("RECV");
            byte[] array = new byte[8 + pathbytes.Length];
            Array.Copy(com, 0, array, 0, 4);
            pathbytes.Length.Swap32bitsToArray(array, 4);
            Array.Copy(pathbytes, 0, array, 8, pathbytes.Length);
            return array;
        }
    }
}
