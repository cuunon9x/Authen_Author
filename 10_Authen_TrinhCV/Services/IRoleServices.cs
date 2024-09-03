using _10_Authen_TrinhCV.Data.DB.Entities;
using _10_Authen_TrinhCV.Models;

namespace _10_Authen_TrinhCV.Services
{
    public interface IRoleServices
    {
        Task<Role> GetRoleAsync(RoleRequest model);
        Task<UserRole> GetUserRoleAsync(int userId, int roleId);
        Task<UserRole> GetRolesForUserAsync(int userId);
        Task<string> GetRolenameAsync(string userName);
        Task<int> CreateAsync(int userId, int roleId);
        Task<bool> DeleteAsync(int userId, int roleId);
    }
}
