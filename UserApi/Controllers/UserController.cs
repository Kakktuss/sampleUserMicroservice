using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserApplication.Commands.Command;

namespace UserApi.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Route("/api/v{version:apiVersion}/user")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
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
            Result result = await _mediator.Send(new CreateUserCommand
            {
                Uuid = uuid,

                Username = username,

                Email = email
            });

            if (result.IsFailed && result.Errors.Exists(e => e.HasMetadata("errCode", "errUserAlreadyExists")))
                return BadRequest("The user already exists");

            if (result.IsFailed || result.Errors.Exists(e => e.HasMetadata("errCode", "errDbSaveFail")))
                return Problem("An error occured while trying to save user into the database");

            if (result.IsFailed)
                return BadRequest(result.Errors);

            return Created($"/api/v1/user/{uuid}", "");
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
        [HttpDelete]
        public async Task Delete([FromBody] DeleteUserCommand deleteUserCommand)
        {
            await _mediator.Send(deleteUserCommand);
        }
    }
}