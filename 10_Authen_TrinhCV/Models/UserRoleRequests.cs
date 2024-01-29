using System.ComponentModel.DataAnnotations;

namespace _10_Authen_TrinhCV.Models
{
    public class UserRoleRequests
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoleId { get; set; }
    }
}
