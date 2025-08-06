using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SQKLocalServe.DataAccess.Entities
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }
        
        [ForeignKey("Status")]
        public int? StatusId { get; set; }
        
        [StringLength(50)]
        public string CreatedBy { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        [StringLength(50)]
        public string UpdatedBy { get; set; }
        
        public DateTime? UpdatedDate { get; set; }

        // Navigation property for Status
        public virtual Status Status { get; set; }
    }
}