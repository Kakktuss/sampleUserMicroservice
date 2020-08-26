using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using UserApplication.Commands.Command;

namespace UserApplication.Commands.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        public Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}