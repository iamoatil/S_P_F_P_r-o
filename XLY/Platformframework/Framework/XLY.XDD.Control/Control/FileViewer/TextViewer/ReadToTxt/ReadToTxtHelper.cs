using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control.ReadToTxt
{
    /// <summary>
    /// 读取到文本的辅助类
    /// </summary>
    public static class ReadToTxtHelper
    {
        /// <summary>
        /// 支持内置的office读取格式
        /// </summary>
        private static readonly string[] support_office_suffix = {
                ".txt",
                ".log",
                ".ini",
                ".html",
                ".htm",
                ".xml",
                ".xaml",
                ".js",
                ".csv",
                ".doc", 
                ".docx", 
                ".xls", 
                ".xlxs", 
                ".ppt", 
                ".pptx", 
                ".pdf",
                ".xlsb",
                ".rtf",
                ".odf",
                ".odt",
                ".ods",
                ".odp",
                ".odg",
                ".pages",
                ".numbers",
                ".keynote",
                ".fodp",
                ".fods",
                ".fodt",
                ".eml",
                ".cs"
            };

        /// <summary>
        /// 是否支持该文件后缀
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns>是否支持该文件后缀</returns>
        public static bool IsSupport(string extension)
        {
            return support_office_suffix.Contains(extension.Trim().ToLower());
        }

        /// <summary>
        /// 获取office文档内容
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string _GetContent(IOfficeFile file)
        {

            if (file is IWordFile)
            {
                IWordFile wordFile = file as IWordFile;
                return wordFile.ParagraphText;
            }
            else if (file is IPowerPointFile)
            {
                IPowerPointFile pptFile = file as IPowerPointFile;
                return pptFile.AllText;
            }
            else if (file is ITextFile)
            {
                ITextFile textFile = file as ITextFile;
                return textFile.CommentText;
            }
            else if (file is IPDFFile)
            {
                IPDFFile pdfFile = file as IPDFFile;
                return pdfFile.CommentText;
            }
            else if (file is IHtmlFile)
            {
                IHtmlFile htmlFile = file as IHtmlFile;
                return htmlFile.CommentText;
            }
            else if (file is IExcelFile)
            {
                IExcelFile excelFile = file as IExcelFile;
                return excelFile.CommentText;
            }
            else
            {
                return String.Format("无法解析");
            }
        }

        /// <summary>
        /// 获取Office文档内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetContent(string path)
        {
            IOfficeFile iof = OfficeFileFactory.CreateOfficeFile(path);

            string text = _GetContent(iof);

            return text;
        }
    }
}
