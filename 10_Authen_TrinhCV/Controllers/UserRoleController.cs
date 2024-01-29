using _10_Authen_TrinhCV.Filters;
using _10_Authen_TrinhCV.Models;
using _10_Authen_TrinhCV.Services;
using Microsoft.AspNetCore.Mvc;

namespace _10_Authen_TrinhCV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [JwtAuthenticationAttribute("Admin")]
    public class UserRoleController : ControllerBase
    {
        private readonly IRoleServices _roleServices;
        private readonly IUserServices _userServices;
        public UserRoleController(IRoleServices roleServices, IUserServices userServices)
        {
            _roleServices = roleServices;
            _userServices = userServices;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(UserRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid!");
            }
            var id = await _userServices.CreateAsync(model);
            if (id == 0)
            {
                return BadRequest();
            }
            return Created("todo", id);
        }
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRole(UserRoleRequests model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid!");
            }
            var id = await _roleServices.CreateAsync(model.UserId, model.RoleId);
            if (id == 0)
            {
                return BadRequest();
            }
            return Created("todo", id);
        }
        [HttpDelete("deleteUserInRole")]
        public async Task<IActionResult> DeleteUserInRole(UserRoleRequests model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid!");
            }
            var isDeleted = await _roleServices.DeleteAsync(model.UserId, model.RoleId);
            if (isDeleted)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
