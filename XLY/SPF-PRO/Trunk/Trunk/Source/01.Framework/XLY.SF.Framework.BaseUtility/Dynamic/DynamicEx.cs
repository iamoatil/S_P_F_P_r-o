using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;

/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 16:32:36
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.BaseUtility
{
    [Serializable]
    public class DynamicEx : DynamicObject
    {
        /// <summary>
        /// 存储成员键值的字典
        /// </summary>
        private Dictionary<string, object> Members;

        public DynamicEx()
        {
            Members = new Dictionary<string, object>();
        }

        /// <summary>
        /// 设置指定属性值（如属性不存在则添加）
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">属性值</param>
        /// <returns>成功返回true</returns>
        public bool Set(string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName))
                return false;
            if (Members.ContainsKey(propertyName))
                Members[propertyName] = value == DBNull.Value ? null : value;
            else
                Members.Add(propertyName, value == DBNull.Value ? null : value);
            return true;
        }

        /// <summary>
        /// 获取指定属性名称的值
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns>没有对应属性则返回null</returns>
        public object Get(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && Members.ContainsKey(propertyName))
                return Members[propertyName];
            return null;
        }

        /// <summary>
        /// 根据编码名称字典转换为格式化的字符串
        /// </summary>
        /// <param name="codenames"></param>
        /// <returns></returns>
        public string ToString(Dictionary<string, string> codenames)
        {
            if (Members.Count < 1)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();

            foreach (var item in Members)
            {
                if (item.Key != "XLYLogString")
                {
                    string name = codenames.TryGetValue(item.Key, out name) ? name : item.Key;
                    sb.Append(name).Append(" : ").Append(item.Value.ToSafeString());
                    sb.Append("\t");
                }
            }

            return sb.ToString().TrimEnd(Environment.NewLine.ToArray());
        }


        #region DynamicObject Override

        /// <summary>
        /// 获取动态成员值
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            Members.TryGetValue(binder.Name, out result);
            return true;
        }

        /// <summary>
        /// 设置动态成员值
        /// </summary>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (value == DBNull.Value)
            {
                value = null;
            }
            Members[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// 获取动态成员
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Members.Keys;
        }
        #endregion

        /// <summary>
        /// 动态类型扩展
        /// </summary>
        public DynamicEx Dynamic
        {
            get { return _Dynamic; }
            set { _Dynamic = value; }
        }
        private DynamicEx _Dynamic;
    }
}
