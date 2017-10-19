using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;

namespace XLY.SF.Project.ScriptEngine.Engine
{
    public class File
    {
        /// <summary>
        /// 读取指定的文件内容，需指定编码格式，filePath为文件路径，encode为编码方式
        /// </summary>
        public string ReadFile(string filePath, string encode)
        {
            try
            {
                var content = System.IO.File.ReadAllText(filePath, Encoding.GetEncoding(encode));
                return content.Replace('\0', ' ');
            }
            catch (Exception ex)
            {
                var str = string.Format("Read file exception", filePath, ex.Message);
                Console.WriteLine(str);
            }
            return string.Empty;
        }

        /// <summary>
        /// 使用utf-8编码读取文件
        /// </summary>
        public string ReadFile(string filePath)
        {
            return this.ReadFile(filePath, "utf-8");
        }

        /// <summary>
        /// 读取xml文件为json对象
        /// </summary>
        public string ReadXML(string filePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                return Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc.DocumentElement);
            }
            catch (Exception ex)
            {
                var str = string.Format("Read file exception", filePath, ex.Message);
                Console.WriteLine(str);

                //如果直接加载失败，尝试过滤二进制字符后再加载
                try
                {
                    XmlDocument doc = new XmlDocument();
                    string txt = System.IO.File.ReadAllText(filePath);
                    string txt2 = ReplaceLowOrderASCIICharacters(txt);
                    doc.InnerXml = txt2;
                    return Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc.DocumentElement);
                }
                catch (Exception ex2)
                {
                    str = string.Format("Read file exception", filePath, ex2.Message);
                    Console.WriteLine(str);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 过滤文本中的二进制字符，不然xml打不开
        /// </summary>
        /// <param name="tmp"></param>
        /// <returns></returns>
        private string ReplaceLowOrderASCIICharacters(string tmp)
        {
            StringBuilder info = new StringBuilder();
            foreach (char cc in tmp)
            {
                int ss = (int)cc;
                if (((ss >= 0) && (ss <= 8)) || ((ss >= 11) && (ss <= 12)) || ((ss >= 14) && (ss <= 32)))
                {
                    info.Append(' ');
                }
                else info.Append(cc);
            }
            return info.ToString();
        }

        #region FindFiles
        /// <summary>
        /// 获取指定文件夹得所有文件列表，返回JSON数组
        /// </summary>
        public string FindFiles(string path)
        {
            try
            {
                var files = System.IO.Directory.GetFiles(path);
                return Newtonsoft.Json.JsonConvert.SerializeObject(files);
            }
            catch (Exception ex)
            {
                var str = string.Format("Get Files in folder exception", path, ex.Message);
                Console.WriteLine(str);
                return string.Empty;
            }
        }

        /// <summary>
        /// 验证文件是否可用
        /// </summary>
        public bool IsValid(string path)
        {
            try
            {
                //文件不存在
                if (!System.IO.File.Exists(path)) return false;
                System.IO.FileInfo info = new FileInfo(path);
                //文件大小为0
                if (info == null || info.Length <= 0) return false;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取指定文件夹得所有文件名字（无后缀名）列表，返回JSON数组
        /// </summary>
        public string FindFileNames(string path)
        {
            try
            {
                var files = System.IO.Directory.GetFiles(path);

                var fileNames = new List<string>();
                ForEach(files,f => fileNames.Add(System.IO.Path.GetFileNameWithoutExtension(f)));

                return Newtonsoft.Json.JsonConvert.SerializeObject(fileNames);
            }
            catch (Exception ex)
            {
                var str = string.Format("Get files in folder exception", path, ex.Message);
                Console.WriteLine(str);
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取指定文件夹得所有文件名字列表，返回JSON数组

        /// </summary>
        public string FindFileNamesWithExtension(string path)
        {
            try
            {
                var files = System.IO.Directory.GetFiles(path);

                var fileNames = new List<string>();
                ForEach(files,f => fileNames.Add(System.IO.Path.GetFileName(f)));

                return Newtonsoft.Json.JsonConvert.SerializeObject(fileNames);
            }
            catch (Exception ex)
            {
                var str = string.Format("Get files in folder exception", path, ex.Message);
                Console.WriteLine(str);
                return string.Empty;
            }
        }

        public void ForEach<T>(IEnumerable<T> souce, Action<T> action)
        {
            if (!IsValid(souce))
            {
                return;
            }
            foreach (var item in souce)
            {
                action(item);
            }
        }
        public bool IsValid<T>(IEnumerable<T> source)
        {
            return source != null && source.Any();
        }
        #endregion

        /// <summary>
        /// 获取指定文件夹得所有文件夹列表，返回JSON数组
        /// </summary>
        public string FindDirectories(string path)
        {
            try
            {
                var files = System.IO.Directory.GetDirectories(path);
                return Newtonsoft.Json.JsonConvert.SerializeObject(files);
            }
            catch (Exception ex)
            {
                var str = string.Format("Get files in folder exception", path, ex.Message);
                Console.WriteLine(str);
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取文件相对路径映射的物理路径
        /// </summary>
        public string GetPhysicalPath(string relativePath)
        {
            return GetPhysicalPath(relativePath);
        }
        
        /// <summary>
        /// 获取指定文件或文件夹路径的名称
        /// 如 d:\File\aa或d:\File\aa.rar 返回aa,aa.rar
        /// </summary>
        public string GetFileName(string source)
        {
            return GetFileName(source);
        }

        #region GetFilePath
        /// <summary>
        /// 获取指定文件或文件夹路径的所在文件路径
        ///  如 d:\File\aa或d:\File\aa.rar 返回d:\File
        /// </summary>
        public string GetFilePath(string source)
        {
            return GetFilePath(source);
        }
        #endregion
    }
}
