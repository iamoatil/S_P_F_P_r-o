using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Models.Logical;

namespace XLY.SF.Project.Models
{
    public static class QueryableExtensions
    {
        public static IQueryable<TModel> ToModels<TEntity, TModel>(this IQueryable<TEntity> source)
            where TModel : LogicalModelBase, new()
            where TEntity : class
        {
            return source.Select(x => new TModel { Entity = x });
        }

        public static TModel ToModel<TEntity, TModel>(this TEntity entity)
            where TModel : LogicalModelBase, new()
            where TEntity : class
        {
            if (entity == null) return null;
            return new TModel { Entity = entity };
        }
    }
}
