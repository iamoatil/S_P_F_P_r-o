using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Utility.Logger;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 菜单行为信息
    /// </summary>
    public class MenuActionInfo
    {
        public MenuActionInfo()
        {
            this.IsEnabled = true;
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// 快捷键
        /// </summary>
        public string KeyText { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 当点击时触发
        /// </summary>
        public event EventHandler<MenuActionInfoEventArgs> OnClick;

        /// <summary>
        /// 执行点击事件
        /// </summary>
        /// <param name="args"></param>
        internal void DoOnClick(object sender, EventArgs args)
        {
            try
            {
                if (this.OnClick != null)
                {
                    MenuActionInfoEventArgs e = new MenuActionInfoEventArgs();
                    e.MenuActionInfo = this;
                    e.Source = args;
                    this.OnClick(sender, e);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("右键菜单点击时出错", ex);
            }
        }
    }

    /// <summary>
    /// 菜单行为列表
    /// </summary>
    public class MenuActionInfoCollection : List<MenuActionInfo>
    { }

    /// <summary>
    /// 菜单行为事件
    /// </summary>
    public class MenuActionInfoEventArgs : EventArgs
    {
        /// <summary>
        /// 菜单行为事件
        /// </summary>
        public MenuActionInfo MenuActionInfo { get; set; }

        /// <summary>
        /// 源事件参数
        /// </summary>
        public EventArgs Source { get; set; }
    }
}
