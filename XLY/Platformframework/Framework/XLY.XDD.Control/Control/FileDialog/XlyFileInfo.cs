using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control.Control
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class XlyFileInfo : ViewModelBase
    {
        #region Name -- 文件名

        private string _Name;
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; this.OnPropertyChanged("Name"); }
        }

        #endregion

        #region CreateTime -- 创建日期

        private DateTime _CreateTime;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime
        {
            get { return _CreateTime; }
            set { _CreateTime = value; this.OnPropertyChanged("CreateTime"); }
        }

        #endregion

        #region EditTime -- 编辑日期

        private DateTime _EditTime;
        /// <summary>
        /// 编辑日期
        /// </summary>
        public DateTime EditTime
        {
            get { return _EditTime; }
            set { _EditTime = value; this.OnPropertyChanged("EditTime"); }
        }

        #endregion

        #region Size -- 文件大小

        private long _Size;
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size
        {
            get { return _Size; }
            set { _Size = value; this.OnPropertyChanged("Size"); }
        }

        #endregion

        #region IsHidden -- 是否是隐藏文件

        private bool _IsHidden;
        /// <summary>
        /// 是否是隐藏文件
        /// </summary>
        public bool IsHidden
        {
            get { return _IsHidden; }
            set { _IsHidden = value; this.OnPropertyChanged("IsHidden"); }
        }

        #endregion

        #region Type -- 文件信息类型

        private XlyFileInfoType _Type;
        /// <summary>
        /// 文件信息类型
        /// </summary>
        public XlyFileInfoType Type
        {
            get { return _Type; }
            set { _Type = value; this.OnPropertyChanged("Type"); }
        }

        #endregion
    }
}
