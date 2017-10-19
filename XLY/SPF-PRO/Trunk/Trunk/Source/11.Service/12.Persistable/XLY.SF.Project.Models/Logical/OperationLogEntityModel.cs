using System;
using System.ComponentModel.DataAnnotations;
using XLY.SF.Project.Models.Entities;

namespace XLY.SF.Project.Models.Logical
{
    public class OperationLogEntityModel : LogicalModelBase<OperationLog>
    {
        #region Constructors

        public OperationLogEntityModel(OperationLog entity)
            : base(entity)
        {
        }

        public OperationLogEntityModel()
        {
        }

        #endregion

        #region Properties

        public Int64 OperationID => Entity.OperationID;

        [Required]
        [StringLength(50)]
        public string OperationContent
        {
            get => Entity.OperationContent;
            set
            {
                Entity.OperationContent = value;
                OnPropertyChanged();
            }
        }

        [Required]
        public DateTime OperationDateTime
        {
            get => Entity.OperationDateTime;
            set
            {
                Entity.OperationDateTime = value;
                OnPropertyChanged();
            }
        }

        public String ScreenShotPath
        {
            get => Entity.ScreenShotPath;
            set
            {
                Entity.ScreenShotPath = value;
                OnPropertyChanged();
            }
        }

        [Required]
        public UserInfoEntityModel OperationUser
        {
            get;
            set;
        }

        #endregion
    }
}
