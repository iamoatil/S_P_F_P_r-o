using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.SystemKeys;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:36:37
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.MessageBase
{
    /// <summary>
    /// 系统消息
    /// </summary>
    public class SysCommonMsgArgs : ArgsBase
    {
        /// <summary>
        /// 系统消息类型
        /// </summary>
        //public string SysKey { get; private set; }

        public SysCommonMsgArgs(string sysKey)
        {
            base.MsgToken = sysKey;
        }
    }

    /// <summary>
    /// 系统消息
    /// </summary>
    public class SysCommonMsgArgs<TParam> : SysCommonMsgArgs, IArgsParameter<TParam>
    {
        public SysCommonMsgArgs(string sysKey) 
            : base(sysKey)
        {
        }

        public TParam Parameters { get; set; }
    }
}
