using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ValidationBase;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/16 13:36:38
 * 类功能说明：实体数据基类，已继承验证和更新接口
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.EntityBase
{
    /// <summary>
    /// 实体数据基类，已继承验证和更新属性接口
    /// </summary>
    public abstract class DbEntityBase<TModel>
        where TModel : DbModelBase
    {
        public abstract TModel ToModel();
    }
}
