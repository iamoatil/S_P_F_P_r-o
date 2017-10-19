using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XLY.SF.Project.DataFilter.Asist;

namespace XLY.SF.Project.DataFilter.Providers
{
    /// <summary>
    /// 基于SQLite数据库的数据提供器。
    /// </summary>
    public class SQLiteFilterDataProvider : SQLFilterDataProvider
    {
        #region Constructors

        static SQLiteFilterDataProvider()
        {
            SQLiteFunction.RegisterFunction(typeof(RegexMatchFunction));
        }

        public SQLiteFilterDataProvider(String file,String tableName, String password = "")
            :base(new SQLiteConnectionStringBuilder()
            {
                DataSource =file?? throw new ArgumentNullException("file"),
                Password = password
            }.ConnectionString,tableName)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Nested Type

        [SQLiteFunction(Name = "RegexMatch", FuncType = FunctionType.Scalar, Arguments = 2)]
        public class RegexMatchFunction : SQLiteFunction
        {
            public override Object Invoke(Object[] args)
            {
                return Regex.IsMatch(Convert.ToString(args[0]), Convert.ToString(args[1]));
            }
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取数据（目前暂时使用内容为SQL语句的常量表达式）。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <param name="count">集合的大小。</param>
        /// <returns>数据。</returns>
        public override IEnumerable<T> Query<T>(Expression expression, out Int32 count)
        {
            SQLiteConnection connection = new SQLiteConnection(ConnectionString);
            connection.Flags = SQLiteConnectionFlags.UseConnectionPool;
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = SQLExpressionConverter.GetCountSql(expression, TableName);
            count = (Int32)(Int64)command.ExecuteScalar();
            if (count == 0) return new T[0];

            command.CommandText = SQLExpressionConverter.GetSelectionSql(expression, TableName);
            DbDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return new DbEnumerableDataReader<T>(reader);
        }

        #endregion

        #endregion
    }
}
