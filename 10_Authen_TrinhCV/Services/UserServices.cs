using _10_Authen_TrinhCV.Data.DB.EF;
using _10_Authen_TrinhCV.Data.DB.Entities;
using _10_Authen_TrinhCV.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace _10_Authen_TrinhCV.Services
{
    public class UserServices : IUserServices
    {
        private readonly AuthDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserServices(AuthDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<int> CreateAsync(UserRequest userRequest)
        {
            var userExisted = await GetAsync(new UserRequest() { Username = userRequest.Username });
            if (userExisted == null || userExisted.Id == 0)
            {
                return 0;
            }
            User user = new User()
            {
                Username = userRequest.Username.Trim(),
                Password = EncryptPassword(userRequest.Password.Trim()),
                Email = userRequest.Email,
                Fullname = userRequest.Fullname
            };
            var userAdded = await _dbContext.Users.AddAsync(user);

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return userAdded.Entity.Id;
            }
            return 0;
        }

        public async Task<User> UpdateAsync(UserRequest userRequest, bool updatePassword = false)
        {
            var userExisted = await GetAsync(new UserRequest() { Username = userRequest.Username });
            if (userExisted == null || userExisted.Id == 0)
            {
                return null;
            }
            if (updatePassword)
            {
                User updateUserPassword = new User()
                {
                    Id = userRequest.Id,
                    Password = EncryptPassword(userRequest.Password)
                };
                _dbContext.Users.Attach(updateUserPassword);
                _dbContext.Entry(updateUserPassword).Property(u => u.Password).IsModified = true;
            }
            else
            {
                User user = new User()
                {
                    Id = userRequest.Id,
                    Username = userRequest.Username.Trim(),
                    Email = userRequest.Email,
                    Fullname = userRequest.Fullname
                };
                _dbContext.Users.Attach(user);
                if (!string.IsNullOrWhiteSpace(userRequest.Fullname))
                {
                    _dbContext.Entry(user).Property(u => u.Fullname).IsModified = true;
                }
                if (!string.IsNullOrWhiteSpace(userRequest.Email))
                {
                    _dbContext.Entry(user).Property(u => u.Email).IsModified = true;
                }
                if (!string.IsNullOrWhiteSpace(userRequest.Username))
                {
                    _dbContext.Entry(user).Property(u => u.Username).IsModified = true;
                }
            }
            var success = await _dbContext.SaveChangesAsync() > 0;
            if (success)
            {
                return await GetAsync(new UserRequest() { Id = userRequest.Id });
            }

            return null;
        }


        public async Task<User> GetAsync(UserRequest user)
        {
            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                return _dbContext.Users.Where(u => u.Username.Trim() == user.Username.Trim()).FirstOrDefault();
            }
            else if (!string.IsNullOrWhiteSpace(user.Email))
            {
                return _dbContext.Users.Where(u => u.Email == user.Email.Trim()).FirstOrDefault();
            }
            else if (!string.IsNullOrWhiteSpace(user.Fullname))
            {
                return _dbContext.Users.Where(u => u.Fullname == user.Fullname.Trim()).FirstOrDefault();
            }
            else if (user.Id > 0)
            {
                return await _dbContext.Users.FindAsync(user.Id);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            if (userId == 0)
            {
                return false;
            }
            var user = await GetAsync(new UserRequest() { Id = userId });
            if (user != null && user.Id > 0)
            {
                _dbContext.Users.Remove(user);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> VerifyUser(UserLogin user)
        {
            User? userExisted = await GetAsync(new UserRequest() { Username = user.Username });
            if (userExisted != null && userExisted.Id > 0)
            {
                return VerifyPassword(userExisted.Password.Trim(), user.Password.Trim());
            }
            return false;
        }

        private string EncryptPassword(string password)
        {
            string encryptedPassw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(_configuration["Parameters:Salt"]),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
            return encryptedPassw;
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            string encryptedPassw = EncryptPassword(storedPassword);
            return encryptedPassw == enteredPassword;
        }
    }
}
