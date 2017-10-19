using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 列表会话模式激活按钮
    /// </summary>
    public class GridViewConversationActionButtonProvider : IXlyActionButtonProvider
    {
        /// <summary>
        /// 执行激活
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="args">参数</param>
        public void Action(object type, object args)
        {
            if (type is GridViewConversationContainerMessageType && args != null)
            {
                GridViewConversationContainerMessageType _type = (GridViewConversationContainerMessageType)type;
                if (_type == GridViewConversationContainerMessageType.String)
                {

                }
                else if (_type == GridViewConversationContainerMessageType.Image)
                {
                    //XlyMessageBox.ShowInfo(args.ToSafeString());
                }
            }
        }
    }
}
