using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.DataFilter.Providers
{
    /// <summary>
    /// 基于SQL语句的数据提供器。
    /// </summary>
    public abstract class SQLFilterDataProvider : IFilterDataProvider
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataFilter.Providers.SQLDataProvider 实例。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <param name="tableName">表名。</param>
        protected SQLFilterDataProvider(String connectionString, String tableName)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException("connectionString");
            TableName = tableName ?? throw new ArgumentNullException("tableName");
        }

        #endregion

        #region Properties

        /// <summary>
        /// 表名。
        /// </summary>
        public String TableName { get; }

        /// <summary>
        /// 数据库连接字符串。
        /// </summary>
        public String ConnectionString { get; }

        #endregion

        #region Nested Type

        /// <summary>
        ///可枚举的数据读取器。
        /// </summary>
        public class DbEnumerableDataReader<T> : IEnumerable<T>
        {
            #region Fields

            private DbDataReader _reader;

            private readonly Dictionary<String, String> _temp;

            private static readonly Dictionary<Type, String> DefaultValueCache;

            private readonly MethodInfo _defaultValueMethodInfo;

            private T[] _cache = new T[0];

            private Boolean _isFirstRead = true;

            #endregion

            #region Constructors

            static DbEnumerableDataReader()
            {
                DefaultValueCache = new Dictionary<Type, String>();
            }

            internal DbEnumerableDataReader(DbDataReader reader)
            {
                _reader = reader ?? throw new ArgumentNullException("reader");
                _temp = new Dictionary<String, String>();
                List<KeyValuePair<String, Type>> columns = new List<KeyValuePair<String, Type>>();
                String name;
                Type type;
                for (Int32 i = 0; i < _reader.FieldCount; i++)
                {
                    name = _reader.GetName(i);
                    type = _reader.GetFieldType(i);
                    columns.Add(new KeyValuePair<String, Type>(name, type));
                    _temp.Add(name, null);
                }
                Columns = columns.ToArray();
                _defaultValueMethodInfo = this.GetType().GetMethod("MakeDefaultGeneric", BindingFlags.NonPublic|BindingFlags.Static);
            }

            #endregion

            #region Properties

            public KeyValuePair<String,Type>[] Columns { get; }

            #endregion

            #region Methods

            #region Public

            public IEnumerator<T> GetEnumerator()
            {
                if (_isFirstRead)
                {
                    return ReadDataDynamically().GetEnumerator();
                }
                else
                {
                    return ((IEnumerable<T>)_cache).GetEnumerator();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region Private

            private String MakeDefault(Type type)
            {
                if (DefaultValueCache.ContainsKey(type))
                {
                    return DefaultValueCache[type];
                }
                MethodInfo mi = _defaultValueMethodInfo.MakeGenericMethod(type);
                String defaultValue = (mi.Invoke(null, null) ?? String.Empty).ToString();
                DefaultValueCache.Add(type, defaultValue);
                return defaultValue;
            }

            private static _T MakeDefaultGeneric<_T>()
            {
                return default(_T);
            }

            private IEnumerable<T> ReadDataDynamically()
            {
                _isFirstRead = false;
                try
                {
                    Object[] values = new Object[_reader.FieldCount];
                    String json = null;
                    T temp = default(T);
                    List<T> list = new List<T>();
                    while (_reader.Read())
                    {
                        _reader.GetValues(values);
                        for (Int32 i = 0; i < Columns.Length; i++)
                        {
                            if (values[i] == null)
                            {
                                _temp[Columns[i].Key] = MakeDefault(Columns[i].Value);
                            }
                            else
                            {
                                _temp[Columns[i].Key] = values[i].ToString();
                            }
                        }
                        json = JsonConvert.SerializeObject(_temp);
                        temp = JsonConvert.DeserializeObject<T>(json);
                        list.Add(temp);
                        yield return temp;
                    }
                    _cache = list.ToArray();
                }
                finally
                {
                    _reader.Close();
                    _reader = null;
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取数据。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <param name="count">集合的大小。</param>
        /// <returns>数据。</returns>
        public abstract IEnumerable<T> Query<T>(Expression expression,out Int32 count);

        #endregion

        #endregion
    }
}
