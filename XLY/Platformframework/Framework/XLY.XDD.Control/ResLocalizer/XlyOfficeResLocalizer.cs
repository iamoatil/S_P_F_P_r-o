using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control.ResLocalizer
{
    /// <summary>
    /// Dev Office 资源转换器
    /// </summary>
    public class XlyOfficeResLocalizer : DevExpress.Office.Localization.OfficeResLocalizer
    {
        public override string GetLocalizedString(DevExpress.Office.Localization.OfficeStringId id)
        {
            switch (id)
            {
                case DevExpress.Office.Localization.OfficeStringId.MenuCmd_CopySelection: return "复制";
                case DevExpress.Office.Localization.OfficeStringId.MenuCmd_CutSelection: return "剪切";
                case DevExpress.Office.Localization.OfficeStringId.MenuCmd_Paste: return "粘贴";
                case DevExpress.Office.Localization.OfficeStringId.MenuCmd_InsertHyperlink: return "超链接...";
            }
            string result = base.GetLocalizedString(id);
            return result;
        }
    }
}
