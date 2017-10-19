using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XLY.SF.Project.Models.Entities
{
    [Table("RecentCases")]
    public class RecentCase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 ID { get; set; }

        [Required]
        [StringLength(36)]
        public String CaseID { get; set; }

        [Required]
        [MaxLength(100)]
        public String Name { get; set; }

        [Required]
        [MaxLength(100)]
        public String Number { get; set; }

        [Required]
        [MaxLength(10)]
        public String Author { get; set; }

        [Required]
        [MaxLength(20)]
        public String Type { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public String CaseProjectFile { get; set; }
    }
}
