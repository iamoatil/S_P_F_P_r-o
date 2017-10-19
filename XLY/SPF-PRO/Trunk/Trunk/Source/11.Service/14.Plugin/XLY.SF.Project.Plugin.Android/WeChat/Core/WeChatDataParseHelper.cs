/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/11 14:36:58 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓微信数据解析辅助类
    /// </summary>
    internal static class WeChatDataParseHelper
    {
        /// <summary>
        /// 解密安卓微信数据库
        /// </summary>
        /// <param name="sqliteFile">要解密的数据库文件全路径</param>
        /// <returns>解密后数据库文件全路径</returns>
        public static string DecryptAndroidWeChatSqlite(string sqliteFile)
        {
            IntPtr wxHandle = IntPtr.Zero;

            try
            {
                // 打开数据库文件，获得文件句柄
                wxHandle = WXDeCryptedCoreDll.WXOpen(sqliteFile);
                if (IntPtr.Zero == wxHandle)
                {
                    LoggerManagerSingle.Instance.Error("解密安卓微信数据库时打开数据库文件失败！ 解密文件：" + sqliteFile);
                    return string.Empty;
                }

                // 解密后文件路径
                var fi = new FileInfo(sqliteFile);
                var newFile = Path.Combine(fi.DirectoryName, string.Format("Decrypt_{0}", fi.Name));

                //数据库解密
                var result = WXDeCryptedCoreDll.WXDeCryptedDBToFile(wxHandle, newFile);
                if (0 != result || !File.Exists(newFile))
                {
                    LoggerManagerSingle.Instance.Error(string.Format("解密安卓微信数据库时打开数据库文件失败！ 解密文件：{0} 错误码：{1}", sqliteFile, result));
                    return string.Empty;
                }
                else
                {
                    return newFile;
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "解密安卓微信数据库时出现异常！ 解密文件：" + sqliteFile);
            }
            finally
            {
                if (IntPtr.Zero != wxHandle)
                {
                    WXDeCryptedCoreDll.CloseHanle(ref wxHandle);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 解密安卓微信数据库，获取bak数据并保存到数据库
        /// </summary>
        /// <param name="sqliteFile">要解密的数据库文件全路径</param>
        /// <param name="deleteDataDbFile">bak数据保存数据库文件全路径</param>
        /// <returns>解密后数据库文件全路径</returns>
        public static string DecryptAndroidWeChatSqliteGetDelete(string sqliteFile, ref string deleteDataDbFile)
        {
            IntPtr wxHandle = IntPtr.Zero;
            string resultFile = string.Empty;

            try
            {
                // 打开数据库文件，获得文件句柄
                wxHandle = WXDeCryptedCoreDll.WXOpen(sqliteFile);
                if (IntPtr.Zero == wxHandle)
                {
                    LoggerManagerSingle.Instance.Error("解密安卓微信数据库时打开数据库文件失败！ 解密文件：" + sqliteFile);
                    return string.Empty;
                }

                // 解密后文件路径
                var fi = new FileInfo(sqliteFile);
                var newFile = Path.Combine(fi.DirectoryName, string.Format("Decrypt_{0}", fi.Name));

                //创建数据库存储数据
                var saveDbPath = sqliteFile + ".data.db";
                FileHelper.DeleteFileSafe(saveDbPath);

                var saveDb = new SqliteContext(saveDbPath);
                saveDb.ExecuteNonQuery("CREATE TABLE WxbakMsgdata (\"type\"  TEXT,\"isSend\"  TEXT, \"utctime\"  TEXT, \"wxid\"  TEXT,\"pmsg\"  BLOB);");
                saveDb.ExecuteNonQuery("CREATE INDEX wxidIndex ON WxbakMsgdata(wxid);");

                saveDb.UsingSafeTransaction(c =>
                {
                    //数据库解密
                    var result = WXDeCryptedCoreDll.WXDeCryptedDBToFileBakdata(wxHandle, newFile, pHandel =>
                    {
                        WxbakMsgdata data = (WxbakMsgdata)Marshal.PtrToStructure(pHandel, typeof(WxbakMsgdata));
                        var sb = new StringBuilder();

                        sb.Append("INSERT INTO WxbakMsgdata VALUES (");
                        sb.AppendFormat("'{0}',", data.type);
                        sb.AppendFormat("'{0}',", data.isSend);
                        sb.AppendFormat("'{0}',", data.utctime);
                        sb.AppendFormat("'{0}',", GetString(data.wxid, data.wxidByteLen));
                        sb.AppendFormat("@msgdata)");

                        c.CommandText = sb.ToString();
                        c.CommandType = CommandType.Text;
                        c.Parameters.Clear();
                        c.Parameters.Add(new System.Data.SQLite.SQLiteParameter("msgdata", GetByteArr(data.pmsg, data.msgbytelen)) { DbType = DbType.Binary });
                        c.ExecuteNonQuery();

                        return 0;
                    });

                    if (0 != result || !File.Exists(newFile))
                    {
                        LoggerManagerSingle.Instance.Error(string.Format("解密安卓微信数据库时打开数据库文件失败！ 解密文件：{0} 错误码：{1}", sqliteFile, result));
                    }
                    else
                    {
                        resultFile = newFile;
                    }
                });
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "解密安卓微信数据库时出现异常！ 解密文件：" + sqliteFile);
            }
            finally
            {
                if (IntPtr.Zero != wxHandle)
                {
                    WXDeCryptedCoreDll.CloseHanle(ref wxHandle);
                }
            }

            return resultFile;
        }

        private static string GetString(IntPtr pData, int length, Encoding encoding = null)
        {
            if (pData == IntPtr.Zero || length == 0)
            {
                return string.Empty;
            }

            if (null == encoding)
            {
                encoding = Encoding.UTF8;
            }

            byte[] arr = new byte[length];

            Marshal.Copy(pData, arr, 0, length);

            return encoding.GetString(arr);
        }

        private static byte[] GetByteArr(IntPtr pData, int length)
        {
            if (pData == IntPtr.Zero || length == 0)
            {
                return null;
            }

            byte[] arr = new byte[length];

            Marshal.Copy(pData, arr, 0, length);

            return arr;
        }

    }
}
