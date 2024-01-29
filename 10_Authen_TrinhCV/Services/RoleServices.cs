using _10_Authen_TrinhCV.Data.DB.EF;
using _10_Authen_TrinhCV.Data.DB.Entities;
using _10_Authen_TrinhCV.Models;
using System.Data;

namespace _10_Authen_TrinhCV.Services
{
    public class RoleServices : IRoleServices
    {
        private readonly AuthDbContext _dbContext;
        private readonly IUserServices _userServices;

        public RoleServices(AuthDbContext dbContext, IUserServices userServices)
        {
            _dbContext = dbContext;
            _userServices = userServices;
        }

        public async Task<int> CreateAsync(int userId, int roleId)
        {
            if (userId == 0 || roleId == 0)
            {
                return 0;
            }
            var role = await GetRoleAsync(new RoleRequest() { Id = roleId });
            var user = await _userServices.GetAsync(new UserRequest() { Id = userId });
            if (role == null || role.Id <= 0 || user == null || user.Id <= 0)
            {
                return 0;
            }
            var userRole = await GetUserRoleAsync(userId, roleId);
            if (userRole == null)
            {
                var userRoleAdded = await _dbContext.UserRoles.AddAsync(new UserRole() { RoleId = roleId, UserId = userId });
                await _dbContext.SaveChangesAsync();
                return userRoleAdded.Entity.Id;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> DeleteAsync(int userId, int roleId)
        {
            if (userId == 0 || roleId == 0)
            {
                return false;
            }
            var role = await GetRoleAsync(new RoleRequest() { Id = roleId });
            var user = await _userServices.GetAsync(new UserRequest() { Id = userId });
            if (role == null || role.Id <= 0 || user == null || user.Id <= 0)
            {
                return false;
            }
            var userRole = await GetUserRoleAsync(userId, roleId);
            if (userRole != null)
            {
                _dbContext.UserRoles.Remove(userRole);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            else
            {
                return false;
            }
        }
        public async Task<UserRole> GetRolesForUserAsync(int userId)
        {
            if (userId > 0)
            {
                return _dbContext.UserRoles.Where(r => r.UserId == userId).FirstOrDefault();
            }
            return null;
        }
        public async Task<UserRole> GetUserRoleAsync(int userId, int roleId)
        {
            if (userId > 0 && roleId > 0)
            {
                return _dbContext.UserRoles.Where(r => r.UserId == userId && r.RoleId == roleId).FirstOrDefault();
            }
            return null;
        }
        public async Task<string> GetRolenameAsync(string userName)
        {
            var user = await _userServices.GetAsync(new UserRequest() { Username = userName });
            var userRoles = await GetRolesForUserAsync(user.Id);
            var role = await GetRoleAsync(new RoleRequest() { Id = userRoles.Id });
            if (role == null)
            {
                return string.Empty;
            }
            return role.Code;
        }
        public async Task<Role> GetRoleAsync(RoleRequest role)
        {
            if (!string.IsNullOrWhiteSpace(role.Code))
            {
                return _dbContext.Roles.Where(r => r.Code.ToLower() == r.Code.Trim().ToLower()).FirstOrDefault();
            }
            else if (role.Id > 0)
            {
                return await _dbContext.Roles.FindAsync(role.Id);
            }
            else
            {
                return null;
            }
        }
    }
}
