/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/16 11:02:32 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace X64Service
{
    /// <summary>
    /// 安卓微信数据库解密DLL
    /// </summary>
    public static class WXDeCryptedCoreDll
    {
        private const string _DllName = @"bin\WXDecrypted.dll";

        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="pdbFilePath">数据库文件路径</param>
        /// <returns></returns>
        [DllImport(_DllName, EntryPoint = "WX_open")]
        public static extern IntPtr WXOpen(string pdbFilePath);

        /// <summary>
        /// 解密数据库并保存到指定文件
        /// </summary>
        /// <param name="wxHandle">打开的要解密的数据库句柄</param>
        /// <param name="pfilePath">解密后数据库保存路径</param>
        /// <returns></returns>
        [DllImport(_DllName, EntryPoint = "WX_deCryptedDBToFile")]
        public static extern int WXDeCryptedDBToFile(IntPtr wxHandle, string pfilePath);

        /// <summary>
        /// 解密数据库保存到指定文件并读取bak文件数据
        /// </summary>
        /// <param name="wxHandle">打开的要解密的数据库句柄</param>
        /// <param name="pfilePath">解密后数据库保存路径</param>
        /// <param name="callback">bak文件数据解析回调</param>
        /// <returns></returns>
        [DllImport(_DllName, EntryPoint = "WX_deCryptedDBToFileBakdata")]
        public static extern int WXDeCryptedDBToFileBakdata(IntPtr wxHandle, string pfilePath, WXBakdataCallBack callback);

        /// <summary>
        /// 关闭数据库
        /// </summary>
        /// <param name="wxHandle">打开的数据库句柄</param>
        /// <returns></returns>
        [DllImport(_DllName, EntryPoint = "WX_CloseHanle")]
        public static extern int CloseHanle(ref IntPtr wxHandle);

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WxbakMsgdata
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public int type;//消息类型
        /// <summary>
        /// 是否是发送还是接受
        /// </summary>
        public Byte isSend;//是否是发送还是接受
        /// <summary>
        /// utc时间
        /// </summary>
        public UInt64 utctime;// utc时间
        /// <summary>
        /// wxid数据
        /// </summary>
        public IntPtr wxid;//id数据
        /// <summary>
        /// wxid字节长度
        /// </summary>
        public UInt16 wxidByteLen;//wxid字节长度
        /// <summary>
        /// msg消息  utf8编码
        /// </summary>
        public IntPtr pmsg;//msg消息  utf8编码
        /// <summary>
        /// msg消息字节长度
        /// </summary>
        public UInt16 msgbytelen;//msg字节长度
    }

    public delegate int WXBakdataCallBack(IntPtr pData);

}
