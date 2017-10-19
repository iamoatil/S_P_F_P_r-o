using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/15 15:11:39
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.BaseUtility.BaseUtilityEnum
{
    #region EnumEncodingType（字符编码方式）

    /// <summary>
    /// 字符编码方式
    /// </summary>
    public enum EnumEncodingType
    {
        /// <summary>
        /// UTF_8
        /// </summary>
        [Description("utf-8")]
        UTF_8 = 0x1,

        /// <summary>
        /// GB2312
        /// </summary>
        [Description("gb2312")]
        GB2312 = 0x2,

        /// <summary>
        /// GBK
        /// </summary>
        [Description("GBK")]
        GBK = 3,

        /// <summary>
        /// GB18030
        /// </summary>
        [Description("GB18030")]
        GB18030 = 4
    }

    #endregion
}
