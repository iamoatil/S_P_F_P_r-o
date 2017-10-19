using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 表视图列信息
    /// </summary>
    public class GridViewColumn : GridViewColumnBase
    {
        #region Type -- 单元格类型

        private GridViewColumnType _Type;
        /// <summary>
        /// 单元格类型
        /// </summary>
        public GridViewColumnType Type
        {
            get { return _Type; }
            set { _Type = value; this.OnPropertyChanged("Type"); }
        }

        #endregion

        #region Template -- 自定义模版

        private DataTemplate _Template;
        /// <summary>
        /// 自定义模版
        /// </summary>
        public DataTemplate Template
        {
            get { return _Template; }
            set { _Template = value; this.OnPropertyChanged("Template"); }
        }

        #endregion

        #region TemplateKey -- 在没有自定义模版的情况下列表控件资源中寻找模版键值

        private object _TemplateKey;
        /// <summary>
        /// 在列表控件资源中寻找模版键值
        /// </summary>
        public object TemplateKey
        {
            get { return _TemplateKey; }
            set { _TemplateKey = value; this.OnPropertyChanged("TemplateKey"); }
        }

        #endregion
    }
}
