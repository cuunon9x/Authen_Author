using System.ComponentModel.DataAnnotations;

namespace _10_Authen_TrinhCV.Models
{
    public class UserRequest
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Username { get; set; }
        [Required]
        [MaxLength(256)]
        public string Password { get; set; }
        [Required]
        [MaxLength(256)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [MaxLength(256)]
        public string NewPassword { get; set; }
        [MaxLength(100)]
        public string Fullname { get; set; }
        [MaxLength(256)]
        public string Email { get; set; }
    }
}
