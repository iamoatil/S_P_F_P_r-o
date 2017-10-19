using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;

namespace XLY.SF.Project.Models
{
    public interface IDatabaseContext
    {
        IQueryable<OperationLog> OperationLogs { get; }

        IQueryable<UserInfo> UserInfos { get; }

        IQueryable<RecentCase> RecentCases { get; }

        Boolean Add<TModel>(TModel model)
            where TModel : LogicalModelBase;

        void AddRange<TModel>(params TModel[] models)
            where TModel : LogicalModelBase;

        Boolean Remove<TModel>(TModel model)
            where TModel : LogicalModelBase;

        void RemoveRange<TModel>(params TModel[] models)
            where TModel : LogicalModelBase;

        Boolean Update<TModel>(TModel model)
            where TModel : LogicalModelBase;
    }
}
