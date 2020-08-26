using FluentResults;
using MediatR;

namespace UserApplication.Commands.Command
{
    public class CreateUserCommand : IRequest<Result>
    {
        public string Uuid { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
    }
}