using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Commands
{
    public class ChangeUserStatusCommand : IRequest<IResponseWrapper>
    {
        public ChangeUserStatusRequest ChangeUserStatus { get; set; }
    }

    public class ChangeUserStatusCommandHandler : IRequestHandler<ChangeUserStatusCommand, IResponseWrapper>
    {
        private readonly IUserService _userService;

        public ChangeUserStatusCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
        {
            return await _userService.ChangeUserStatusAsync(request.ChangeUserStatus);
        }
    }
}
