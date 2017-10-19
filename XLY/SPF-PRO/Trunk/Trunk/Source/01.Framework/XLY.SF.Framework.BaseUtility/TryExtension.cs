using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Language;

namespace XLY.SF.Framework.BaseUtility
{
    public static class TryExtension
    {
        /// <summary>
        /// GetDescriptionX,获取枚举的描述信息(Descripion),支持多语言配置
        /// </summary>
        public static string GetDescriptionX(this Enum value)
        {
            //有效性验证
            Guard.ArgumentNotNull(value, "the value of System.Enum");
            string name = value.ToString();
            FieldInfo field = value.GetType().GetField(name);
            if (field == null)
            {
                return name;
            }
            string nameDexc = LanguageHelperSingle.Instance.GetEntityDescByKey(name);
            if (!String.IsNullOrEmpty(nameDexc))
            {
                return nameDexc;
            }
            else
            {
                object attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                DescriptionAttribute description = (DescriptionAttribute)attribute;
                return description.Description;
            }
        }
    }
}
