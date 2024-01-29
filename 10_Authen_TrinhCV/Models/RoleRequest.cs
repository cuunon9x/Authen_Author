using System.ComponentModel.DataAnnotations;

namespace _10_Authen_TrinhCV.Models
{
    public class RoleRequest
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Code { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }
    }
}
