using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/16 18:11:51
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.Domains
{
    #region EnumFileAttributeFlags：文件属性枚举
    /// <summary>
    /// 文件属性枚举
    /// </summary>
    [Flags]
    public enum EnumFileAttributeFlags : uint
    {
        [Description("LANGKEY_ZhiDu_00346")]
        FILE_ATTRIBUTE_READONLY = 0x00000001,
        [Description("LANGKEY_YinCang_00347")]
        FILE_ATTRIBUTE_HIDDEN = 0x00000002,
        [Description("LANGKEY_XiTong_00348")]
        FILE_ATTRIBUTE_SYSTEM = 0x00000004,
        [Description("LANGKEY_WenJianJia_00349")]
        FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
        [Description("LANGKEY_CunDang_00350")]
        FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
        [Description("LANGKEY_SheBei_00351")]
        FILE_ATTRIBUTE_DEVICE = 0x00000040,
        [Description("LANGKEY_WenJian_00352")]
        FILE_ATTRIBUTE_NORMAL = 0x00000080,
        [Description("LANGKEY_LinShiWenJian_00353")]
        FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
        [Description("LANGKEY_XiShuWenJian_00354")]
        FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
        [Description("LANGKEY_JieXiDian_00355")]
        FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
        [Description("LANGKEY_YaSuo_00356")]
        FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
        [Description("LANGKEY_TuoJi_00357")]
        FILE_ATTRIBUTE_OFFLINE = 0x00001000,
        [Description("LANGKEY_FeiNeiRongSuoYin_00358")]
        FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,
        [Description("LANGKEY_JiaMi_00359")]
        FILE_ATTRIBUTE_ENCRYPTED = 0x00004000
    }
    #endregion

    #region EnumGetFileInfoFlags：获取文件标示
    /// <summary>
    /// 获取文件标示
    /// </summary>
    [Flags]
    public enum EnumGetFileInfoFlags : uint
    {
        SHGFI_ICON = 0x000000100,     // get icon
        SHGFI_DISPLAYNAME = 0x000000200,     // get display name
        SHGFI_TYPENAME = 0x000000400,     // get type name
        SHGFI_ATTRIBUTES = 0x000000800,     // get attributes
        SHGFI_ICONLOCATION = 0x000001000,     // get icon location
        SHGFI_EXETYPE = 0x000002000,     // return exe type
        SHGFI_SYSICONINDEX = 0x000004000,     // get system icon index
        SHGFI_LINKOVERLAY = 0x000008000,     // put a link overlay on icon
        SHGFI_SELECTED = 0x000010000,     // show icon in selected state
        SHGFI_ATTR_SPECIFIED = 0x000020000,     // get only specified attributes
        SHGFI_LARGEICON = 0x000000000,     // get large icon
        SHGFI_SMALLICON = 0x000000001,     // get small icon
        SHGFI_OPENICON = 0x000000002,     // get open icon
        SHGFI_SHELLICONSIZE = 0x000000004,     // get shell size icon
        SHGFI_PIDL = 0x000000008,     // pszPath is a pidl
        SHGFI_USEFILEATTRIBUTES = 0x000000010,     // use passed dwFileAttribute
        SHGFI_ADDOVERLAYS = 0x000000020,     // apply the appropriate overlays
        SHGFI_OVERLAYINDEX = 0x000000040      // Get the index of the overlay
    }
    #endregion
}
