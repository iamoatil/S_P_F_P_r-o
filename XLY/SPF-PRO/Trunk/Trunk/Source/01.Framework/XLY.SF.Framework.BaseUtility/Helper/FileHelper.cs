using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Log4NetService;

namespace XLY.SF.Project.BaseUtility.Helper
{
    public class FileHelper
    {
        #region 字段定义
        /// <summary>
        /// 同步标识
        /// </summary>
        private static readonly Object _sync = new object();
        /// <summary>
        /// windows中非法路径字符
        /// </summary>
        private static readonly char[] InvalidPathChars = new char[]
                                                              {
                                                                  '"', '<', '>', '|', '\0', '\x0001', '\x0002', '\x0003'
                                                                  , '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n'
                                                                  , '\v','\f', '\r', '\x000e', '\x000f', '\x0010', '\x0011',
                                                                  '\x0012', '\x0013', '\x0014', '\x0015', '\x0016',
                                                                  '\x0017', '\x0018', '\x0019', '\x001a', '\x001b',
                                                                  '\x001c', '\x001d', '\x001e', '\x001f', '*', '?', ':'
                                                              };

        #endregion

        #region 检测文件名是否合法
        /// <summary>
        /// 判断文件是否可用，字符为空、文件不存在，文件大小为0都 表示文件不可用。
        /// true表示文件合法可用。
        /// </summary>
        public static bool IsValid(string file)
        {
            if (String.IsNullOrEmpty(file))
                return false;
            if (!File.Exists(file))
                return false;
            FileInfo info = new FileInfo(file);
            if (info.Length <= 0)
                return false;
            return true;
        }
        #endregion

        #region 是否合法的文件夹

        /// <summary>
        /// 是否合法的文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsValidDictory(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return false;
            }
            else
            {
                return Directory.Exists(path);
            }
        }

        #endregion

        #region 读取文件

        public static String FileToUTF8String(String filePath)
        {
            if (!IsValid(filePath))
                return String.Empty;

            return File.ReadAllText(filePath, Encoding.UTF8);
        }

        public static byte[] FileToAllBytes(String filePath)
        {
            if (!IsValid(filePath))
                return new byte[0];

            return File.ReadAllBytes(filePath);
        }

        public static String FileToUnicodeString(String filePath)
        {
            if (!IsValid(filePath))
                return String.Empty;

            return File.ReadAllText(filePath, Encoding.Unicode);
        }

        public static void StringToFile(String filePath, String content)
        {
            if (!IsValid(filePath))
            {
                return;
            }

            CreateDirectory(Path.GetDirectoryName(filePath));

            File.WriteAllText(filePath, content);
        }

        #region FileToBytes：读取文件到缓冲区中
        /// <summary>
        /// 读取文件到缓冲区中，长度len默认为0，读取所有(最大为int.MaxValue或文件实际长度)。
        /// 注意：若读取大文件或读取大量数据不要用该方法，自己单独实现。
        /// </summary>
        public static byte[] FileToBytes(string filePath, int len = 0, int start = 0)
        {
            if (!IsValid(filePath))
                return new byte[0];

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                //文件长度最大支持int.MaxValue或文件实际长度
                int flen = fs.Length > int.MaxValue ? int.MaxValue : (int)fs.Length;
                //若未指定长度，则读取最大长度flen
                int readLen = len == 0 ? flen : len;
                //若读取长度大于最大长度，取实际长度
                readLen = readLen >= flen ? flen : readLen;
                var res = new byte[readLen];
                fs.Read(res, start, readLen);
                return res;
            }
        }

        #endregion

        #endregion

        #region 创建一个目录
        /// <summary>
        /// 创建一个目录(目录的绝对路径)
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        public static void CreateDirectory(string directoryPath)
        {
            //有效性验证
            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }

            //如果目录不存在则创建该目录
            if (!Directory.Exists(directoryPath))
            {
                ZetaLongPaths.ZlpIOHelper.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// 创建一个目录(目录的绝对路径)
        /// 内部已做异常处理
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void CreateDirectorySafe(string directoryPath)
        {
            try
            {
                CreateDirectory(directoryPath);
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
        }

        /// <summary>
        /// 创建路径，如果已经存在，删除路径再重新创建
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void CreateExitsDirectory(string directoryPath)
        {
            //有效性验证
            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }
            if (Directory.Exists(directoryPath))
            {//如果目录存在则删除目录再创建该目录
                ZetaLongPaths.ZlpIOHelper.DeleteDirectory(directoryPath, true);
            }
            ZetaLongPaths.ZlpIOHelper.CreateDirectory(directoryPath);
        }

        /// <summary>
        /// 创建路径，如果已经存在，删除路径再重新创建
        /// 内部已做异常处理
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void CreateExitsDirectorySafe(string directoryPath)
        {
            try
            {
                CreateExitsDirectory(directoryPath);
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
        }

        /// <summary>
        /// 删除一个文件
        /// </summary>
        /// <param name="filePathName"></param>
        public static void DeleteFile(string filePathName)
        {
            //有效性验证
            if (string.IsNullOrEmpty(filePathName))
            {
                return;
            }

            if (File.Exists(filePathName))
            {//如果目录存在则删除目录再创建该目录
                ZetaLongPaths.ZlpIOHelper.DeleteFile(filePathName);
            }
        }

        public static void DeleteFileSafe(string filePathName)
        {
            try
            {
                DeleteFile(filePathName);
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
        }

        /// <summary>
        /// 删除一个目录
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void DeleteDirectory(string directoryPath)
        {
            //有效性验证
            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }

            if (Directory.Exists(directoryPath))
            {//如果目录存在则删除目录再创建该目录
                ZetaLongPaths.ZlpIOHelper.DeleteDirectory(directoryPath, true);
            }
        }

        public static void DeleteDirectorySafe(string directoryPath)
        {
            try
            {
                DeleteDirectory(directoryPath);
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
        }

        /// <summary>
        /// 创建一个文件的上级目录(必须是个文件的路径)
        /// 传入示例:c:\\temp\\abc\\新建文本.txt;
        /// 返回结果:c:\\temp\\abc
        /// </summary>
        /// <param name="filePath">文本文件的路径</param>
        public static string CreateFileDirectory(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            CreateDirectory(dir);
            return dir;
        }

        #endregion

        #region 创建一个文件

        /// <summary>
        /// 创建一个文件(文件的绝对路径)
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void CreateFile(string filePath)
        {
            //有效性验证
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            //如果文件不存在则创建该文件
            if (!File.Exists(filePath))
            {
                CreateFileDirectory(filePath);
                lock (_sync)
                {
                    new FileStream(filePath, FileMode.OpenOrCreate);
                }
            }
        }

        /// <summary>
        /// 创建一个文件  如果已经存在 则删除后重新创建
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateNewFile(string filePath)
        {
            //有效性验证
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            //如果文件存在则删除该文件
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            CreateFileDirectory(filePath);

            lock (_sync)
            {
                new FileStream(filePath, FileMode.OpenOrCreate);
            }
        }

        #endregion

        #region 创建一个文件,并将字节流写入文件
        /// <summary>
        /// 创建一个文件,并将字节流写入文件。(文件的绝对路径)
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="buffer">二进制流数据</param>
        public static void CreateFile(string filePath, byte[] buffer)
        {
            //有效性验证
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            try
            {
                //如果文件不存在则创建该文件
                if (!File.Exists(filePath))
                {
                    //创建一个FileInfo对象
                    FileInfo file = new FileInfo(filePath);

                    //创建文件
                    using (FileStream fs = file.Create())
                    {
                        //写入二进制流
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region 创建一个文件,并将字符串写入文件

        #region 默认格式为utf-8

        /// <summary>
        /// 创建一个文件,并将字符串写入文件，如果文件已存在则覆盖。(文件的绝对路径)
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">字符串数据</param>
        public static void CreateFile(string filePath, string text)
        {
            CreateFile(filePath, text, Encoding.GetEncoding("utf-8"));
        }

        #endregion

        #region 指定格式
        /// <summary>
        /// 创建一个文件,并将字符串写入文件，如果文件已存在则覆盖。(文件的绝对路径)
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">字符串数据</param>
        /// <param name="encoding">字符编码</param>
        public static void CreateFile(string filePath, string text, Encoding encoding)
        {
            //有效性验证
            if (filePath == null || filePath == string.Empty)
            {
                return;
            }

            //获取文件目录路径
            string directoryPath = GetFilePath(filePath);

            //如果目录不存在则创建该目录
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            //创建文件
            FileInfo file = new FileInfo(filePath);
            using (FileStream stream = file.Create())
            {
                using (StreamWriter writer = new StreamWriter(stream, encoding))
                {
                    //写入字符串     
                    writer.Write(text);

                    //输出
                    writer.Flush();
                }
            }
        }
        #endregion

        #endregion

        #region 打开一个文件路径

        /// <summary>
        /// 打开一个文件夹或者文件
        /// </summary>
        /// <param name="path">需要打开的文件、文件夹路径</param>
        public static void Open(params string[] path)
        {
            if ((path == null) || path.Any())
            {
                return;
            }
            var nPath = ConnectPath(path);
            System.Diagnostics.Process.Start("explorer.exe", nPath);

        }

        #endregion

        #region 拼接路径

        /// <summary>
        /// 拼接路径（自动处理其中的分隔符），
        /// </summary>
        public static string ConnectPath(char separate, params string[] path)
        {
            if ((path == null) || !path.Any())
            {
                return String.Empty;
            }
            if (path.Length == 2)
                return string.Format("{0}{1}{2}", path[0].TrimEnd(separate), separate, path[1].TrimStart(separate));
            if (path.Length == 1)
                return path[0];
            StringBuilder sb = new StringBuilder(32);
            foreach (var p in path)
            {
                sb.Append(p.TrimEnd(separate).TrimStart(separate)).Append(separate);
            }
            return sb.ToString().TrimEnd(separate);
        }

        /// <summary>
        /// 拼接路径（自动处理其中的分隔符），默认分隔符char separate = '\\'
        /// </summary>
        public static string ConnectPath(params string[] path)
        {
            return ConnectPath('\\', path);
        }

        #endregion

        #region 转换linux路径为合法的windows路径
        /// <summary>
        /// 转换linux路径为合法的windows路径
        /// 1.转换其中的非法字符为validChar(默认下划短线)
        /// 2.替换分隔符'/' to '\\'
        /// </summary>
        /// <param name="linuxpath"></param>
        /// <param name="validChar"></param>
        /// <returns></returns>
        public static string ConvertLinuxPath(string linuxpath, char validChar = '_')
        {
            var index = linuxpath.IndexOfAny(InvalidPathChars);
            if (index >= 0)
            {
                return ConvertLinuxPath(linuxpath.Replace(linuxpath[index], validChar));
            }
            return linuxpath.Replace('/', '\\');
        }
        #endregion

        #region 拼接Linux文件格式的两个路径

        /// <summary>
        /// 链接Linux文件格式的两个路径（自动处理其中的分隔符），默认分隔符'/'
        /// </summary>
        public static string ConnectLinuxPath(string path1, string path2)
        {
            return ConnectPath('/', path1, path2);
        }

        #endregion

        #region 获取指定路径的linux格式文件或文件夹名称
        /// <summary>
        /// 获取指定路径的linux格式文件或文件夹名称，分隔符‘/’
        /// </summary>
        public static string GetLinuxFileName(string source)
        {
            if (String.IsNullOrEmpty(source))
                return string.Empty;
            return Path.GetFileName(source);
        }
        #endregion

        #region 获取指定路径的linux格式文件或文件夹所在路径
        /// <summary>
        /// 获取指定路径的linux格式文件或文件夹所在路径，分隔符‘/’
        /// </summary>
        public static string GetLinuxFilePath(string source)
        {
            if (String.IsNullOrEmpty(source))
                return string.Empty;
            return Path.GetDirectoryName(source);
        }
        #endregion

        #region 从文件路径中获取扩展名
        /// <summary>
        /// 从文件路径中获取扩展名,不包含扩展名前面的句点,绝对路径
        /// 范例：c:\test\test.jpg,返回jpg
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetExtension(string filePath)
        {
            //有效性验证
            if (String.IsNullOrEmpty(filePath))
                return string.Empty;

            string extension = string.Empty;
            extension = Path.GetExtension(filePath).Replace(".", string.Empty);

            return extension;
        }


        /// <summary>
        /// 尝试从当前路径中获取文件的后缀; 如 123_jpg;
        /// 只是对这种格式做处理!
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="splitChar">拆分字符标识</param>
        /// <returns></returns>
        public static string TryGetExtension(string filename, char splitChar)
        {
            //有效性验证
            if (String.IsNullOrEmpty(filename))
                return string.Empty;

            var temp = filename.Split(splitChar);
            var extension = string.Empty;
            if (temp.Any())
            {
                extension = temp.Last();
            }
            return extension;
        }
        #endregion

        #region 获取文件相对路径映射的物理路径
        /// <summary>
        /// 获取文件相对路径映射的物理路径，若文件为绝对路径则直接返回
        /// </summary>
        /// <param name="relativePath">文件的相对路径</param>        
        public static string GetPhysicalPath(string relativePath)
        {
            //有效性验证
            if (string.IsNullOrEmpty(relativePath))
            {
                return string.Empty;
            }
            //~,~/,/,\
            relativePath = relativePath.Replace("/", @"\").Replace("~", string.Empty).Replace("~/", string.Empty);
            relativePath = relativePath.StartsWith("\\") ? relativePath.Substring(1, relativePath.Length - 1) : relativePath;
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var fullPath = System.IO.Path.Combine(path, relativePath);
            return fullPath;
        }
        #endregion

        #region 获取文件大小描述，绝对路径
        /// <summary>
        /// 获取文件大小描述，绝对路径
        /// </summary>
        public static string GetFileSize(string filePath)
        {
            return GetFileSize(GetFileLength(filePath));
        }

        /// <summary>
        /// 获取当前文件的大小
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static long GetFileLength(string filePath)
        {
            if (!IsValid(filePath))
                return 0;
            FileInfo fi = new FileInfo(filePath);
            return fi.Length;
        }

        /// <summary>
        /// 获取文件大小描述（参数：文件大小，字节数）
        /// </summary>
        public static string GetFileSize(long len, string format = "F2")
        {
            if (len <= 0)
            {
                return "0 KB";
            }

            string unit = " B";
            double res = len, rule = 1024D;
            //KB
            if (len >= rule)
            {
                res = len / rule;
                unit = " KB";
            }
            //M
            if (res > rule)
            {
                res = res / rule;
                unit = " MB";
            }
            //G
            if (res > rule)
            {
                res = res / rule;
                unit = " GB";
            }
            //去掉多余的0
            if (res - Math.Truncate(res) == 0)
            {
                return string.Concat(res.ToString("F2"), unit);
            }
            return string.Concat(res.ToString("F2"), unit);
        }

        /// <summary>
        /// 获取文件大小（参数：文件大小，字节数）
        /// </summary>
        public static string GetFileSize(int len)
        {
            return GetFileSize((long)len);
        }
        #endregion

        #region 获取指定文件夹(包括子文件夹)下的所有文件

        /// <summary>
        /// 获取非系统级目录的所有文件
        /// </summary>
        /// <param name="folder">非系统级目录。</param>
        /// <param name="extensions">筛选的后缀</param>
        /// <returns>返回满足条件的文件列表</returns>
        public static IList<FileInfo> GetFiles(string folder, string[] extensions)
        {
            if (!Directory.Exists(folder))
            {
                return new List<FileInfo>();
            }
            var allFiles = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
            var allMeetFiles = (from file in allFiles
                                let extension = System.IO.Path.GetExtension(file)
                                where extension != null && extensions.Contains(extension.ToLower())
                                select new FileInfo(file)).ToList();

            return allMeetFiles;
        }
        #endregion

        #region 过滤非法文件名

        private static readonly char[] _InvalidChars = System.IO.Path.GetInvalidFileNameChars();

        public static string FilterInvalidFileName(string oriName)
        {
            if (!IsValid(oriName))
            {
                return oriName;
            }

            int index = oriName.IndexOfAny(_InvalidChars);
            while (index >= 0)
            {
                char ch = oriName[index];
                switch (ch)
                {
                    case '<': oriName = oriName.Replace(ch, '＜'); break;
                    case '>': oriName = oriName.Replace(ch, '＞'); break;
                    case '/': oriName = oriName.Replace(ch, '／'); break;
                    case '\\': oriName = oriName.Replace(ch, '＼'); break;
                    case '|': oriName = oriName.Replace(ch, 'Ⅰ'); break;
                    case ':': oriName = oriName.Replace(ch, '：'); break;
                    case '\"': oriName = oriName.Replace(ch, '“'); break;
                    case '*': oriName = oriName.Replace(ch, '＊'); break;
                    case '?': oriName = oriName.Replace(ch, '？'); break;
                    default: oriName = oriName.Replace(ch, ' '); break;
                }
                index = oriName.IndexOfAny(_InvalidChars);
            }
            return oriName;

        }

        #endregion

        #region 判断当前文件是否被占用
        /// <summary>
        /// 判断当前文件是否被占用
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool IsFileInUsing(string path)
        {
            var isUse = true;
            using (new FileStream(path, FileMode.Open))
            {
                isUse = false;
            }
            return isUse;
        }

        #endregion

        #region 输入路径是否合法

        /// <summary>
        /// 输入路径是否合法
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>合法与否</returns>
        public static bool InputPathIsValid(string path)
        {
            return IsPositiveNumber(path) && IsHasUninvalidPathChars(path);
        }

        /// <summary>
        /// 是否是文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsPositiveNumber(string path)
        {
            return Regex.IsMatch(path,
                    @"^[a-zA-Z]:\\[^\/\:\*\?\""\<\>\|\,]+$",
                    RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 文件路径是否合法
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsHasUninvalidPathChars(string path)
        {
            return !Path.GetInvalidPathChars().Any(path.Contains);
        }
        #endregion

        #region 获取文件夹下的特定文件列表
        /// <summary>
        /// 获取文件夹下的特定文件列表，多个扩展名使用分号分开
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetFileListInfo(DirectoryInfo dir, string filter = "*.*")
        {
            var filters = filter.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().Replace("*", "").Replace(".", ""));
            return dir.GetFiles().Where(f =>
            {
                return filters.Any(s => f.Name.EndsWith(s, StringComparison.CurrentCultureIgnoreCase));
            });

        }
        #endregion

        #region GetFilePath
        /// <summary>
        /// 获取指定文件或文件夹路径的所在文件路径，默认分隔符separate='\\'
        ///  如 d:\File\aa或d:\File\aa.rar 返回d:\File
        /// </summary>
        public static string GetFilePath(string source, char separate = '\\')
        {
            if (String.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }
            source = source.TrimEnd(separate);
            var index = source.LastIndexOf(separate);
            if (index > 0)
            {
                return source.Substring(0, index + 1);
            }
            return source;
        }
        #endregion

        #region GetFileName
        /// <summary>
        /// 获取指定文件或文件夹路径的名称，默认分隔符separate='\\'
        /// 如 d:\File\aa或d:\File\aa.rar 返回aa,aa.rar
        /// </summary>
        public static string GetFileName(string source, char separate = '\\')
        {
            if (String.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }
            source = source.TrimEnd(separate);
            var index = source.LastIndexOf(separate);
            if (index > 0)
            {
                return source.Substring(index + 1, source.Length - index - 1);
            }
            return source;
        }
        #endregion

        #region ReadFileHead 读取文件头指定长度的数据
        /// <summary>
        /// 读取文件头指定长度的数据
        /// </summary>
        public static byte[] ReadFileHead(string filePath, int index = 4)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buff = new byte[index];
                fs.Read(buff, 0, index);
                return buff;
            }
        }
        #endregion

        #region 计算文件MD5

        public static string MD5FromFile(string filePath)
        {
            return CryptographyHelper.MD5FromFile(filePath);
        }

        public static string MD5FromFileUpper(string filePath)
        {
            return MD5FromFile(filePath).ToUpper();
        }

        #endregion

        #region 拷贝文件夹
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="destPath"></param>
        public static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加
                if (destPath[destPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                {
                    destPath += System.IO.Path.DirectorySeparatorChar;
                }

                // 判断目标目录是否存在如果不存在则新建
                if (!System.IO.Directory.Exists(destPath))
                {
                    System.IO.Directory.CreateDirectory(destPath);
                }

                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                // string[] fileList = Directory.GetFiles（srcPath）；
                string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
                // 遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    if (System.IO.Directory.Exists(file))
                    {
                        CopyDirectory(file, destPath + System.IO.Path.GetFileName(file));
                    }
                    // 否则直接Copy文件
                    else
                    {
                        File.Copy(file, destPath + System.IO.Path.GetFileName(file), true);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        #endregion

    }
}
