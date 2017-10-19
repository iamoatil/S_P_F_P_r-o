using System;
using System.IO;

namespace XLY.XDD.Control.ReadToTxt
{
    public static class OfficeFileFactory
    {
        public static IOfficeFile CreateOfficeFile(String filePath)
        {
            String extension = Path.GetExtension(filePath).ToLower();

            if (String.Equals(".doc", extension))
            {
                return new WordBinaryFile(filePath);
            }
            else if (String.Equals(".ppt", extension))
            {
                return new PowerPointFile(filePath);
            }
            else if (String.Equals(".docx", extension))
            {
                return new WordOOXMLFile(filePath);
            }
            else if (String.Equals(".pptx", extension))
            {
                return new PowerPointOOXMLFile(filePath);
            }
            else if (String.Equals(".pdf", extension))
            {
                return new PDFFile(filePath);
            }
            else if (String.Equals(".html", extension) || 
                String.Equals(".htm", extension)||
                String.Equals(".xaml", extension))
            {
                return new HtmlFile(filePath);
            }
            else if (String.Equals(".xls", extension))
            {
                return new ExcelFile(filePath);
            }
            else if (String.Equals(".xlsx", extension))
            {
                return new ExcelFile(filePath,true);
            }
            else if (String.Equals(".txt", extension) || 
                String.Equals(".log", extension)||
                String.Equals(".ini", extension)||
                String.Equals(".csv", extension)||
                String.Equals(".js", extension)||
                String.Equals(".xml", extension))
            {
                return new TextFile(filePath);
            }
            else
            {
                if (extension[extension.Length - 1] == 'x' || extension[extension.Length - 1] == 'X')
                {
                    return new OfficeOpenXMLFile(filePath);
                }
                else
                {
                    return new CompoundBinaryFile(filePath);
                }
            }
        }
    }
}