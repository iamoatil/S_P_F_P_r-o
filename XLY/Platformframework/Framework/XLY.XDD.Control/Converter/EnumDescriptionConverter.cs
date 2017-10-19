using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 枚举说明转化器
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum)
            {
                return GetDescriptionX((Enum)value);
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

        /// <summary>
        /// 获取枚举的描述信息
        /// </summary>
        public string GetDescriptionX(Enum value)
        {
            string name = value.ToString();
            FieldInfo field = value.GetType().GetField(name);
            if (field == null)
            {
                return name;
            }
            object attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            DescriptionAttribute description = (DescriptionAttribute)attribute;
            return description.Description;
        }
    }
}
