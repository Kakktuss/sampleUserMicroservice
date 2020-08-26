using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserApplication.Commands.Command;
using UserDomain.AggregateModels.Users;
using IUserRepository = UserApplication.EntityFrameworkDataAccess.Repositories.IUserRepository;

namespace UserApplication.Commands.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
    {
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository,
            ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository;

            _logger = logger;
        }

        public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("[CreateUserCommand] Starting processing the command.");

            if (await _userRepository.ExistsByUuidAsync(request.Uuid))
            {
                _logger.LogInformation(
                    $"[CreateUserCommand] End: The user {request.Uuid} already exists in the database.");

                return Results.Fail(new Error("The user already exists")
                    .WithMetadata("errCode", "errUserAlreadyExists"));
            }

            _userRepository.Add(new User(request.Uuid, request.Username, request.Email));

            try
            {
                var result = await _userRepository.UnitOfWork.SaveEntitiesAsync();

                if (!result)
                {
                    _logger.LogInformation("[CreateUserCommand] End: An error happened while trying to save the user");

                    return Results.Fail(new Error("An error happened while trying to save the user")
                        .WithMetadata("errCode", "errDbSaveFail"));
                }
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(
                    "[CreateUserCommand] Error: An error happened while trying to save the user in the database", e);

                return Results.Fail(new Error("An error happened while trying to save the user")
                    .WithMetadata("errCode", "errDbSaveFail"));
            }

            _logger.LogTrace("[CreateUserCommand] Command processed successfully.");

            return Results.Ok();
        }
    }
}