using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.EntityBase;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/22 17:07:56
 * 类功能说明：
 * 1. 实现数据库，增、删、改、查
 * 2. 单独多连接查询，需要自己添加实现方法
 *************************************************/

namespace XLY.SF.Project.Persistable
{
    /// <summary>
    /// 数据库管理器【单例模式】
    /// </summary>
    [Export(typeof(IDatabaseContext))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DbService : DbContext, IDatabaseContext
    {
        #region 构造

        public DbService()
            : base("System")
        {
        }

        #endregion

        #region Properties

        public DbSet<OperationLog> OperationLogs { get; set; }

        public DbSet<UserInfo> UserInfos { get; set; }

        public DbSet<RecentCase> RecentCases { get; set; }

        IQueryable<OperationLog> IDatabaseContext.OperationLogs => OperationLogs;

        IQueryable<UserInfo> IDatabaseContext.UserInfos => UserInfos;

        IQueryable<RecentCase> IDatabaseContext.RecentCases => RecentCases;

        public Boolean Add<TModel>(TModel model)
            where TModel : LogicalModelBase
        {
            DbSet set = Set(model.Entity.GetType());
            set?.Add(model.Entity);
            return SaveChanges() == 1;
        }

        public void AddRange<TModel>(params TModel[] models) 
            where TModel : LogicalModelBase
        {
            if (models.Length == 0) return;
            DbSet set = Set(models[0].Entity.GetType());
            if (set == null) return;
            set.AddRange(models);
            SaveChanges();
        }

        public Boolean Remove<TModel>(TModel model)
            where TModel : LogicalModelBase
        {
            DbSet set = Set(model.Entity.GetType());
            if (set == null) return false;
            Object attached = set.Attach(model.Entity);
            if (attached == null) return false;
            set.Remove(attached);
            return SaveChanges() == 1;
        }

        public void RemoveRange<TModel>(params TModel[] models) 
            where TModel : LogicalModelBase
        {
            if (models.Length == 0) return;
            DbSet set = Set(models[0].Entity.GetType());
            if (set == null) return;
            Object[] attaches = new Object[models.Length];
            Object attached = null;
            for (Int32 i = 0; i < models.Length; i++)
            {
                attached = set.Attach(models[i]);
                if (attached == null) return;
                attaches[i] = attached;
            }
            set.RemoveRange(attaches);
            SaveChanges();
        }

        public Boolean Update<TModel>(TModel model)
            where TModel : LogicalModelBase
        {
            DbSet set = Set(model.Entity.GetType());
            if (set == null) return false;
            Object attached = set.Attach(model.Entity);
            if (attached == null) return false;
            Entry(attached).State = EntityState.Modified;
            return SaveChanges() == 1;
        }

        #endregion
    }
}
