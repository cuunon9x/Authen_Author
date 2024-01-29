using _10_Authen_TrinhCV.Data.DB.Entities;
using _10_Authen_TrinhCV.Models;

namespace _10_Authen_TrinhCV.Services
{
    public interface IUserServices
    {
        Task<User> GetAsync(UserRequest user);
        Task<int> CreateAsync(UserRequest user);
        Task<User> UpdateAsync(UserRequest userRequest, bool updatePassword = false);
        Task<bool> DeleteAsync(int userId);
        Task<bool> VerifyUser(UserLogin user);
    }
}
