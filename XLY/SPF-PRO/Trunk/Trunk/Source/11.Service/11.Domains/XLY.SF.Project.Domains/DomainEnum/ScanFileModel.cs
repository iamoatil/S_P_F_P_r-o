using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/16 18:12:45
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.Domains
{
    public enum ScanFileModel : byte
    {
        /// <summary>
        /// 快速扫描
        /// </summary>
        [Description("LANGKEY_KuaiSuSaoMiaoZhengChangShanChu_00360")]
        Quick = 0x43,

        /// <summary>
        /// 深度扫描
        /// </summary>
        [Description("LANGKEY_ShenDuSaoMiaoZhengChangShanChu_00361")]
        Depth = 0x80,

        /// <summary>
        /// 高级扫描
        /// </summary>
        [Description("LANGKEY_GaoJiSaoMiaoZhengChangShanChuD_00362")]
        Expert = 0x87,

        /// <summary>
        /// RAW 扫描
        /// </summary>
        [Description("LANGKEY_RAWSaoMiaoRAW_00363")]
        Raw = 0xC0
    }
}
