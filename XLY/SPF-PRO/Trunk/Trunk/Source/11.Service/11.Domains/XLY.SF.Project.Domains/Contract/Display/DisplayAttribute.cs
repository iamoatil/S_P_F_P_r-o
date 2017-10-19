using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    #region DisplayAttribute（属性的显示设置特性）

    /// <summary>
    /// 数据类型
    /// </summary>
    public enum DisplayDataType
    {
        TEXT,
        INTEGER,
        REAL,
        BLOB
    }

    /// <summary>
    /// 属性的显示设置特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayAttribute : Attribute
    {
        public DisplayAttribute(String text)
        {
            Text = text;
        }

        public DisplayAttribute()
        {

        }

        /// <summary>
        /// 文本的语言资源Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 对齐方式，默认左对齐
        /// </summary>
        public EnumAlignment Alignment { get; set; }

        /// <summary>
        /// 列的数据类型
        /// </summary>
        public EnumColumnType ColumnType { get; set; }

        /// <summary>
        /// 列标题
        /// </summary>
        public String Text { get; set; }

        public Type type { get; set; }

        /// <summary>
        /// 显示格式化
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 数据库中存储的列类型
        /// </summary>
        public DisplayDataType DataType
        {
            get
            {
                switch (ColumnType)
                {
                    case EnumColumnType.DateTime:
                        return DisplayDataType.REAL;
                    case EnumColumnType.Double:
                        return DisplayDataType.REAL;
                    case EnumColumnType.Int:
                        return DisplayDataType.INTEGER;
                    default:
                        return DisplayDataType.TEXT;
                }
            }
        }

        /// <summary>
        /// 列可见性
        /// </summary>
        public EnumDisplayVisibility Visibility { get; set; } = EnumDisplayVisibility.Visible;

        ///// <summary>
        ///// 是否支持检索，默认支持true
        ///// </summary>
        //public bool FullTextSearchEnable { get; set; }

        public PropertyInfo Owner { get; set; }


        /// <summary>
        /// 获取该特性对应属性的值
        /// </summary>
        public object GetValue(object instance)
        {
            if (this.Owner == null || instance == null)
            {
                return null;
            }

            return this.Owner.GetValue(instance);
        }

    }
    #endregion
}
