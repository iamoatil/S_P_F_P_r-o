using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 默认的更新输入框文本内容
    /// </summary>
    public class DefaultUpdateXlyComboBoxText : IUpdateXlyComboBoxText
    {
        /// <summary>
        /// 更新文本内容
        /// </summary>
        /// <param name="_text">当前文本内容</param>
        /// <param name="_old">老的数据对象</param>
        /// <param name="_new">新的数据对象</param>
        /// <returns>文本内容</returns>
        public string Update(string _text, object _old, object _new)
        {
            return _new.ToSafeString();
        }
    }
}
