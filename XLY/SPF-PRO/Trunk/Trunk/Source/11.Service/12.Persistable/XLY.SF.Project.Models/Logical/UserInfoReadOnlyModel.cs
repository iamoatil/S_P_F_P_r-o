using System;
using System.ComponentModel.DataAnnotations;
using XLY.SF.Project.Models.Entities;

namespace XLY.SF.Project.Models.Logical
{
    public class UserInfoReadOnlyModel : UserInfoEntityModel
    {
        #region Constructors

        public UserInfoReadOnlyModel(UserInfo entity)
            : base(entity)
        {
        }

        #endregion

        #region Properties

        public override String UserName
        {
            get => base.UserName;
            set { }
        }

        public override String WorkUnit
        {
            get => base.WorkUnit;
            set { }
        }

        public override String IdNumber
        {
            get => base.IdNumber;
            set { }
        }

        public override String PhoneNumber
        {
            get => base.PhoneNumber;
            set { }
        }

        public override String LoginUserName
        {
            get => base.LoginUserName;
            set { }
        }

        public override String LoginPassword
        {
            get => base.LoginPassword;
            set { }
        }

        #endregion

    }
}
