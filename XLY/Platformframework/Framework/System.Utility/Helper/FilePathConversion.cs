using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Utility.Logger;

namespace System.Utility.Helper
{
    /// <summary>
    /// Windows长路径及特殊字符字符处理
    /// @Copy XLY
    /// @Author luochao
    /// @Date 20160627
    /// </summary>
    public class FilePathConversion
    {
        //指定的路径或文件名太长，或者两者都太长。完全限定文件名必须少于 260 个字符，并且目录名必须少于 248 个字符。
        private const int _FILEPATH_MAX_PATH = 260;
        private const int _DIRPATH_MAX_PATH = 248;

        /// <summary>
        /// 获取指定文件的短路径名
        /// 该API不适合对畸形文件夹进行操作
        /// </summary>
        /// <param name="lpszLongPath">输入：指定欲获取短路径名的那个文件的名字。可以是个完整路径，或者由当前目录决定</param>
        /// <param name="lpszShortPath">输出：指定一个缓冲区，用于装载文件的短路径和文件名</param>
        /// <returns>转换是否成功，true成功，反之亦然</returns>
        [DllImport("FilePathConversion.dll", EntryPoint = "GetShortPathName_EX", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static bool GetShortPathName_EX(string lpszLongPath, [Out] StringBuilder lpszShortPath);

        /// <summary>
        /// 获取指定文件的长路径名
        /// 该API不适合对畸形文件夹进行操作
        /// </summary>
        /// <param name="lpszShortPath">输入：指定欲获取长路径名的那个文件的名字</param>
        /// <param name="lpszLongPath">输出：指定一个缓冲区，用于装载文件的长路径和文件名</param>
        /// <returns>转换是否成功，true成功，反之亦然</returns>
        [DllImport("FilePathConversion.dll", EntryPoint = "GetLongPathName_EX", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static bool GetLongPathName_EX(string lpszShortPath, [Out] StringBuilder lpszLongPath);

        /// <summary>
        /// 获取指定文件的短路径名
        /// </summary>
        /// <param name="lpszLongPath">输入：指定欲获取短路径名的那个文件的名字。可以是个完整路径，或者由当前目录决定</param>
        /// <param name="IsFolder">输入：是否是文件夹（true是，反之亦然）</param>
        /// <returns>如果传入路径为长路径，且合法，则返回短路劲，反之亦然</returns>
        public static string GetShortPathName(string lpszLongPath, bool IsFolder)
        {
            try
            {
                //测试用
                //lpszLongPath = "D:\\XLYSFTasks\\任务-2016-06-28-14-32-58\\source\\data\\com.android.providers.contacts\\com.android.providers.contacts\\com.android.providers.contacts\\com.android.providers.contacts\\com.android.providers.contacts\\com.android.providers.contacts\\com.android.providers.contacts\\com.android.providers.contacts\\com.android.providers.contacts\\com.android.providers.contacts\\com.android.providers.contacts\\databases\\profile.db-journal";
                if (string.IsNullOrEmpty(lpszLongPath))
                {
                    //LogHelper.Debug(string.Format("GetShortPathName check[{0}]", lpszLongPath));
                    return lpszLongPath;
                }
                string lpszLongPath_temp = lpszLongPath;

                /*
                 * 目录创建：
                 * 目录名长度超过248 Windows就无法创建，且在248~254之间无法进行短路径转换，超过255就可以进行短路径转换
                 * 因此，
                 * 在248~254这个区间使用java进行原路径创建并返回，
                 * 超过255的由java进行代替路径创建，后通过c++转短路径处理后返回，由C#进行目录名路径创建
                 * 
                 * 完全限定文件名创建：
                 * 完全限定文件名长度超过260 Windows就无法创建
                 * 因此，完全限定文件名超过260的由java进行替代路径创建，后通过c++转短路径处理后返回，由C#进行目录名路径创建
                 */
                if (IsFolder && (lpszLongPath_temp.Length >= 248 && lpszLongPath_temp.Length < 254))
                {
                    string strCurrentDirectory = System.Environment.CurrentDirectory;
                    string comd1 = string.Format(@"cd /d {0}", strCurrentDirectory);    //命令1：切换到执行目录
                    string comd2 = string.Format(@"{0}\jre1.8.0_91\bin\java JFile {1}", strCurrentDirectory, lpszLongPath_temp);    //命令2：调用java在jre环境下创建长路径
                    int _count = 0;
                    do
                    {
                        bool rexFlag = false;
                        do
                        {
                            if (_count >= 5) break;

                            // dos调用java创建长路径
                            rexFlag = ExeCommand(comd1, comd2);
                            System.Threading.Thread.Sleep(2000);
                            ++_count;
                        } while (!rexFlag);
                    } while (!System.IO.Directory.Exists(lpszLongPath_temp));
                    return lpszLongPath_temp;
                }
                else if ((IsFolder && lpszLongPath.Length >= 254) || (!IsFolder && lpszLongPath.Length >=260))
                {
                    // 构建java创建替代目录
                    int starIndex = lpszLongPath_temp.IndexOf(@"\source\");
                    if (starIndex != -1)
                        lpszLongPath_temp = string.Format(@"{0}\lpaths\{1}", lpszLongPath_temp.Substring(0, starIndex), lpszLongPath_temp.Substring(starIndex + 8));

                    string strCurrentDirectory = System.Environment.CurrentDirectory;
                    string comd1 = string.Format(@"cd /d {0}", strCurrentDirectory);    //命令1：切换到执行目录
                    string comd2 = string.Format(@"{0}\jre1.8.0_91\bin\java JFile {1}", strCurrentDirectory, lpszLongPath_temp);    //命令2：调用java在jre环境下创建长路径

                    StringBuilder outShortPath = new StringBuilder(lpszLongPath_temp.Length);
                    bool brst = false;
                    int _count = 0;
                    do
                    {
                        bool rexFlag = false;
                        do
                        {
                            if (_count >= 5) break;

                            // dos调用java创建长路径
                            rexFlag = ExeCommand(comd1, comd2);
                            System.Threading.Thread.Sleep(2000);
                            ++_count;
                        } while (!rexFlag);
                        /*
                         * 调用c++传入长路径返回短路径
                         * 返回短路径格式：\\\\?\\D:\\XLYSFT~1\\任F796~2\\lpath\\data\\COMAND~1.CON\\COMAND~1.CON\\COMAND~1.CON\\COMAND~1.CON\\COMAND~1.CON\\COMAND~1.CON\\COMAND~1.CON\\COMAND~1.CON\\COMAND~1.CON\\COMAND~1.CON\\COMAND~1.CON\\DATABA~1\\PROFIL~1.DB-
                         */
                        brst = GetShortPathName_EX(lpszLongPath_temp.StartsWith("\\\\?\\") ? lpszLongPath_temp : string.Format("\\\\?\\{0}", lpszLongPath_temp), outShortPath);
                    } while (!brst);

                    //前缀处理后格式：D:\XLYSFT~1\任F796~2\lpath\data\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\DATABA~1\PROFIL~1.DB-
                    string retrunShortPath = outShortPath.ToString().StartsWith("\\\\?\\") ? outShortPath.ToString().Remove(0, "\\\\?\\".Length) : outShortPath.ToString();
                    int tempStartIndex = retrunShortPath.ToString().IndexOf(@"\lpaths\");
                    if (tempStartIndex != -1)
                    {
                        //替换后格式：D:\XLYSFT~1\任F796~2\source\data\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\COMAND~1.CON\DATABA~1\PROFIL~1.DB-
                        retrunShortPath = string.Format(@"{0}\source\{1}", retrunShortPath.Substring(0, tempStartIndex), retrunShortPath.Substring(tempStartIndex + 8));
                        return retrunShortPath;
                    }
                }
                return lpszLongPath;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("GetShortPathName error[{0}]", lpszLongPath), ex);
                return lpszLongPath;
            }
        }

        /// <summary>
        /// 获取指定文件的长路径名
        /// </summary>
        /// <param name="lpszShortPath">输入：指定欲获取短路径名的那个文件的名字。可以是个完整路径，或者由当前目录决定</param>
        /// <returns>如果传入路径为短路径，且合法，则返回长路劲，反之亦然</returns>
        public static string GetLongPathName(string lpszShortPath)
        {
            try
            {
                if (string.IsNullOrEmpty(lpszShortPath) || lpszShortPath.Length >= _FILEPATH_MAX_PATH)
                {
                    LogHelper.Debug(string.Format("GetLongPathName check[{0}]", lpszShortPath));
                    return lpszShortPath;
                }

                StringBuilder outLongPath = new StringBuilder(lpszShortPath.Length);
                bool brst = GetLongPathName_EX(lpszShortPath.StartsWith("\\\\?\\") ? lpszShortPath : string.Format("\\\\?\\{0}", lpszShortPath), outLongPath);
                if (!brst)
                {
                    LogHelper.Debug(string.Format("GetLongPathName failue[{0}]", lpszShortPath));
                    return lpszShortPath.StartsWith("\\\\?\\") ?
                         lpszShortPath.Remove(0, "\\\\?\\".Length) :
                         lpszShortPath;
                }
                else
                {
                    LogHelper.Debug(string.Format("GetLongPathName success[{0}]", outLongPath));
                    return outLongPath.ToString().StartsWith("\\\\?\\") ?
                        outLongPath.ToString().Remove(0, "\\\\?\\".Length) :
                        outLongPath.ToString();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("GetLongPathName error[{0}]", lpszShortPath), ex);
                return lpszShortPath;
            }
        }

        /// <summary>
        /// 执行dos命令
        /// </summary>
        /// <param name="comds">输入：命令集合</param>
        /// <returns>布尔类型，是否成功，true成功，反之亦然</returns>
        public static bool ExeCommand(params string[] comds)
        {
            if (null == comds || comds.Length < 1) return false;
            try
            {
                //创建进程对象 
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "cmd.exe";           //设定需要执行的命令 
                    p.StartInfo.UseShellExecute = false;        //不使用系统外壳程序启动 
                    p.StartInfo.RedirectStandardInput = true;   //可以重定向输入  
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;          //不创建窗口 
                    p.Start();
                    foreach (var comd in comds)
                    {
                        p.StandardInput.WriteLine(comd);
                    }
                }
            }
            catch// (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 执行DOS命令，返回DOS命令的输出
        /// dos命令
        /// 等待命令执行的时间（单位：毫秒），如果设定为0，则无限等待
        /// 返回输出，如果发生异常，返回空字符串
        /// </summary>
        /// <param name="dosCommand">命令文本</param>
        /// <param name="milliseconds">等待关联进程退出的时间（以毫秒为单位）</param>
        /// <returns></returns>
        public static string[] ExeCommand(string dosCommand, int milliseconds = 0)
        {
            string[] returnValues = new string[2] { "0", "OK" };
            if (!dosCommand.IsValid())
            {
                returnValues[0] = "-1";
                returnValues[1] = "命令文本为空";
                return returnValues;
            }

            using (Process process = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";                     //设定需要执行的命令
                startInfo.Arguments = "/C " + dosCommand;           //设定参数，其中的“/C”表示执行完命令后马上退出
                startInfo.UseShellExecute = false;                  //不使用系统外壳程序启动
                startInfo.RedirectStandardInput = false;            //不重定向输入
                startInfo.RedirectStandardOutput = true;            //重定向输出
                startInfo.CreateNoWindow = true;                    //不创建窗口
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())                            //开始进程
                    {
                        if (milliseconds == 0)
                        {
                            process.WaitForExit();                  //这里无限等待进程结束
                        }
                        else
                        {
                            process.WaitForExit(milliseconds);      //这里等待进程结束，等待时间为指定的毫秒
                        }
                        returnValues[1] =  process.StandardOutput.ReadToEnd();//读取进程的输出
                    }
                }
                catch (Exception e)
                {
                    returnValues[0] = "-1";
                    returnValues[1] = "执行DOS命令，返回DOS命令的输出异常：" + e.Message;
                    LogHelper.Error("执行DOS命令，返回DOS命令的输出异常：", e);
                }
            }
            return returnValues;
        }
    }
}
