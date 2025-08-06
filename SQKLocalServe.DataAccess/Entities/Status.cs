using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SQKLocalServe.DataAccess.Entities
{
    [Table("Status")]
    public class Status
    {
        [Key]
        public int StatusId { get; set; }
        
        public virtual ICollection<Role> Roles { get; set; }
    }
}