// ***********************************************************************
// Assembly:XLY.SF.Project.Domains.Contract.DataItems
// Author:Songbing
// Created:2017-06-29 15:57:01
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据库数据读取，支持迭代
    /// </summary>
    public class DbDataReader : IEnumerator, IDisposable
    {
        private SQLiteDataReader Reader { get; set; }
        private SQLiteConnection _connection { get; set; }

        private string ConnectionStr { get; set; }

        private string CommandSql { get; set; }

        public DbDataReader(string connectionStr, string sql)
        {
            ConnectionStr = connectionStr;
            CommandSql = sql;

            Reset();
        }

        public object Current => Reader.GetValues();

        public bool MoveNext()
        {
            if (Reader == null)
            {
                Reset();
            }
            return Reader.Read();
        }

        public void Reset()
        {
            Dispose();

            var _connection = new SQLiteConnection(ConnectionStr);
            _connection.Open();
            using (var com = new SQLiteCommand(CommandSql, _connection))
            {
                Reader = com.ExecuteReader();
            }
        }

        public void Dispose()
        {
            if (null != Reader)
            {
                Reader.Close();
                Reader = null;
            }
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }
    }

    /// <summary>
    /// 数据库数据读取，支持迭代
    /// </summary>
    public class DbDataReader<T> : IEnumerator<T>, IDisposable where T : AbstractDataItem
    {
        private SQLiteDataReader Reader { get; set; }
        private SQLiteConnection _connection { get; set; }

        private string ConnectionStr { get; set; }

        public string CommandSql { get; set; }

        public DbDataReader(string connectionStr, string sql)
        {
            ConnectionStr = connectionStr;
            CommandSql = sql;

            Reset();
        }

        public T Current => Serializer.JsonDeserilize<T>(Reader.GetString(0));

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if(Reader == null)
            {
                Reset();
            }
            return Reader.Read();
        }

        public void Reset()
        {
            Dispose();

            var _connection = new SQLiteConnection(ConnectionStr);
            _connection.Open();
            using (var com = new SQLiteCommand(CommandSql, _connection))
            {
                Reader = com.ExecuteReader();
            }
        }

        public void Dispose()
        {
            if (null != Reader)
            {
                Reader.Close();
                Reader = null;
            }
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
