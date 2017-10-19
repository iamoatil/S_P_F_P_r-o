using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Utility.Logger;
using System.Reflection;
using System.ComponentModel;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 枚举信息
    /// </summary>
    public class EnumInfo : ViewModelBase
    {
        #region Name -- 名称

        private string _Name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; this.OnPropertyChanged("Name"); }
        }

        #endregion

        #region Value -- 值

        private object _Value;
        /// <summary>
        /// 值
        /// </summary>
        public object Value
        {
            get { return _Value; }
            set { _Value = value; this.OnPropertyChanged("Value"); }
        }

        #endregion

        #region Detail -- 描述

        private string _Detail;
        /// <summary>
        /// 描述
        /// </summary>
        public string Detail
        {
            get { return _Detail; }
            set { _Detail = value; this.OnPropertyChanged("Detail"); }
        }

        #endregion

        /// <summary>
        /// 获取枚举类型的信息
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static List<EnumInfo> GetEnumInfos(Type enumType)
        {
            try
            {
                List<EnumInfo> list = new List<EnumInfo>();
                string[] names = Enum.GetNames(enumType);
                Array array = Enum.GetValues(enumType);

                for (int i = 0; i < names.Length; ++i)
                {
                    EnumInfo info = new EnumInfo();
                    info.Name = names[i];
                    info.Value = array.GetValue(i);
                    FieldInfo fieldInfo = enumType.GetField(info.Name);
                    if (fieldInfo != null)
                    {
                        DescriptionAttribute attribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute;
                        if (attribute != null)
                            info.Detail = attribute.Description;
                    }
                    list.Add(info);
                }
                return list;
            }
            catch
            {
                return null;
            }
        }
    }
}
