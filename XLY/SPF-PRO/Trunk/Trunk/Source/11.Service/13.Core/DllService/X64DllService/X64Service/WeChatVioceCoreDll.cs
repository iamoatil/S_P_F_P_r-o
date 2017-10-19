/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/29 17:35:50 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace X64Service
{
    /// <summary>
    /// 语音解码接口
    /// </summary>
    public static class WeChatVioceCoreDll
    {
        private const string _dllFile = @"bin\audio_dec_krnl.dll";

        /// <summary>
        /// 装载音频文件
        /// </summary>
        /// <param name="filename">原始音频文件路径</param>
        /// <param name="audiotype">音频类型</param>
        /// <returns>装载句柄</returns>
        [DllImport(_dllFile, EntryPoint = "audio_dec_fun_0")]
        public static extern IntPtr Mount(string filename, uint audiotype);

        /// <summary>
        /// 卸载装载句柄
        /// </summary>
        /// <param name="mount">装载音频文件获取到的句柄</param>
        [DllImport(_dllFile, EntryPoint = "audio_dec_fun_1")]
        public static extern void UnMount(ref IntPtr mount);

        /// <summary>
        /// 音频解码
        /// </summary>
        /// <param name="mount">音频解码句柄</param>
        /// <param name="intPtr">输出解码文件名路径,底层是char[]</param>
        /// <param name="dir">存放解码文件目录</param>
        /// <returns>返回状态(0：成功,other：错误码)</returns>
        [DllImport(_dllFile, EntryPoint = "audio_dec_fun_2")]
        public static extern int Decode(IntPtr mount, byte[] intPtr, string dir = null);

        /// <summary>
        /// 控制解码行为操作
        /// </summary>
        /// <param name="mount">装载音频文件获取到的句柄</param>
        /// <param name="status">操作状态: 1 - 继续执行, 2 - 暂停, 3 - 停止</param>
        [DllImport(_dllFile, EntryPoint = "audio_dec_fun_4")]
        public static extern void SetStatus(IntPtr mount, int status);
    }

}
