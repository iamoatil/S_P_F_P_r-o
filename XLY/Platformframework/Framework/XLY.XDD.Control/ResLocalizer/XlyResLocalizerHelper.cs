using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control.ResLocalizer
{
    /// <summary>
    /// Dev语言资源辅助类
    /// </summary>
    public static class XlyResLocalizerHelper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            DevExpress.XtraRichEdit.Localization.XtraRichEditResLocalizer.Active = new XlyXtraRichEditResLocalizer();
            DevExpress.XtraSpreadsheet.Localization.XtraSpreadsheetResLocalizer.Active = new XlyXtraSpreadsheetResLocalizer();
            DevExpress.Office.Localization.OfficeResLocalizer.Active = new XlyOfficeResLocalizer();
        }
    }
}
