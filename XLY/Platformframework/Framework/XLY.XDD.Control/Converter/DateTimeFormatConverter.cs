using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 日期时间格式化转化器
    /// </summary>
    public class DateTimeFormatConverter : IValueConverter
    {
        #region Format -- 格式化字符串

        private string _Format = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string Format
        {
            get { return this._Format; }
            set { this._Format = value; }
        }

        #endregion

        #region IsReturnInvalidValue -- 是否返回无效的数值

        private bool _IsReturnInvalidValue = false;

        /// <summary>
        /// 是否返回无效的数值
        /// </summary>
        public bool IsReturnInvalidValue
        {
            get { return this._IsReturnInvalidValue; }
            set { this._IsReturnInvalidValue = value; }
        }

        #endregion

        #region InvalidString -- 无效时显示的字符串

        private string _InvalidString = string.Empty;
        /// <summary>
        /// 无效时显示的字符串
        /// </summary>
        public string InvalidString
        {
            get { return _InvalidString; }
            set { _InvalidString = value; }
        }

        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime || value is DateTime?)
            {
                if (value is DateTime)
                {
                    DateTime dt = (DateTime)value;
                    if ((dt == DateTime.MinValue || dt == DateTime.MaxValue) && !this.IsReturnInvalidValue)
                    {
                        return this.InvalidString;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(this.Format))
                            return dt.ToString();
                        else
                            return dt.ToString(this.Format);
                    }
                }
                else
                {
                    DateTime dt = ((DateTime?)value).Value;
                    if ((dt == DateTime.MinValue || dt == DateTime.MaxValue) && !this.IsReturnInvalidValue)
                    {
                        return this.InvalidString;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(this.Format))
                            return dt.ToString();
                        else
                            return dt.ToString(this.Format);
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
