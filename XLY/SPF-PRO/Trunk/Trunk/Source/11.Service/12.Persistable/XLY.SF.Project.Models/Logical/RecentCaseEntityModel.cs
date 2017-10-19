using System;
using System.ComponentModel.DataAnnotations;
using XLY.SF.Project.Models.Entities;

namespace XLY.SF.Project.Models.Logical
{
    public class RecentCaseEntityModel : LogicalModelBase<RecentCase>
    {
        #region Constructors

        public RecentCaseEntityModel(RecentCase entity)
            : base(entity)
        {
        }

        public RecentCaseEntityModel()
        {
        }

        #endregion

        #region Properties

        [Required]
        public Int32 ID => Entity.ID;

        [Required]
        [StringLength(36)]
        public String CaseID
        {
            get => Entity.CaseID;
            set => Entity.CaseID = value;
        }

        [Required]
        [StringLength(30)]
        public String Name
        {
            get => Entity.Name;
            set => Entity.Name = value;
        }

        [Required]
        [StringLength(100)]
        public String Number
        {
            get => Entity.Number;
            set => Entity.Number = value;
        }

        [Required]
        [StringLength(10)]
        public String Author
        {
            get => Entity.Author;
            set => Entity.Author = value;
        }

        [Required]
        [StringLength(30)]
        public String Type
        {
            get => Entity.Type;
            set => Entity.Type = value;
        }

        [Required]
        public DateTime Timestamp
        {
            get => Entity.Timestamp;
            set => Entity.Timestamp = value;
        }

        [Required]
        public String CaseProjectFile
        {
            get => Entity.CaseProjectFile;
            set => Entity.CaseProjectFile = value;
        }

        #endregion
    }
}
