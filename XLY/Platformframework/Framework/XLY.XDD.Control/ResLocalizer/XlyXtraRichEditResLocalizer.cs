using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control.ResLocalizer
{
    /// <summary>
    /// DevRichEdit资源转换器
    /// </summary>
    public class XlyXtraRichEditResLocalizer : DevExpress.XtraRichEdit.Localization.XtraRichEditResLocalizer
    {
        public override string GetLocalizedString(DevExpress.XtraRichEdit.Localization.XtraRichEditStringId id)
        {
            switch (id)
            {
                case DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.MenuCmd_CopySelection: return "复制";
                case DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.MenuCmd_CutSelection: return "剪切";
                case DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.MenuCmd_Paste: return "粘贴";
                case DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.MenuCmd_IncrementIndent: return "增加缩进";
                case DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.MenuCmd_DecrementIndent: return "减少缩进";
                case DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.MenuCmd_Bookmark: return "书签";
                case DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.MenuCmd_Hyperlink: return "超链接...";
                case DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.MenuCmd_ShowFontForm: return "字体...";
                case DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.MenuCmd_ShowParagraphForm: return "段落...";
            }
            string result = base.GetLocalizedString(id);
            return result;
        }
    }
}
