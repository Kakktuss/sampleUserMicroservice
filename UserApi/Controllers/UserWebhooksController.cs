using System.Threading.Tasks;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApplication.Dtos.Request;
using UserApplication.Services;

namespace UserApi.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Route("/api/v{version:apiVersion}/user/webhooks")]
    public class UserWebhooksController : Controller
    {
        
        private readonly IUserService _userService;

        public UserWebhooksController(IUserService userService)
        {
            _userService = userService;
        }
        
        /// <summary>
        ///     Creates an user
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     POST /api/v1/user/
        ///     {
        ///     "uuid": "{userUuid}",
        ///     "username": "{username]",
        ///     "email": "{email}"
        ///     }
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] string uuid,
            [FromForm] string username,
            [FromForm] string email)
        {
            Result result = await _userService.CreateAsync(new CreateUserDto(uuid, username, email));

            if (result.IsFailed && result.Errors.Exists(e => e.HasMetadata("errCode", "errUserAlreadyExists")))
                return BadRequest("The user already exists");

            if (result.IsFailed || result.Errors.Exists(e => e.HasMetadata("errCode", "errDbSaveFail")))
                return Problem("An error occured while trying to save user into the database");

            return Created($"/api/v1/user/{uuid}", "");
        }
    }
}