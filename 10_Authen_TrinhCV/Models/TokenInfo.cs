namespace _10_Authen_TrinhCV.Models
{
    /// <summary>
    /// Security information on a web service access token
    /// </summary>
    public class TokenInfo 
    {
        public string Id { get; set; }
        public string ExpiresIn { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string ValidTo { get; set; }
    }
}
