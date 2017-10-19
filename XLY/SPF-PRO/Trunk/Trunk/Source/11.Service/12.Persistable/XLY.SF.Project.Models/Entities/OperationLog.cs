using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XLY.SF.Project.Models.Entities
{
    [Table("OperationLogs")]
    public class OperationLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 OperationID { get; set; }

        [Required]
        [MaxLength(50)]
        public String OperationContent { get; set; }

        public DateTime OperationDateTime { get; set; }

        public String ScreenShotPath { get; set; }

        public Int64 OperationUserID { get; set; }

        [ForeignKey("OperationUserID")]
        public UserInfo OperationUser { get; set; }
    }
}
