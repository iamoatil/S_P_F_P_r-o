using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control.ReadToTxt
{
    public class HtmlFile : IOfficeFile, IHtmlFile
    {
        public Dictionary<string, string> DocumentSummaryInformation { get; private set; }
        public Dictionary<string, string> SummaryInformation { get; private set; }

        private string _allText;

        public string CommentText
        {
            get { return this._allText; }
        }

        public HtmlFile(string filePath)
        {
            using (FileStream fs = new FileStream(filePath,FileMode.Open,FileAccess.Read,FileShare.Read))
            {
                StreamReader sr = new StreamReader(fs);

                var text = sr.ReadToEnd();

                HtmlToText htmlToText = new HtmlToText();

                this._allText= htmlToText.Convert(text);
            }

            
        }
    }
}
