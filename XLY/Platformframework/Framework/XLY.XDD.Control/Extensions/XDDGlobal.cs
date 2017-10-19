using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 基础组建公共部分
    /// </summary>
    public static class XDDGlobal
    {
        #region NoneWindowStyle

        private static Style _NoneWindowStyle;
        /// <summary>
        /// 空窗口样式
        /// </summary>
        public static Style NoneWindowStyle
        {
            get
            {
                if (_NoneWindowStyle == null)
                {
                    _NoneWindowStyle = XlyResourceHelper.GetResourceFromResourceDictionary<Style>("pack://application:,,,/XLY.XDD.Control;component/Themes/Window/NoneWindowStyle.xaml", "NoneWindowStyle");
                }
                return _NoneWindowStyle;
            }
        }

        #endregion

        #region MirrorInfoWindowStyle

        private static Style _MirrorInfoWindowStyle;
        /// <summary>
        /// 空窗口样式
        /// </summary>
        public static Style MirrorInfoWindowStyle
        {
            get
            {
                if (_MirrorInfoWindowStyle == null)
                {
                    _MirrorInfoWindowStyle = XlyResourceHelper.GetResourceFromResourceDictionary<Style>("pack://application:,,,/XLY.XDD.Control;component/Themes/Window/NoneWindowStyle.xaml", "MirrorInfoWindowStyle");
                }
                return _MirrorInfoWindowStyle;
            }
        }

        #endregion
    }
}
