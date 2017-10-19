using System;
using System.ComponentModel;
using System.Reflection;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 17:43:20
 * 类功能说明：
 *
 *************************************************/

namespace X86DllServer.Service
{
    public static partial class BaseTypeExtension
    {
        #region 获取说明信息

        /// <summary>
        /// 获取枚举的描述信息(Descripion)。
        /// 支持位域，如果是位域组合值，多个按分隔符组合。
        /// </summary>
        public static string[] GetDescription(this Enum enumValue)
        {
            var enumTmpValues = enumValue.ToString().Split(',');
            string[] result = new string[enumTmpValues.Length];
            var enumType = enumValue.GetType();
            for (int i = 0; i < result.Length; i++)
            {
                var fieldTmp = enumType.GetField(enumTmpValues[i].Trim());
                var att = System.Attribute.GetCustomAttribute(fieldTmp, typeof(DescriptionAttribute), false);
                result[i] = att == null ? string.Empty : ((DescriptionAttribute)att).Description;
            }
            return result;
        }

        #endregion

        public static string GetDescriptions(this Enum @this, string separator = ",")
        {
            var names = @this.ToString().Split(',');
            string[] res = new string[names.Length];
            var type = @this.GetType();
            for (int i = 0; i < names.Length; i++)
            {
                var field = type.GetField(names[i].Trim());
                if (field == null) continue;
                res[i] = GetDescription(field);
            }
            return string.Join(separator, res);
        }
        private static string GetDescription(FieldInfo field)
        {
            var att = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute), false);
            return att == null ? string.Empty : ((DescriptionAttribute)att).Description;
        }

        #region [位域]（位域枚举是否包含指定的值）

        /// <summary>
        /// 位域枚举是否包含指定的值，true：包含。
        /// .net中可以直接使用HasFlag判定
        /// </summary>
        public static bool Has(this Enum value, Enum target)
        {
            Type valueType = value.GetType();
            Type targetType = target.GetType();
            if (Enum.IsDefined(valueType, value) &&
                Enum.IsDefined(targetType, target) &&
                valueType == targetType)
            {
                return (value.GetHashCode() & target.GetHashCode()) == target.GetHashCode();
            }
            return false;
        }
        #endregion

        #region [位域]除去位域枚举指定的一个枚举（此方法不应该为扩展方法）
        ///// <summary>
        ///// 除去位域枚举指定的一个枚举，返回处理后的枚举值
        ///// </summary>
        //public static T Remove<T>(this Enum value, Enum target)
        //    where T : Enum
        //{
        //    Type valueType = value.GetType();
        //    Type targetType = target.GetType();
        //    if (Enum.IsDefined(valueType, value) &&
        //        Enum.IsDefined(targetType, target) &&
        //        valueType == targetType)
        //    {

        //    }

        //    var a = value.GetHashCode() & (~target.GetHashCode());
        //    return a.ToEnum<T>();
        //}
        #endregion
    }
}
