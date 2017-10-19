using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:35:03
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.MessageBase
{
    public interface IArgsParameter<TParam>
    {
        /// <summary>
        /// 消息参数
        /// </summary>
        TParam Parameters { get; set; }
    }

    public class ArgsBase
    {
        /// <summary>
        /// 消息标识
        /// </summary>
        public string MsgToken { get; internal set; }
    }
}
