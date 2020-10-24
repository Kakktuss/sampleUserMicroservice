using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentResults;
using UserApplication.Dtos.Request;
using UserApplication.Models;
using UserApplication.ViewModels;

namespace UserApplication.Services
{
    public interface IUserService
    {
        Task<Result<IEnumerable<UserViewModel>>> RetrieveAsync();

        Task<Result<UserViewModel>> RetrieveByUuidAsync(string uuid);
        
        Task<Result> CreateAsync(CreateUserDto createUserDto);

        Task<Result> CompleteProfileAsync(CompleteUserProfileDto completeUserProfile);

        Task<Result> DeleteAsync(DeleteUserDto deleteUserDto);
    }
}