using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 引用参数
    /// </summary>
    public class InvokeArgs
    {
        private Dictionary<object, object> _dic = new Dictionary<object, object>();

        public object this[object key]
        {
            get
            {
                if (this._dic.ContainsKey(key))
                    return this._dic[key];
                else
                    return null;
            }
            set
            {
                this._dic[key] = value;
            }
        }
    }
}
