using _10_Authen_TrinhCV.Filters;
using _10_Authen_TrinhCV.Models;
using _10_Authen_TrinhCV.Services;
using Microsoft.AspNetCore.Mvc;

namespace _10_Authen_TrinhCV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [JwtAuthenticationAttribute("Normal")]
    public class UserInfoController : ControllerBase
    {
        private readonly IRoleServices _roleServices;
        private readonly IUserServices _userServices;
        public UserInfoController(IRoleServices roleServices, IUserServices userServices)
        {
            _roleServices = roleServices;
            _userServices = userServices;
        }
        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword(UserRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid!");
            }
            var user = _userServices.UpdateAsync(model, true);
            if (user == null || user.Id <= 0)
            {
                return BadRequest();
            }
            return Ok(user);
        }
        [HttpPut("updateInfo")]
        public async Task<IActionResult> UpdateInfo(UserRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid!");
            }
            var user = _userServices.UpdateAsync(model, false);
            if (user == null || user.Id <= 0)
            {
                return BadRequest();
            }
            return Ok(user);
        }
    }
}
