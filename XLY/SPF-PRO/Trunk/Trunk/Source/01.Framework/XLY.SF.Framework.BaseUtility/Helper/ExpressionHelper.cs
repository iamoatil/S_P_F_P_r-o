using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/* ==============================================================================
* Description：ExpressionFilterHelper  
* Author     ：Fhjun
* Create Date：2017/3/23 15:41:32
* ==============================================================================*/

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// 使用Expression实现对类属性的操作
    /// </summary>
    public class ExpressionHelper
    {
        static ExpressionHelper()
        {
            DicMethod["StringContains"] = typeof(ExpressionHelper).GetMethod("StringContains");
            DicMethod["StringRegex"] = typeof(ExpressionHelper).GetMethod("StringRegex");
            DicMethod["StringBuilder"] = typeof(ExpressionHelper).GetMethod("StringBuilder");
            DicMethod["SearchRegex"] = typeof(ExpressionHelper).GetMethod("SearchRegex");
            DicMethod["ListJoin"] = typeof(ExpressionHelper).GetMethod("ListJoin");
            DicMethod["Concat"] = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
        }

        private static readonly Dictionary<Type, Delegate> DicFilterDelegate = new Dictionary<Type, Delegate>();
        private static readonly Dictionary<Type, Delegate> DicMd5Delegates = new Dictionary<Type, Delegate>();
        private static readonly Dictionary<Type, Delegate> DicSearchDelegates = new Dictionary<Type, Delegate>();
        private static readonly Dictionary<string, MethodInfo> DicMethod = new Dictionary<string, MethodInfo>();
        private static readonly Dictionary<Type, MethodInfo> DicToStringMethod = new Dictionary<Type, MethodInfo>();
        private static readonly Dictionary<Type, string[]> DicTypeProperties = new Dictionary<Type, string[]>();

        /// <summary>
        /// 创建属性过滤委托，将会生成如p=>StringContains(p.Name,"12")||StringContains(p.Age.ToString(),"12")这样的lamda表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyNames"></param>
        /// <param name="comparer"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        private static Func<T, bool> CreateFilterDelegate<T>(T source, string[] propertyNames, ComparerEnum comparer,
            object searchPattern)
        {
            if (source == null)
            {
                return null;
            }
            var realType = source.GetType(); //元素的实际类型，比如List<object> source时，返回Message

            if (DicFilterDelegate.ContainsKey(realType)) //如果保存了之前的lambda表达式
            {
                return (Func<T, bool>) DicFilterDelegate[realType];
            }
            //没有保存则直接创建
            var elementtype = typeof (T); //元素类型，比如List<object> source时，返回object

            LambdaExpression lmbada = null;

            var parameter = Expression.Parameter(elementtype, "p");
            Expression comparisionExpression = null;
            var constExpression = Expression.Constant(searchPattern);
            for (int i = 0; i < propertyNames.Length; i++)
            {
                string propertyName = propertyNames[i];
                var property = realType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null)
                {
                    return null;
                }

                Expression propertyAccess = realType != elementtype
                    ? Expression.MakeMemberAccess(Expression.TypeAs(parameter, realType), property)
                    : Expression.MakeMemberAccess(parameter, property); //如果实际类型和参数类型不一样，需要使用as转换
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
                {
                    var getValueOrDefault = property.PropertyType.GetMethods().First(p => p.Name == "GetValueOrDefault");
                    propertyAccess = Expression.Call(propertyAccess, getValueOrDefault);
                }

                Expression comparisionExpression2;
                switch (comparer)
                {
                    case ComparerEnum.StringContains:
                        if (property.PropertyType != typeof (string))
                        {
                            //如果不是string类型，则调用tostring方法
                            MethodInfo miToString;
                            if (DicToStringMethod.ContainsKey(property.PropertyType))
                            {
                                miToString = DicToStringMethod[property.PropertyType];
                            }
                            else
                            {
                                miToString = property.PropertyType.GetMethod("ToString", new Type[0]);
                                DicToStringMethod[property.PropertyType] = miToString;
                            }
                            //生成p.ToString()
                            Expression expressionToString = Expression.Call(propertyAccess, miToString);

                            comparisionExpression2 = Expression.Call(DicMethod["StringContains"], expressionToString,
                                constExpression);
                        }
                        else
                        {
                            comparisionExpression2 = Expression.Call(DicMethod["StringContains"], propertyAccess,
                                constExpression);
                        }
                        break;
                    case ComparerEnum.Regex:
                        if (property.PropertyType != typeof (string))
                        {
                            MethodInfo miToString;
                            if (DicToStringMethod.ContainsKey(property.PropertyType))
                            {
                                miToString = DicToStringMethod[property.PropertyType];
                            }
                            else
                            {
                                miToString = property.PropertyType.GetMethod("ToString", new Type[0]);
                                DicToStringMethod[property.PropertyType] = miToString;
                            }
                            //生成p.ToString()
                            Expression expressionToString = Expression.Call(propertyAccess, miToString);

                            comparisionExpression2 = Expression.Call(DicMethod["StringRegex"], expressionToString,
                                constExpression);
                        }
                        else
                        {
                            comparisionExpression2 = Expression.Call(DicMethod["StringRegex"], propertyAccess,
                                constExpression);
                        }
                        break;
                    default:
                        comparisionExpression2 = Expression.Equal(propertyAccess, constExpression);
                        break;
                }
                //如果有多个属性，则添加使用"||"连接多个条件
                comparisionExpression = comparisionExpression == null
                    ? comparisionExpression2
                    : Expression.OrElse(comparisionExpression, comparisionExpression2);
            }

            lmbada = Expression.Lambda(comparisionExpression, parameter);
            DicFilterDelegate[realType] = lmbada.Compile();

            //最终将生成p=>StringContains(p.Name,"12")||StringContains(p.Age.ToString(),"12")这样的lamda表达式
            return (Func<T, bool>) DicFilterDelegate[realType];
        }

        /// <summary>
        /// 创建MD5生成委托，将会生成如p=>string.Concat(StringBuilder("Name",p.Name), StringBuilder("Age",p.Age.ToString()))这样的lamda表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        private static Func<T, string> CreateMd5Delegate<T>(T source, string[] propertyNames)
        {
            if (source == null)
            {
                return null;
            }
            var realType = source.GetType(); //元素的实际类型，比如List<object> source时，返回Message

            if (DicMd5Delegates.ContainsKey(realType)) //如果保存了之前的lambda表达式
            {
                return (Func<T, string>)DicMd5Delegates[realType];
            }
            else //没有保存则直接创建
            {
                var elementtype = typeof(T); //元素类型，比如List<object> source时，返回object
                var parameter = Expression.Parameter(elementtype, "p");
                Expression comparisionExpression = null;
                LambdaExpression lmbada;
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i] == "MD5")   //MD5列自身不计算
                    {
                        continue;
                    }
                    string propertyName = propertyNames[i];
                    var property = realType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (property == null)
                    {
                        return null;
                    }

                    Expression propertyAccess = realType != elementtype
                        ? Expression.MakeMemberAccess(Expression.TypeAs(parameter, realType), property)
                        : Expression.MakeMemberAccess(parameter, property); //如果实际类型和参数类型不一样，需要使用as转换
                    //if (property.PropertyType.IsGenericType &&
                    //    property.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
                    //{
                    //    var getValueOrDefault =
                    //        property.PropertyType.GetMethods().First(p => p.Name == "GetValueOrDefault");
                    //    propertyAccess = Expression.Call(propertyAccess, getValueOrDefault);
                    //}

                    var constExpression = Expression.Constant(propertyName);
                    Expression comparisionExpression2;
                    if (property.PropertyType != typeof (string))  //如果不是string类型，则调用ToString方法
                    {
                        MethodInfo miToString;
                        if (DicToStringMethod.ContainsKey(property.PropertyType))
                        {
                            miToString = DicToStringMethod[property.PropertyType];
                        }
                        else
                        {
                            miToString = property.PropertyType.GetMethod("ToString", new Type[0]);
                            DicToStringMethod[property.PropertyType] = miToString;
                        }
                        //生成p.ToString()
                        Expression expressionToString = Expression.Call(propertyAccess, miToString);
                        comparisionExpression2 = Expression.Call(DicMethod["StringBuilder"], constExpression,  expressionToString);
                    }
                    else
                    {
                        comparisionExpression2 = Expression.Call(DicMethod["StringBuilder"], constExpression, propertyAccess);
                    }
                    //调用string.Concat方法连接多个属性名称
                    comparisionExpression = comparisionExpression == null
                        ? comparisionExpression2
                        : Expression.Call(DicMethod["Concat"], comparisionExpression, comparisionExpression2);
                }
                //生成如 p=>string.Concat(StringBuilder("Name",p.Name), StringBuilder("Age",p.Age.ToString()))
                lmbada = Expression.Lambda(comparisionExpression, parameter);
                DicMd5Delegates[realType] = lmbada.Compile();
            }

            return (Func<T, string>)DicMd5Delegates[realType];
        }

        /// <summary>
        /// 创建MD5生成委托，将会生成如p=>string.Concat(StringBuilder("Name",p.Name), StringBuilder("Age",p.Age.ToString()))这样的lamda表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        private static Func<T, List<string>> CreateSearchDelegate<T>(T source, Regex regex, string[] propertyNames)
        {
            if (source == null)
            {
                return null;
            }
            var realType = source.GetType(); //元素的实际类型，比如List<object> source时，返回Message

            if (DicSearchDelegates.ContainsKey(realType)) //如果保存了之前的lambda表达式
            {
                return (Func<T, List<string>>)DicSearchDelegates[realType];
            }

            //没有保存则直接创建
            var elementtype = typeof (T); //元素类型，比如List<object> source时，返回object
            var parameter = Expression.Parameter(elementtype, "p");
            Expression comparisionExpression = null;
            LambdaExpression lmbada;
            var constExpression = Expression.Constant(regex);       //正则参数
            for (int i = 0; i < propertyNames.Length; i++)
            {
                string propertyName = propertyNames[i];
                var property = realType.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null)
                {
                    return null;
                }

                Expression propertyAccess = realType != elementtype
                    ? Expression.MakeMemberAccess(Expression.TypeAs(parameter, realType), property)
                    : Expression.MakeMemberAccess(parameter, property); //如果实际类型和参数类型不一样，需要使用as转换
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
                {
                    var getValueOrDefault =
                        property.PropertyType.GetMethods().First(p => p.Name == "GetValueOrDefault");
                    propertyAccess = Expression.Call(propertyAccess, getValueOrDefault);
                }

                Expression comparisionExpression2;
                if (property.PropertyType != typeof (string)) //如果不是string类型，则调用ToString方法
                {
                    MethodInfo miToString;
                    if (DicToStringMethod.ContainsKey(property.PropertyType))
                    {
                        miToString = DicToStringMethod[property.PropertyType];
                    }
                    else
                    {
                        miToString = property.PropertyType.GetMethod("ToString", new Type[0]);
                        DicToStringMethod[property.PropertyType] = miToString;
                    }
                    //生成p.ToString()
                    Expression expressionToString = Expression.Call(propertyAccess, miToString);
                    comparisionExpression2 = Expression.Call(DicMethod["SearchRegex"],expressionToString, constExpression);
                }
                else
                {
                    comparisionExpression2 = Expression.Call(DicMethod["SearchRegex"], propertyAccess, constExpression);
                }
                //调用ListJoin方法连接多个属性名称
                comparisionExpression = comparisionExpression == null
                    ? comparisionExpression2
                    : Expression.Call(DicMethod["ListJoin"], comparisionExpression, comparisionExpression2);
            }
            //生成如 p=>ListJoin(SearchRegex(p.Name,regex), SearchRegex(p.Age.ToString(),regex))
            lmbada = Expression.Lambda(comparisionExpression, parameter);
            DicSearchDelegates[realType] = lmbada.Compile();

            return (Func<T, List<string>>)DicSearchDelegates[realType];
        }

        public static bool StringContains(string value, string subValue)
        {
            if (value == null)
            {
                return false;
            }

            return value.Contains(subValue);
        }

        public static bool StringRegex(string value, Regex regex)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return regex.IsMatch(value);
        }

        public static List<string> SearchRegex(string value, Regex regex)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            List<string> res = new List<string>();
            foreach (Match m in regex.Matches(value))
            {
                res.Add(m.Value); 
            }
            return res;
        }

        public static List<string> ListJoin(List<string> list1, List<string> list2)
        {
            list1.AddRange(list2);
            return list1;
        }

        public static string StringBuilder(string s1, string s2)
        {
            return string.Format("{0}={1};", s1, s2);
        }

        /// <summary>
        /// 执行数据的过滤，输入待查询的属性列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="compare"></param>
        /// <param name="propertyNames"></param>
        /// <param name="searchText"></param>
        /// <param name="searchRegex"></param>
        /// <returns></returns>
        public static bool Filter<T>(T obj, ComparerEnum compare, string[] propertyNames, string searchText, Regex searchRegex)
        {
            Func<T, bool> fun = compare == ComparerEnum.StringContains
                ? CreateFilterDelegate(obj, propertyNames, compare, searchText)
                : CreateFilterDelegate(obj, propertyNames, compare, searchRegex);
            return fun == null ? false : fun(obj);
        }

        /// <summary>
        /// 执行数据的过滤，包含了displayAttribute特性的属性列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="compare"></param>
        /// <param name="displayAttribute"></param>
        /// <param name="searchText"></param>
        /// <param name="searchRegex"></param>
        /// <returns></returns>
        public static bool Filter<T>(T obj, ComparerEnum compare, Type displayAttribute, string searchText, Regex searchRegex)
        {
            string[] propertyNames = GetProperties(obj, displayAttribute);
            return Filter(obj, compare, propertyNames, searchText, searchRegex);
        }

        /// <summary>
        /// 执行数据的过滤，包含了全部属性计算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="compare"></param>
        /// <param name="searchText"></param>
        /// <param name="searchRegex"></param>
        /// <returns></returns>
        public static bool Filter<T>(T obj, ComparerEnum compare, string searchText, Regex searchRegex)
        {
            string[] propertyNames = GetProperties(obj,null);
            return Filter(obj, compare, propertyNames, searchText, searchRegex);
        }

        /// <summary>
        /// 计算部分属性的MD5
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static string Md5<T>(T obj, string[] propertyNames)
        {
            Func<T, string> fun = CreateMd5Delegate(obj, propertyNames);
            return fun == null ? "" : CryptographyHelper.MD5FromString(fun(obj));
        }

        /// <summary>
        /// 计算包含了displayAttribute特性的属性的MD5值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="displayAttribute"></param>
        /// <returns></returns>
        public static string Md5<T>(T obj, Type displayAttribute)
        {
            string[] propertyNames = GetProperties(obj, displayAttribute);
            Func<T, string> fun = CreateMd5Delegate(obj, propertyNames);
            string str = fun(obj);
            return fun == null ? "" : CryptographyHelper.MD5FromString(str);
        }

        /// <summary>
        /// 计算全部属性的MD5值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Md5<T>(T obj)
        {
            string[] propertyNames = GetProperties(obj, null);
            Func<T, string> fun = CreateMd5Delegate(obj, propertyNames);
            return fun == null ? "" : CryptographyHelper.MD5FromString(fun(obj));
        }

        /// <summary>
        /// 正则查询匹配数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="searchRegex"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static List<string> Search<T>(T obj,Regex searchRegex, string[] propertyNames)
        {
            Func<T, List<string>> fun = CreateSearchDelegate(obj, searchRegex,propertyNames);
            return fun == null ? null : fun(obj);
        }

        /// <summary>
        /// 正则查询匹配数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="searchRegex"></param>
        /// <returns></returns>
        public static List<string> Search<T>(T obj, Regex searchRegex)
        {
            string[] propertyNames = GetProperties(obj, null);
            Func<T, List<string>> fun = CreateSearchDelegate(obj, searchRegex, propertyNames);
            return fun == null ? null : fun(obj);
        }

        /// <summary>
        /// 正则查询匹配数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="searchRegex"></param>
        /// <param name="displayAttribute"></param>
        /// <returns></returns>
        public static List<string> Search<T>(T obj, Regex searchRegex, Type displayAttribute)
        {
            string[] propertyNames = GetProperties(obj, displayAttribute);
            Func<T, List<string>> fun = CreateSearchDelegate(obj, searchRegex, propertyNames);
            return fun == null ? null : fun(obj);
        }

        private static string[] GetProperties<T>(T obj, Type displayAttribute)
        {
            string[] propertyNames;
            Type t = obj.GetType();
            if (DicTypeProperties.ContainsKey(t))
            {
                propertyNames = DicTypeProperties[t];
            }
            else
            {
                if (displayAttribute == null)
                {
                    propertyNames =t.GetProperties()
                       .ToList()
                       .ConvertAll(k => k.Name)
                       .ToArray();
                }
                else
                {
                    propertyNames = t.GetProperties()
                      .Where(p => p.IsDefined(displayAttribute, false))
                      .ToList()
                      .ConvertAll(k => k.Name)
                      .ToArray();
                }
               
                DicTypeProperties[t] = propertyNames;
            }
            return propertyNames;
        }
    }

    public enum ComparerEnum
    {
        StringContains,
        Regex,
    }

}
