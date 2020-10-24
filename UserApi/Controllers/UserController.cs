using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApplication.Dtos.Request;
using UserApplication.Services;
using UserApplication.ViewModels;

namespace UserApi.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Route("/api/v{version:apiVersion}/user")]
    [Authorize(Roles="User")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize("user:self_read")]
        public async Task<IActionResult> Retrieve()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == null) return Unauthorized();

            Result<UserViewModel> result =
                await _userService.RetrieveByUuidAsync(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            if (result.IsFailed && result.Errors.Exists(e => e.HasMetadata("errCode", "errUserNotFound")))
                return NotFound("The user is not found");

            return Ok(result.ValueOrDefault);
        }

        [Authorize("user:self_update")]
        [HttpPost]
        public async Task<IActionResult> Complete([FromForm] string countryUuid)
        {
                        
            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == null) return Unauthorized();

            Result result = await _userService.CompleteProfileAsync(
                new CompleteUserProfileDto(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "",
                    Guid.Parse(countryUuid)));

            if (result.IsFailed && result.Errors.Exists(e => e.HasMetadata("errCode", "errUserNotFound")))
                return NotFound("The user is not found");
            
            if (result.IsFailed && result.Errors.Exists(e => e.HasMetadata("errCode", "errCountryNotFound")))
                return NotFound("The country is not found");
            
            if (result.IsFailed || result.Errors.Exists(e => e.HasMetadata("errCode", "errDbSaveFail")))
                return Problem("An error occured while trying to save user into the database");
            
            return Ok();
        }

        /// <summary>
        ///     Deletes an user
        ///     DELETE /api/v1/user
        ///     {
        ///     "uuid": "{userUuid}"
        ///     }
        /// </summary>
        /// <param name="deleteUserCommand"></param>
        /// <returns></returns>
        [Authorize("user:self_delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            
            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == null) return Unauthorized();

            Result result = await _userService.DeleteAsync(new DeleteUserDto(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? ""));

            if (result.IsFailed && result.Errors.Exists(e => e.HasMetadata("errCode", "errUserNotFound")))
                return NotFound("The user is not found");
            
            if (result.IsFailed || result.Errors.Exists(e => e.HasMetadata("errCode", "errDbSaveFail")))
                return Problem("An error occured while trying to save user into the database");
            
            return Ok();
        }
    }
}