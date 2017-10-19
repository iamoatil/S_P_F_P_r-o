using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;


namespace XLY.XDD.Control.ReadToTxt
{
    public class PDFFile : IOfficeFile, IPDFFile
    {
       public Dictionary<string, string> DocumentSummaryInformation { get; private set; }
       public Dictionary<string, string> SummaryInformation { get; private set; }

       private StringBuilder _allText;

       public PDFFile(string filePath)
       {
           using (PdfReader reader = new PdfReader(filePath))
           {
               _allText = new StringBuilder();
               if (reader != null && reader.NumberOfPages > 0)
               {
                   for (int i = 1; i <= reader.NumberOfPages; i++)
                   {
                       _allText.Append(PdfTextExtractor.GetTextFromPage(reader, i) + Environment.NewLine);
                   }
               }
           }
       }

        public string CommentText
        {
            get { return this._allText.ToSafeString(); }
        }
    }
}
