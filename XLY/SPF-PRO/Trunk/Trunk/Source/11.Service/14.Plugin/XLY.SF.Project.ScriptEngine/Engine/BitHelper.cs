using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* ==============================================================================
* Description：BitHelper  
* Author     ：fenghj
* Create Date：2016/12/20 15:47:39
* ==============================================================================*/

namespace XLY.SF.Project.ScriptEngine.Engine
{
    /// <summary>
    /// 脚本中添加对位(bit)的操作
    /// </summary>
    public class BitHelper
    {
        #region 整数和bytes的转换
        /// <summary>
        /// 字节数组转整数，
        /// isBigEndian=true表示高字节再前，低字节在后(比如[0x12,0x34]表示为0x1234；
        /// isBigEndian=false表示高字节再后，低字节在前(比如[0x12,0x34]表示为0x3412；
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="isBigEndian"></param>
        /// <returns></returns>
        public long ToInt(int[] blob, bool isBigEndian = false)
        {
            if (blob == null || blob.Length == 0)
                return 0;
            long l = blob[0];
            int i = 1;
            if (isBigEndian)  //isBigEndian=true表示高字节再前，低字节在后(比如[0x12,0x34]表示为0x1234；
            {
                while (i < blob.Length)
                {
                    l = (l << 8) + blob[i];
                    i++;
                }
            }
            else  //isBigEndian=false表示高字节再后，低字节在前(比如[0x12,0x34]表示为0x3412；
            {
                while (i < blob.Length)
                {
                    l = l + (blob[i] << (8 * i));
                    i++;
                }
            }
            return l;
        }

        /// <summary>
        /// 字节数组转可变长整数,
        /// 转换方法为：移除每一字节的最高bit，然后连接每个字节，最后结合为一个整数
        /// </summary>
        /// <param name="blob">字节数组</param>
        /// <param name="isFixedLength">true表示所有字节全部转换,false表示会自动判断可转换的长度(字节最高位为1表示转换，为0表示不继续转换)</param>
        /// <returns></returns>
        public int ToVariableInt(int[] blob, bool isFixedLength = true)
        {
            int length = blob.Length;
            if (!isFixedLength)   //自动判断可转换的长度(字节最高位为1表示转换，为0表示不继续转换)
            {
                length = 0;
                for (int i = 0; i < blob.Length; i++)
                {
                    length++;
                    if ((((byte) blob[i]) & 0x80) == 0)
                    {
                        break;
                    }
                }
            }

            int r = 0;
            for (int i = 0; i < length; i++)
            {
                byte b = (byte) (((byte) blob[i]) & 0x7F);
                r = r | (b << (7*i)); //最高位被移除，后面的自动补位
            }
            return r;
        }

        /// <summary>
        /// 整数转字节数组，由低到高，如0x123456将转换为[86,52,18,0]
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public byte[] ToBytes(int v)
        {
            return BitConverter.GetBytes(v);
        }

        /// <summary>
        /// 整形数据转换为bit,由低到高，如0x123456将转换为[0,1,1,0,1,0,1,0,0,0,1,0,1,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0]
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public byte[] ToBit(int v)
        {
            BitArray ba = new BitArray(new int[]{v});
            byte[] r = new byte[ba.Length];
            for (int i = 0; i < ba.Length; i++)
            {
                r[i] = (byte)(ba[i] ? 1 : 0);
            }
            return r;
        }

        #endregion

        #region 位操作
        /// <summary>
        /// 获取整数的某几位
        /// </summary>
        /// <param name="v">整数</param>
        /// <param name="bitIndex">起始bit位，0~31</param>
        /// <param name="bitLength">bit长度</param>
        /// <returns></returns>
        public int GetBit(int data, int bitIndex = 0, int bitLength = 1)
        {
            if (bitLength < 1 || bitIndex < 0 || bitIndex + bitLength > 31)
                return 0;
            return (data >> bitIndex) % (1 << bitLength);
        }

        /// <summary>
        /// 设置整形数据的某几位的值，比如SetBit(0xffffff, 0x34, 4, 8) = 0xfff34f
        /// </summary>
        /// <param name="data">整数</param>
        /// <param name="bitValue">设置的值</param>
        /// <param name="bitIndex">起始bit位，0~31</param>
        /// <param name="bitLength">bit长度</param>
        /// <returns></returns>
        public int SetBit(int data, int bitValue, int bitIndex = 0, int bitLength = 1)
        {
            if (bitLength < 1 || bitIndex < 0 || bitIndex + bitLength > 31)
                return 0;
            int markbit = (int)(((0xffffffff) << (bitIndex + bitLength)) | ((0xffffffff) >> (32 - bitIndex)));
            return (data & markbit) | ((bitValue << bitIndex) & (~markbit));
        }

        /// <summary>
        /// 反转整形数据的bit位，比如ReverseBit(0xffffff, 4, 8) = 0xff00ff
        /// </summary>
        /// <param name="data">整数</param>
        /// <param name="bitIndex">起始bit位，0~31</param>
        /// <param name="bitLength">bit长度</param>
        /// <returns></returns>
        public int ReverseBit(int data, int bitIndex = 0, int bitLength = 1)
        {
            if (bitLength < 1 || bitIndex < 0 || bitIndex + bitLength > 31)
                return 0;
            int v = GetBit(data, bitIndex, bitLength);
            return SetBit(data, ~v, bitIndex, bitLength);
        }
        #endregion
    }
}
