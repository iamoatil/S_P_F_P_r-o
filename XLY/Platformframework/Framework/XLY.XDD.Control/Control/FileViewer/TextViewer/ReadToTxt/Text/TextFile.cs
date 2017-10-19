using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control.ReadToTxt
{
    public class TextFile : IOfficeFile, ITextFile
    {
        public Dictionary<string, string> DocumentSummaryInformation { get; private set; }
        public Dictionary<string, string> SummaryInformation { get; private set; }
       
        
       private string _allText;

       public TextFile(string filePath)
       {
           //using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
           //{
           //    StreamReader sr = new StreamReader(fs,Encoding.UTF8);
           //    _allText = new StringBuilder();
           //    while (sr.Peek()>-1)
           //    {
           //        _allText.Append(sr.ReadLine() + Environment.NewLine);
           //    }
           //}

           _allText = File.ReadAllText(filePath, Encoding.UTF8);
       }

        public string CommentText
        {
            get { return this._allText.ToSafeString(); }
        }
    }
}
