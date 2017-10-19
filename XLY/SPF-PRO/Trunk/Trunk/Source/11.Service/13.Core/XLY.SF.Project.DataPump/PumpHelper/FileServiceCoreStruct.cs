using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains.DllElement;

namespace XLY.SF.Project.DataPump.PumpHelper
{

        #region 节点数据链表结构

        /// <summary>
        /// 节点数据链表结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LINK_DIR_FILE_NODE_INFO
        {
            /// <summary>
            /// 结点数据结构信息
            /// </summary>
            public DIR_FILE_NODE_INFO NodeDataInfo;

            /// <summary>
            /// 下一个文件节点数据链表;
            /// </summary>
            public IntPtr next;
        }

        #endregion

}
