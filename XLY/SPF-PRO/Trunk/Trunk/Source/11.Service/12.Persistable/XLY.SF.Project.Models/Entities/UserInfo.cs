using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XLY.SF.Project.Models.Entities
{
    [Table("UserInfos")]
    public class UserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 UserID { get; set; }

        [Required]
        [MaxLength(10)]
        public String UserName { get; set; }

        public String WorkUnit { get; set; }

        public String IdNumber { get; set; }

        public String PhoneNumber { get; set; }

        [Required]
        [MaxLength(10)]
        public String LoginUserName { get; set; }

        [Required]
        public String LoginPassword { get; set; }
    }
}
