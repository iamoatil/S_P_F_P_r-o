using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Services.ADB
{
    public static class AdbSocketHelper
    {
        #region FormAdbRequest

        /// <summary>
        /// 构建adb请求的byte数据（Forms the adb request.）
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns></returns>
        public static byte[] FormAdbRequest(string req)
        {
            string resultStr = string.Format("{0}{1}\n", req.Length.ToString("X4"), req);
            try
            {
                return Encoding.UTF8.GetBytes(resultStr);
            }
            catch (EncoderFallbackException efe)
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// 构建文件拷贝请求
        /// </summary>
        public static byte[] FormPullFileRequest(string target)
        {
            var pathbytes = Encoding.UTF8.GetBytes(target);
            var com = Encoding.UTF8.GetBytes("RECV");
            byte[] array = new byte[8 + pathbytes.Length];
            Array.Copy(com, 0, array, 0, 4);
            pathbytes.Length.Swap32bitsToArray(array, 4);
            Array.Copy(pathbytes, 0, array, 8, pathbytes.Length);
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

        public static void Swap32bitsToArray(this int value, byte[] dest, int offset)
        {
            dest[offset] = (byte)(value & 0x000000FF);
            dest[offset + 1] = (byte)((value & 0x0000FF00) >> 8);
            dest[offset + 2] = (byte)((value & 0x00FF0000) >> 16);
            dest[offset + 3] = (byte)((value & 0xFF000000) >> 24);
        }

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

        #region Write

        /// <summary>
        /// 向adb socket发生数据（Writes the specified data to the specified socket.）
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
            catch (SocketException sex)
            {
                throw;
            }
            catch (IOException e)
            {
                throw;
            }
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

        #region ReadFile（从socket中接收文件输出）

        /// <summary>
        /// 从socket中接收文件输出
        /// </summary>
        public static void ReadFile(Socket socket, PullFileReceiver receiver)
        {
            var data = new byte[16 * 1024];
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
        public static string GetString(this byte[] bytes, System.Text.Encoding encode)
        {
            if (bytes == null || bytes.Count() <= 0)
            {
                return string.Empty;
            }
            return encode.GetString(bytes);
        }
        #endregion
        

    }
}
