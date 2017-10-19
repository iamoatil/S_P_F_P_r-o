using System;
using System.Collections;
using System.Collections.Generic;
using XLY.SF.Project.DataFilter;
using XLY.SF.Project.Domains.Contract;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据集接口
    /// </summary>
    public interface IDataItems : IFilterable
    {
        string Key { get; set; }

        SqliteDbFile DbInstance { get; }

        string DbFilePath { get; }

        string DbTableName { get; }

        IEnumerable View { get; }

        Int32 Count { get; }

        void Add(object obj);

        void Commit();

        void Filter(params FilterArgs[] args);
    }

    public interface IDataItems<out T> : IDataItems
    {
        new IEnumerable<T> View { get; }
    }
}
