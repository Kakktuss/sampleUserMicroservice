using System.Threading.Tasks;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApplication.Services;

namespace UserApi.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Route("/api/v{version:apiVersion}/user/admin")]
    [Authorize(Roles = "Moderator,Administrator,Super Administrator")]
    public class UserAdminController : Controller
    {
        private readonly IUserService _userService;
        
        public UserAdminController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize("user:read")]
        [HttpGet]
        public async Task<IActionResult> Retrieve()
        {

            var result = await _userService.RetrieveAsync();

            if (result.IsFailed && result.Errors.Exists(e => e.HasMetadata("errCode", "errUsersNotFound")))
            {
                return NotFound();
            }

            return Ok(result.ValueOrDefault);
        }

        [Authorize("user:create")]
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            // TODO: Add system to create user on local and auth0

            return Ok();
        }
        
        [Authorize("user:delete")]
        [HttpDelete("{userUuid}")]
        public async Task<IActionResult> Delete(string userUuid)
        {
            
            // TODO: Add system to delete user on local and auth0

            return Ok();
        } 
    }
}