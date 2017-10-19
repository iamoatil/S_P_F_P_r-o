using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Log4NetService;

namespace XLY.SF.Project.Services
{
    public class DeCryptedDBHelper
    {
        /// <summary>
        /// 安卓微信数据库解密
        /// </summary>
        /// <param name="wechatFile">源文件</param>
        /// <param name="decryptDbPathName">解密后保存路径</param>
        /// <returns></returns>
        public static int DecryptAndroidWeChatSqlite(string wechatFile, string decryptDbPathName)
        {
            var result = -1;
            IntPtr wxHandle = IntPtr.Zero;
            try
            {
                // 打开数据库文件，获得文件句柄
                wxHandle = WXOpen(wechatFile);
                if (wxHandle == IntPtr.Zero)
                {
                    LoggerManagerSingle.Instance.Error("微信数据库打开失败");
                    return result;
                }

                string temppath = new FileInfo(decryptDbPathName).DirectoryName;
                if (!Directory.Exists(temppath))
                {
                    Directory.CreateDirectory(temppath);
                }

                //解密文件
                result = WXDeCryptedDBToFile(wxHandle, decryptDbPathName);
                if (result != 0)
                {
                    LoggerManagerSingle.Instance.Error(string.Format("微信数据库解密失败, 错误码：{0}", result));
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = -1;
                LoggerManagerSingle.Instance.Error(string.Format("微信数据库解密异常, 错误信息：{0}", ex));
            }
            finally
            {
                if (wxHandle != IntPtr.Zero)
                {
                    // 句柄释放
                    CloseHanle(ref wxHandle);
                }
            }

            return result;
        }

        [DllImport("WXDecrypted.dll", EntryPoint = "WX_open")]
        private static extern IntPtr WXOpen(string pdbFilePath);

        [DllImport("WXDecrypted.dll", EntryPoint = "WX_setKey")]
        private static extern int WXSetKey(IntPtr wxHandle, string pkeyValue);

        [DllImport("WXDecrypted.dll", EntryPoint = "WX_deCryptedDB")]
        private static extern int WXDeCryptedDB(IntPtr wxHandle);

        [DllImport("WXDecrypted.dll", EntryPoint = "WX_deCryptedDBToFile")]
        private static extern int WXDeCryptedDBToFile(IntPtr wxHandle, string pfilePath);

        [DllImport("WXDecrypted.dll", EntryPoint = "WX_saveTofile")]
        private static extern int WXSaveTofile(IntPtr wxHandle, string pfilePath);

        [DllImport("WXDecrypted.dll", EntryPoint = "WX_CloseHanle")]
        private static extern int CloseHanle(ref IntPtr wxHandle);
    }
}
