using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace XLY.XDD.Control.ReadToTxt
{
    class ExcelFile : IOfficeFile, IExcelFile
    {
        public Dictionary<string, string> DocumentSummaryInformation { get; private set; }
        public Dictionary<string, string> SummaryInformation { get; private set; }
        private StringBuilder _allText;

        public ExcelFile(string filePath)
        {
            ReadExcel(filePath);
        }

        public ExcelFile(string filePath,bool isXlxs)
        {
            ReadExcel(filePath, isXlxs);
        }

        public string CommentText
        {
            get { return this._allText.ToSafeString(); }
        }

        private  void  ReadExcel(string filePath,bool isXlxs= false)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // 把xls文件中的数据写入wk中

                IWorkbook wk;
                if (isXlxs)
                {
                    wk = new XSSFWorkbook(fs);
                }
                else
                {
                    wk = new HSSFWorkbook(fs);
                } 
                
                _allText = new StringBuilder();
                // sheet总共的表数
                for (int i = 0; i < wk.NumberOfSheets; i++)
                {
                    // 读取当前表数据
                    ISheet sheet = wk.GetSheetAt(i);

                    // 当前表的总行数
                    for (int j = 0; j <= sheet.LastRowNum; j++)
                    {
                        // 读取当前行数据
                        IRow row = sheet.GetRow(j);

                        if (row != null)
                        {
                            // 当前行的总列数
                            for (int k = 0; k <= row.LastCellNum; k++)
                            {
                                //当前表格
                                ICell cell = row.GetCell(k);

                                if (cell != null)
                                {
                                    _allText.Append(cell.ToSafeString() + " ");
                                }
                            }
                            _allText.Append(Environment.NewLine);
                        }
                    }
                }
            }
        }

    }
}
