using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 资源管理器事件
    /// </summary>
    public class ResourceBrowserEventArgs : EventArgs
    {
        #region FullPaths -- 完整路径集合

        private List<string> _FullPaths = new List<string>();
        /// <summary>
        /// 完整路径集合
        /// </summary>
        public List<string> FullPaths
        {
            get { return _FullPaths; }
            set { _FullPaths = value; }
        }

        #endregion
    }
}
