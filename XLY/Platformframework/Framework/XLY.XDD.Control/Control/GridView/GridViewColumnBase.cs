using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 表视图列信息基类
    /// </summary>
    public abstract class GridViewColumnBase : ViewModelBase, IGridViewColumn
    {
        #region Header -- 列头

        private string _Header;
        /// <summary>
        /// 列头
        /// </summary>
        public string Header
        {
            get { return _Header; }
            set { _Header = value; this.OnPropertyChanged("Header"); }
        }

        #endregion

        #region FieldName -- 绑定的字段名

        private string _FieldName;
        /// <summary>
        /// 绑定的字段名
        /// </summary>
        public string FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; this.OnPropertyChanged("FieldName"); }
        }

        #endregion

        #region IsSort -- 是否支持排序

        private bool _IsSort = true;
        /// <summary>
        /// 是否支持排序
        /// </summary>
        public bool IsSort
        {
            get { return _IsSort; }
            set { _IsSort = value; this.OnPropertyChanged("IsSort"); }
        }

        #endregion

        #region IsDistinct -- 是否支持去重（指示该字段在去重是将被使用）

        private bool _IsDistinct = true;
        /// <summary>
        /// 是否支持去重（指示该字段在去重时将被使用）
        /// </summary>
        public bool IsDistinct
        {
            get { return _IsDistinct; }
            set { _IsDistinct = value; this.OnPropertyChanged("IsDistinct"); }
        }

        #endregion

        #region IsDynamic -- 是否是动态数据

        private bool _IsDynamic;
        /// <summary>
        /// 是否是动态数据
        /// </summary>
        public bool IsDynamic
        {
            get { return _IsDynamic; }
            set { _IsDynamic = value; this.OnPropertyChanged("IsDynamic"); }
        }

        #endregion

        #region Width -- 宽度

        private double _Width = 200d;
        /// <summary>
        /// 宽度
        /// </summary>
        public double Width
        {
            get { return _Width; }
            set { _Width = value; this.OnPropertyChanged("Width"); }
        }

        #endregion

        #region Column -- 创建之后保留的对真实列的引用

        private object _Column;
        /// <summary>
        /// 创建之后保留的对真实列的引用
        /// </summary>
        public object Column
        {
            get { return _Column; }
            set { _Column = value; }
        }

        #endregion

        #region IsVisible -- 是否显示

        private bool _IsVisible = true;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; this.OnPropertyChanged("IsVisible"); }
        }

        #endregion

        #region VisibleIndex -- 列位序

        private int _VisibleIndex;
        /// <summary>
        /// 列位序
        /// </summary>
        public int VisibleIndex
        {
            get { return _VisibleIndex; }
            set { _VisibleIndex = value; this.OnPropertyChanged("VisibleIndex"); }
        }

        public bool IsFileNameColor { get; set; }
        public bool IsFilex { get; set; }

        #endregion

        #region IsSupportDetail -- 是否支持显示详细信息

        private bool _IsSupportDetail;
        /// <summary>
        /// 是否支持显示详细信息
        /// </summary>
        public bool IsSupportDetail
        {
            get { return _IsSupportDetail; }
            set { _IsSupportDetail = value; this.OnPropertyChanged("IsSupportDetail"); }
        }

        #endregion

        #region Foreground -- 字体颜色

        private Brush _Foreground;
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Brush Foreground
        {
            get { return _Foreground; }
            set { _Foreground = value; this.OnPropertyChanged("Foreground"); }
        }

        #endregion

        #region FontSize -- 字体大小

        private double? _FontSize;
        /// <summary>
        /// 字体大小
        /// </summary>
        public double? FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; this.OnPropertyChanged("FontSize"); }
        }

        #endregion

        #region FontFamily -- 字体

        private FontFamily _FontFamily;
        /// <summary>
        /// 字体
        /// </summary>
        public FontFamily FontFamily
        {
            get { return _FontFamily; }
            set { _FontFamily = value; this.OnPropertyChanged("FontFamily"); }
        }

        #endregion

        #region TextAlignment -- 文本对齐方式

        private TextAlignment _TextAlignment;
        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return _TextAlignment; }
            set { _TextAlignment = value; this.OnPropertyChanged("TextAlignment"); }
        }

        #endregion

        #region CellMaxHeight -- 单元格最大高度

        private double? _CellMaxHeight;
        /// <summary>
        /// 单元格最大高度
        /// </summary>
        public double? CellMaxHeight
        {
            get { return _CellMaxHeight; }
            set { _CellMaxHeight = value; this.OnPropertyChanged("CellMaxHeight"); }
        }

        #endregion

        #region Format -- 格式化字符串

        private string _Format;
        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string Format
        {
            get { return _Format; }
            set { _Format = value; this.OnPropertyChanged("Format"); }
        }

        #endregion

        #region ContentWidth -- 内容宽度

        private double _ContentWidth = double.NaN;
        /// <summary>
        /// 内容宽度
        /// </summary>
        public double ContentWidth
        {
            get { return _ContentWidth; }
            set { _ContentWidth = value; this.OnPropertyChanged("ContentWidth"); }
        }

        #endregion

        #region ContentHeight -- 内容高度

        private double _ContentHeight = double.NaN;
        /// <summary>
        /// 内容高度
        /// </summary>
        public double ContentHeight
        {
            get { return _ContentHeight; }
            set { _ContentHeight = value; this.OnPropertyChanged("ContentHeight"); }
        }

        #endregion
    }
}
