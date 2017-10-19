using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Utility
{
    /************************************************
    *Author: Songbing
    *Create Time 2016/10/11 15:16:59
    *Description:
    *
    *Update History:
    *
    ***********************************************/
    public class SPADefaultSyncResult:DefaultSyncResult
    {
        /// <summary>
        /// 开始执行插件
        /// </summary>
        public event Action<string> OnBeginIDataParsePlugin;

        /// <summary>
        /// 执行插件结束
        /// </summary>
        public event Action<string, int> OnEndIDataParsePlugin;

        /// <summary>
        /// 开始执行插件
        /// </summary>
        /// <param name="plugin"></param>
        public void BeginIDataParsePlugin(string plugin)
        {
            if(null != OnBeginIDataParsePlugin)
            {
                OnBeginIDataParsePlugin(plugin);
            }
        }

        /// <summary>
        /// 执行插件结束
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="count"></param>
        public void EndIDataParsePlugin(string plugin, int count)
        {
            if(null != OnEndIDataParsePlugin)
            {
                OnEndIDataParsePlugin(plugin, count);
            }
        }

    }
}
