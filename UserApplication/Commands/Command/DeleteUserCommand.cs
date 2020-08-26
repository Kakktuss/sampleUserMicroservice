using MediatR;

namespace UserApplication.Commands.Command
{
    public class DeleteUserCommand : IRequest
    {
        public string Uuid { get; }
    }
}