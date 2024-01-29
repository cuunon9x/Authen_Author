using _10_Authen_TrinhCV.Models;
using _10_Authen_TrinhCV.Services;
using _10_Authen_TrinhCV.ServicesCommon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _10_Authen_TrinhCV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IRoleServices _roleServices;
        private readonly IUserServices _userServices;
        private ISecurityService _securityService;
        public LoginController(ISecurityService securityService, IRoleServices roleServices, IUserServices userServices)
        {
            _roleServices = roleServices;
            _userServices = userServices;
            _securityService = securityService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid!");
            }
            var isUser = await _userServices.VerifyUser(model);
            if (!isUser) { return Unauthorized(); }
            var roleName = await _roleServices.GetRolenameAsync(model.Username);
            var user = await _userServices.GetAsync(new UserRequest() { Username = model.Username });
            var token = _securityService.CreateToken(model.Username, roleName, user.Email, user.Id);
            return Ok(token);
        }
      
    }
}
