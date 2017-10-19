using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32.SafeHandles;

/* ==============================================================================
* Description：脚本中对二进制数据的处理帮助类  
* Author     ：ly
* Create Date：2016/11/29 17:28:54
* ==============================================================================*/

namespace XLY.SF.Project.ScriptEngine.Engine
{
    /// <summary>
    /// 脚本中对二进制数据的处理帮助类
    /// </summary>
    public class BlobHelper : IDisposable
    {
        #region 字节数组和字符串转换
        /// <summary>
        /// 字符串转字节数组，默认为UTF-8编码
        /// </summary>
        /// <param name="content">需要转换的字符串</param>
        /// <returns></returns>
        public byte[] ToBytes(string content)
        {
            return ToBytes(content, "utf-8");
        }

        /// <summary>
        /// 字符串转字节数组，编码格式自定义，比如"utf-8"、"ascii"、"base64"
        /// </summary>
        /// <param name="content">需要转换的字符串</param>
        /// <param name="code">编码格式,比如"utf-8"、"ascii"</param>
        /// <returns></returns>
        public byte[] ToBytes(string content, string code)
        {
            if (code.Equals("base64", StringComparison.OrdinalIgnoreCase))
            {
                return System.Convert.FromBase64String(content);
            }
            else
            {
                Encoding c = Encoding.GetEncoding(code);
                byte[] res = c.GetBytes(content);
                return res;
            }
        }

        /// <summary>
        /// 字节数组转字符串，默认为UTF-8编码
        /// </summary>
        /// <param name="blob">需要转换的字节数组</param>
        /// <returns></returns>
        public string ToString(int[] blob)
        {
            return ToString(blob, "utf-8");
        }

        /// <summary>
        /// 字节数组转字符串，编码格式自定义，比如"utf-8"、"ascii"、"base64"
        /// </summary>
        /// <param name="blob">需要转换的字节数组</param>
        /// <param name="code">编码格式,比如"utf-8"、"ascii"</param>
        /// <returns></returns>
        public string ToString(int[] blob, string code)
        {
            if (code.Equals("base64", StringComparison.OrdinalIgnoreCase))
            {
                return System.Convert.ToBase64String(blob.ToList().ConvertAll(s => (byte) s).ToArray());
            }
            else
            {
                Encoding c = Encoding.GetEncoding(code);
                return c.GetString(blob.ToList().ConvertAll(s => (byte) s).ToArray());
            }
        }
        #endregion

        #region 操作字节数组
        /// <summary>
        /// 从字节数组中截取子数组
        /// </summary>
        /// <param name="blob">源数组</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public int[] GetBytes(int[] blob, int offset, int length)
        {
            if (offset < 0)
            {
                offset = 0;
            }
            else if (offset + length > blob.Length)       //超过文件长度
            {
                return new int[0];
            }
            int[] bs2 = new int[length];
            Array.Copy(blob, offset, bs2, 0, bs2.Length);
            return bs2;
        }

        /// <summary>
        /// 从字节数组中查询子数组，未找到返回-1
        /// </summary>
        /// <param name="blob">源数组</param>
        /// <param name="offset">起始查询偏移</param>
        /// <param name="subBytes">子数组</param>
        /// <returns>查询到则返回序号，否则返回-1</returns>
        public int FindBytes(int[] blob, int offset, int[] subBytes)
        {
            return GetIndexOf<int,int>(blob, offset, subBytes);
        }

        /// <summary>
        /// 从字节数组中截取子数组
        /// </summary>
        /// <param name="blob">源数组</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public int[] GetBytes(byte[] blob, int offset, int length)
        {
            if (offset < 0)
            {
                offset = 0;
            }
            else if (offset + length > blob.Length)       //超过文件长度
            {
                return new int[0];
            }
            int[] bs2 = new int[length];
            Array.Copy(blob, offset, bs2, 0, bs2.Length);
            return bs2;
        }

        /// <summary>
        /// 从字节数组中查询子数组，未找到返回-1
        /// </summary>
        /// <param name="blob">源数组</param>
        /// <param name="offset">起始查询偏移</param>
        /// <param name="subBytes">子数组</param>
        /// <returns>查询到则返回序号，否则返回-1</returns>
        public int FindBytes(byte[] blob, int offset, byte[] subBytes)
        {
            return GetIndexOf<byte,byte>(blob, offset, subBytes);
        }

        /// <summary>
        /// 反转某个数组，比如[0x01,0x02,0x03]将反转为[0x03,0x02,0x01]
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        public int[] ReverseArray(int[] blob)
        {
            Array.Reverse(blob);
            return blob;
        }

        /// <summary>
        /// 反转某个数组，比如[0x01,0x02,0x03]将反转为[0x03,0x02,0x01]
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public int[] ReverseArray(int[] blob, int offset, int length)
        {
            Array.Reverse(blob, offset, length);
            return blob;
        }
        #endregion

        #region 二进制操作文件

        private Dictionary<int, FileStream> _dicStreams = new Dictionary<int, FileStream>();

        /// <summary>
        /// 打开文件并获取文件句柄，文件操作结束后需要调用CloseFileHandle关闭文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public int GetFileHandle(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                int handle = fs.SafeFileHandle.DangerousGetHandle().ToInt32();
                _dicStreams[handle] = fs;
                return handle;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public int GetFileSizeFromHandle(int handle)
        {
            FileStream fs = GetFileStream(handle);
            return (int)fs.Length;
        }

        /// <summary>
        /// 获取文件的某段字节内容
        /// </summary>
        /// <param name="handle">文件句柄</param>
        /// <param name="offset">起始偏移</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public byte[] GetBytesFromHandle(int handle, int offset, int length)
        {
            FileStream fs = GetFileStream(handle);
            if (offset < 0)
            {
                offset = 0;
            }
            else if (offset + length > fs.Length)       //超过文件长度
            {
                return new byte[0];
            }
            fs.Seek(offset, SeekOrigin.Begin);
            byte[] bs = new byte[length];
            int r = fs.Read(bs, 0, length);
            if (r == bs.Length)
                return bs;
            byte[] bs2 = new byte[r];
            Array.Copy(bs, 0, bs2, 0, bs2.Length);
            return bs2;
        }

        /// <summary>
        /// 在文件中查询字节序列，成功返回起始序号，否则返回-1
        /// </summary>
        /// <param name="handle">文件句柄</param>
        /// <param name="offset">起始偏移</param>
        /// <param name="subBytes">子序列</param>
        /// <returns></returns>
        public int FindBytesFromHandle(int handle, int offset, int[] subBytes)
        {
            FileStream fs = GetFileStream(handle);
            if (offset < 0)
            {
                offset = 0;
            }
            else if (offset >= fs.Length)       //超过文件长度
            {
                return -1;
            }
            fs.Seek(offset, SeekOrigin.Begin);
            int crossLength = 0;
            byte[] buff = new byte[subBytes.Length < 512 ? 1024 : subBytes.Length * 2];   //最少读取1K，且为bb的大小的2倍

            while (true)
            {
                //将最后几个字节复制到头部，是为了查询2次读取交叉的部分
                if (crossLength > 0)
                {
                    Array.Copy(buff, buff.Length - crossLength, buff, 0, crossLength);
                }
                int r = fs.Read(buff, crossLength, buff.Length - crossLength);      //读取文件字节，并添加到末尾
                if (r <= 0)
                    break;
                int index = GetIndexOf<byte,int>(buff, 0, subBytes);
                if (index >= 0)     //找到了位置
                {
                    if (crossLength == 0)       //在第一次就查询到这直接返回字节序号
                    {
                        return index + offset;
                    }
                    return (int)fs.Position - r + index - crossLength;
                }
                crossLength = subBytes.Length;
            }
            return -1;
        }

        /// <summary>
        /// 关闭文件句柄
        /// </summary>
        /// <param name="handle"></param>
        public void CloseFileHandle(int handle)
        {
            FileStream fs = GetFileStream(handle);
            try
            {
                fs.Close();
                _dicStreams.Remove(handle);
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// 文件句柄转换为FileStream
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private FileStream GetFileStream(int handle)
        {
            //try
            //{
            //    IntPtr ptr = new IntPtr(handle);
            //    FileStream fs = new FileStream(new SafeFileHandle(ptr, true), FileAccess.Read);
            //    return fs;
            //}
            //catch (Exception)
            //{
            //    throw new Exception(LanguageHelper.Get("LANGKEY_WenJianJuBingWuXiaohandle_04595") + handle);
            //}
            if (_dicStreams.ContainsKey(handle))
            {
                return _dicStreams[handle];
            }
            else
            {
                throw new Exception("Invalid file handle" + handle);
            }
        }
        #endregion

        #region 帮助类

        /// <summary>
        /// 查询子数组
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        /// <param name="subBytes"></param>
        /// <returns></returns>
        private int GetIndexOf<T1,T2>(T1[] buf, int offset, T2[] subBytes) where T1:IComparable
        {
            if (buf == null || subBytes == null || buf.Length == 0 || subBytes.Length == 0 || buf.Length < subBytes.Length)
                return -1;
            if (offset < 0)
            {
                offset = 0;
            }
            if (offset + subBytes.Length > buf.Length)
                return -1;

            int i, j;
            for (i = offset; i < buf.Length - subBytes.Length + 1; i++)
            {
                if (buf[i].CompareTo(subBytes[0]) == 0)
                {
                    for (j = 1; j < subBytes.Length; j++)
                    {
                        if (buf[i + j].CompareTo(subBytes[j])!=0)
                            break;
                    }
                    if (j == subBytes.Length)
                        return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查询子数组
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="offset"></param>
        /// <param name="subBytes"></param>
        /// <returns></returns>
        //private int GetIndexOf(int[] buf, int offset, int[] subBytes)
        //{
        //    if (buf == null || subBytes == null || buf.Length == 0 || subBytes.Length == 0 || buf.Length < subBytes.Length)
        //        return -1;
        //    if (offset < 0)
        //    {
        //        offset = 0;
        //    }
        //    if (offset + subBytes.Length >= buf.Length)
        //        return -1;

        //    int i, j;
        //    for (i = 0; i < buf.Length - subBytes.Length + 1; i++)
        //    {
        //        if (buf[i] == subBytes[0])
        //        {
        //            for (j = 1; j < subBytes.Length; j++)
        //            {
        //                if (buf[i + j] != subBytes[j])
        //                    break;
        //            }
        //            if (j == subBytes.Length)
        //                return i;
        //        }
        //    }
        //    return -1;
        //}

        #endregion

        public void Dispose()
        {
            foreach (var fs in _dicStreams)
            {
                try
                {
                    fs.Value.Close();
                }
                catch (Exception)
                {

                }
            }

            _dicStreams.Clear();
        }
    }
}
