using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using BuildingBlock.Bus.Abstractions.Nats.Events;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserApplication.Dtos.Request;
using UserApplication.EntityFrameworkDataAccess.Repositories;
using UserApplication.IntegrationEvents.Events.User;
using UserApplication.Models;
using UserApplication.ViewModels;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace UserApplication.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;

        private readonly DapperDataAccess.Repositories.IUserRepository _dapperUserRepository;
        
        private readonly ICountryRepository _countryRepository;

        private readonly IStanIntegrationEventBus _stanIntegrationEventBus;

        private readonly ILogger<UserService> _logger;
        
        public UserService(IUserRepository userRepository,
            DapperDataAccess.Repositories.IUserRepository dapperUserRepository,
            ICountryRepository countryRepository,
            IStanIntegrationEventBus stanIntegrationEventBus,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;

            _dapperUserRepository = dapperUserRepository;
            
            _countryRepository = countryRepository;

            _stanIntegrationEventBus = stanIntegrationEventBus;

            _logger = logger;
        }

        public async Task<Result<IEnumerable<UserViewModel>>> RetrieveAsync()
        {
            _logger.LogTrace("[UserService:RetrieveAsync] Starting processing the command");

            var users = await _dapperUserRepository.GetUsersAsync();

            if (users == null || !users.Any())
            {
                return Results.Fail(new Error("There is no user found")
                    .WithMetadata("errCode", "errUsersNotFound"));
            }

            _logger.LogTrace("[UserService:RetrieveAsync] Command processed successfully");

            return Results.Ok(users);
        }

        public async Task<Result<UserViewModel>> RetrieveByUuidAsync(string uuid)
        {
            _logger.LogTrace("[UserService:RetrieveAsync] Starting processing the command");

            var user = await _dapperUserRepository.FindUserByUuidAsync(uuid);

            if (user == null)
            {
                return Results.Fail(new Error($"There is no user with uuid {uuid} found")
                    .WithMetadata("errCode", "errUserNotFound"));
            }

            _logger.LogTrace("[UserService:RetrieveAsync] Command processed successfully");

            return Results.Ok(user);
        }

        public async Task<Result> CreateAsync(CreateUserDto createUserDto)
        {
            _logger.LogTrace("[UserService:CreateAsync] Starting processing the command");

            if (await _userRepository.ExistsByUuidAsync(createUserDto.Uuid) || 
                await _userRepository.ExistsByUuidAsync(createUserDto.Uuid) || 
                await _userRepository.ExistsByUuidAsync(createUserDto.Uuid))
            {
                return Results.Fail(new Error($"The user with uuid {createUserDto.Uuid} already exists")
                    .WithMetadata("errCode", "errUserAlreadyExists"));
            }

            var user = new User(createUserDto.Uuid, createUserDto.Username, createUserDto.Email);
            
            _userRepository.Add(user);

            try
            {
                var result = await _userRepository.UnitOfWork.SaveEntitiesAsync();

                if (!result)
                {
                    return Results.Fail(new Error("An error happened while trying to create the user")
                        .WithMetadata("errCode", "errDbSaveFail"));
                }
            }
            catch (DbUpdateException e)
            {
                return Results.Fail(new Error("An error happened while trying to create the user")
                    .WithMetadata("errCode", "errDbSaveFail"));
            }

            var integrationEventSubject = "user";

            _stanIntegrationEventBus.Publish(integrationEventSubject, "created",
                new UserCreatedIntegrationEvent(createUserDto.Uuid, createUserDto.Username, createUserDto.Email));
            
            _logger.LogTrace("[UserService:CreateAsync] Command executed successfully.");

            return Results.Ok();
        }

        public async Task<Result> CompleteProfileAsync(CompleteUserProfileDto completeUserProfile)
        {

            var user = await _userRepository.FindByUuidAsync(completeUserProfile.UserUuid);

            if (user == null)
            {
                return Results.Fail(new Error($"The user with uuid {completeUserProfile.UserUuid} does not exists")
                    .WithMetadata("errCode", "errUserNotFound"));
            }

            var country = await _countryRepository.FindByUuidAsync(completeUserProfile.CountryUuid);

            if (country == null)
            {
                return Results.Fail(new Error($"The user with uuid {completeUserProfile.UserUuid} is not found")
                    .WithMetadata("errCode", "errCountryNotFound"));
            }
            
            user.SetCountry(country);
            
            user.MarkAsComplete();

            try
            {
                var result = await _userRepository.UnitOfWork.SaveEntitiesAsync();

                if (!result)
                {
                    return Results.Fail(new Error("An error happened while trying to delete the user")
                        .WithMetadata("errCode", "errDbSaveFail"));
                }
            }
            catch (DbUpdateException e)
            {
                return Results.Fail(new Error("An error happened while trying to delete the user")
                    .WithMetadata("errCode", "errDbSaveFail"));
            }

            var integrationEventSubject = "user";

            _stanIntegrationEventBus.Publish(integrationEventSubject, "completed",
                new UserCompletedIntegrationEvent(completeUserProfile.UserUuid, completeUserProfile.CountryUuid));
            
            _logger.LogTrace("[UserService:DeleteAsync] Command executed successfully.");

            return Results.Ok();

        }

        public async Task<Result> DeleteAsync(DeleteUserDto deleteUserDto)
        {
            
            _logger.LogTrace("[UserService:DeleteAsync] Starting processing the command");

            var user = await _userRepository.FindByUuidAsync(deleteUserDto.Uuid);

            if (user == null)
            {
                return Results.Fail(new Error($"The user with uuid {deleteUserDto.Uuid} does not exists")
                    .WithMetadata("errCode", "errUserNotFound"));
            }
            
            _userRepository.Remove(user);
            
            try
            {
                var result = await _userRepository.UnitOfWork.SaveEntitiesAsync();

                if (!result)
                {
                    return Results.Fail(new Error("An error happened while trying to delete the user")
                        .WithMetadata("errCode", "errDbSaveFail"));
                }
            }
            catch (DbUpdateException e)
            {
                return Results.Fail(new Error("An error happened while trying to delete the user")
                    .WithMetadata("errCode", "errDbSaveFail"));
            }

            var integrationEventSubject = "user";

             _stanIntegrationEventBus.Publish(integrationEventSubject, "deleted",
                 new UserDeletedIntegrationEvent(deleteUserDto.Uuid));
            
            _logger.LogTrace("[UserService:DeleteAsync] Command executed successfully.");

            return Results.Ok();
        }
        
    }
}