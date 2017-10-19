using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ValidationBase;

namespace XLY.SF.Framework.Core.Base.EntityBase
{
    public abstract class DbModelBase : ValidateBase
    {
    
        #region ToEntity

        public abstract object ToEntity();

        #endregion
    }
}
