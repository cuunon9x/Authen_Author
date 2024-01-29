using System.ComponentModel.DataAnnotations;

namespace _10_Authen_TrinhCV.Models
{
    public class UserLogin
    {
        [MaxLength(100)]
        [Required]
        public string Username { get; set; }
        [Required]
        [MaxLength(256)]
        public string Password { get; set; }
    }
}
