using _10_Authen_TrinhCV.Models;

namespace _10_Authen_TrinhCV.ServicesCommon
{
    public interface ISecurityService
    {
        string CreateToken(string userName, string roleName, string email, int id);
        TokenInfo ValidateToken(string token, out string errMsg);
    }
}
