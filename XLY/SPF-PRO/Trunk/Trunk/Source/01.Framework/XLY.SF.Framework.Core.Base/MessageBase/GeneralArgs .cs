using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:35:43
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.MessageBase
{
    /// <summary>
    /// 普通通知消息
    /// </summary>
    public class GeneralArgs : ArgsBase
    {
        /// <summary>
        /// 普通消息Key
        /// </summary>
        public string GeneralKey { get; private set; }

        public GeneralArgs(string generalKey)
        {
            GeneralKey = generalKey;
        }
    }

    public class GeneralArgs<TParam> : ArgsBase, IArgsParameter<TParam>
    {
        public TParam Parameters { get; set; }
    }
}
