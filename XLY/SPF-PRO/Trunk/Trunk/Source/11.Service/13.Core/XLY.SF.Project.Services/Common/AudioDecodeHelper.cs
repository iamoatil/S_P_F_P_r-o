/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/29 17:38:39 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 语音文件解码
    /// </summary>
    public static class AudioDecodeHelper
    {
        /// <summary>
        /// 语音文件解码
        /// </summary>
        /// <param name="voiceFilePath">源文件全路径</param>
        /// <param name="savePath">保存路径，默认为NULL，保存在源路径</param>
        /// <returns>解码后文件全路径</returns>
        public static string Decode(string voiceFilePath, string savePath = null)
        {
            if (!FileHelper.IsValid(voiceFilePath))
            {
                return voiceFilePath;
            }

            IntPtr mount = IntPtr.Zero;
            // 语音转码
            try
            {
                mount = WeChatVioceCoreDll.Mount(voiceFilePath, 0xAC00);
                if (mount == IntPtr.Zero)
                {
                    return voiceFilePath;
                }

                var fileDes = new byte[1024];

                var res = WeChatVioceCoreDll.Decode(mount, fileDes, savePath);
                if (res == 0)
                {
                    var resFile = Encoding.Default.GetString(fileDes).TrimEnd('\0');
                    if (resFile.IsValid())
                    {
                        return resFile;
                    }
                    else
                    {
                        return voiceFilePath.TrimEnd(FileHelper.GetExtension(voiceFilePath)) + "wav";
                    }
                }
                else
                {
                    return voiceFilePath;
                }
            }
            catch
            {
                return voiceFilePath;
            }
            finally
            {
                if (mount != IntPtr.Zero)
                {
                    WeChatVioceCoreDll.UnMount(ref mount);
                }
            }
        }

    }

}
