using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XLY.SF.Domain;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.Services.ADB
{
    /// <summary>
    /// 数据接收器.
    /// </summary>
    internal class LSFileReceiver : AbstractOutputReceiver
    {
        /// <summary>
        /// 文件列表结果
        /// </summary>
        public List<LSFile> Files;

        /// <summary>
        /// 源路径（指令路径）
        /// </summary>
        public string Source = string.Empty;

        /// <summary>
        /// 执行数据解析
        /// </summary>
        public override void DoResolver()
        {
            if (this.Source == null) return;
            var files = this.ProcessLines(this.Lines, this.Source);
            if (files.Count == 1)
            {
                var f = files.First();
                //if folder
                var name = FileHelper.GetLinuxFileName(this.Source);
                if (!f.IsFolder && f.Name == name)
                {
                    f.Path = FileHelper.GetLinuxFilePath(this.Source);
                    f.IsRootFile = true;
                }
            }
            this.Files = files;
        }

        /****************** private methods ******************/

        private List<LSFile> ProcessLines(List<string> lines, string path)
        {
            List<LSFile> files = new List<LSFile>();
            if (lines==null || path == null) return files;
            foreach (var line in lines)
            {
                var f = this.ProcessLine(line);
                if (f == null) continue;
                f.Path = path;
                files.Add(f);
            }
            return files;
        }

        public const String LS_PATTERN_EX = @"^([bcdlsp-][-r][-w][-xsS][-r][-w][-xsS][-r][-w][-xstST])\s+(?:\d{0,})?\s*(\S+)\s+(\S+)\s+(?:\d{1,},\s+)?(\d{1,}|\s)\s+(\w{3}|\d{4})[\s-](?:\s?(\d{1,2})\s?)[\s-]\s?(?:(\d{2}|\d{4}|\s)\s*)?(\d{2}:\d{2}|\s)\s*(.*?)([/@=*\|]?)$";

        private LSFile ProcessLine(string line)
        {
            if (String.IsNullOrWhiteSpace(line))
                return null;
            var m = Regex.Match(line.Trim(),LS_PATTERN_EX, RegexOptions.Compiled);
            if (!m.Success) return null;
            LSFile file = new LSFile();
            file.Name = m.Groups[9].Value;
            file.Permission = m.Groups[1].Value;
            var sized = m.Groups[4].Value.Trim();
            file.Size = Int32.Parse(sized);
            //folder
            file.IsFolder = false;
            switch (file.Permission[0])
            {
                case 'b':
                    file.Type = "Block";
                    break;
                case 'c':
                    file.Type = "Character";
                    break;
                case 'd':
                    /* 遇到一个recovery模式的手机，文件夹也有大小，切大小为4096，此处临时修改为此方案，以后根据测试情况在看 */
                    file.Type = "Directory"; if (String.IsNullOrWhiteSpace(sized) || file.Size == 4096) file.IsFolder = true;
                    break;
                case 'l':
                    file.Type = "Link";
                    this.ProcessLink(file.Name, file);
                    break;
                case 's':
                    file.Type = "Socket";
                    break;
                case 'p':
                    file.Type = "FIFO";
                    break;
                case '-':
                default:
                    file.Type = "File";
                    break;
            }
            //datetime
            String date1 = m.Groups[5].Value.Trim();
            String date2 = m.Groups[6].Value.Trim();
            String date3 = m.Groups[7].Value.Trim();
            String time = m.Groups[8].Value.Trim();
            string datestr = String.Format("{0}-{1}-{2} {3}", date1, date2.PadLeft(2, '0'), date3, time);
            try
            {
                var date = DateTime.ParseExact(datestr, "yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture);
                file.CreateDate = date;
            }
            catch (Exception ex)
            {
            }

            //link file
            //return
            return file;
        }
        
        /// <summary>
        /// 对字符串进行正则匹配，默认单行模式，并忽略大小写。
        /// </summary>
        public Match Match(String source, String pattern)
        {
            return Match(source, pattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 对字符串进行正则匹配
        /// </summary>
        public Match Match(String source, String pattern, RegexOptions options)
        {
            return Regex.Match(source, pattern, options);
        }
        private void ProcessLink(string name, LSFile file)
        {
            String[] segments = name.Split(new string[] { " -> " }, StringSplitOptions.RemoveEmptyEntries);
            // we should have 2 segments
            if (segments.Length == 2)
            {
                file.LinkPath = segments[1].Trim();
            }
        }
    }

}
