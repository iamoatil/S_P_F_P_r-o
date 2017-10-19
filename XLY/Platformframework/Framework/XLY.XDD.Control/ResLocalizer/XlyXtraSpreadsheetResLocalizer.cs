using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control.ResLocalizer
{
    /// <summary>
    /// Dev Excel 资源转换器
    /// </summary>
    public class XlyXtraSpreadsheetResLocalizer : DevExpress.XtraSpreadsheet.Localization.XtraSpreadsheetResLocalizer
    {
        public override string GetLocalizedString(DevExpress.XtraSpreadsheet.Localization.XtraSpreadsheetStringId id)
        {
            string result = base.GetLocalizedString(id);
            return result;
        }
    }
}
