using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 11:01:51
 * 类功能说明：基础类型扩展方法类
 * 1.主要是系统基础类型的扩展方法
 * 2.不牵扯任何业务逻辑
 *
 *************************************************/

namespace X86DllServer.Service
{
    public static partial class BaseTypeExtension
    {
        /// <summary>
        /// 判断类型是否（Nullable）可空类型
        /// </summary>
        public static bool IsNullableType(this Type @type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 根据条件查询元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">待查询的数据源</param>
        /// <param name="match">查询条件</param>
        /// <returns></returns>
        public static T Find<T>(this LinkedList<T> list, Predicate<T> match)
        {
            return list.FirstOrDefault((t) =>
            {
                return match(t);
            });
        }

        public static LinkedList<T> FindAll<T>(this LinkedList<T> list, Predicate<T> match)
        {
            LinkedList<T> res = new LinkedList<T>();
            foreach (var item in list)
            {
                if (match(item))
                {
                    res.AddLast(item);
                }
            }
            return res;
        }

        /// <summary>
        /// 删除数组指定索引下标元素
        /// </summary>
        /// <typeparam name="T">输入：数组数据类型</typeparam>
        /// <param name="this">输入：数组数据</param>
        /// <param name="index">输入：移除数组元素对应索引下标，从0开始</param>
        public static T[] DeleteArray<T>(this T[] @this, int index)
        {
            if (null == @this)
                throw new ArgumentNullException("@this");
            if (index < 0 || (@this.Length - 1) < index)
                throw new IndexOutOfRangeException();
            var before = @this.Take(index);
            var after = @this.Skip(1).Take(@this.Length);
            return before.Concat(after).ToArray();
        }

        /// <summary>
        ///  正则验证字符是否符合规则（默认单行模式，并忽略大小写），返回true，符合规则，否则不符合。
        /// </summary>
        public static bool IsMatch(string source, String pattern)
        {
            if (String.IsNullOrWhiteSpace(source))
                return false;
            return IsMatch(source, pattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        public static bool IsMatch(string source, String pattern, RegexOptions options)
        {
            if (String.IsNullOrWhiteSpace(source))
                return false;
            return Regex.IsMatch(source, pattern, options);
        }
    }
}
