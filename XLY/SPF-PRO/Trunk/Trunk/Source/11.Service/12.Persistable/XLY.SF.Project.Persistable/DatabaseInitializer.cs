using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Persistable
{
    internal class DatabaseInitializer : DropCreateDatabaseIfModelChanges<DbService>
    {
        protected override void Seed(DbService context)
        {
        }
    }
}
