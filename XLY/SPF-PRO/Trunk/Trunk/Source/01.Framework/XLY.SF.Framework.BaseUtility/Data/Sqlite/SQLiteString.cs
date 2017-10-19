using NHibernate.Util;
using System.Collections.Generic;
using System.Data.SQLite;

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// 执行SQLitel的Sql命令定义
    /// </summary>
    public class SQLiteString
    {
        public string SqlText { get; set; }
        public IList<SqlParameter> Parameters;

        public SQLiteString()
        {
            Parameters = new List<SqlParameter>();
        }

        public SQLiteString(string sqlText)
            : this()
        {
            SqlText = sqlText;
        }

        public void SetCommand(SQLiteCommand com)
        {
            Guard.ArgumentNotNull(com, "SQLiteCommand");
            com.CommandText = SqlText;
            if (Parameters != null && Parameters.Count > 0)
            {
                Parameters.ForEach(s =>
                    {
                        com.Parameters.Add(s.ToSQLiteParameter());
                    });
            }
        }
    }
}
