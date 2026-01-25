using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Commands
{
    public class ConfirmEmailCommand : IRequest<IResponseWrapper<string>>
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, IResponseWrapper<string>>
    {
        private readonly IUserService _userService;

        public ConfirmEmailCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            return await _userService.ConfirmEmailAsync(request.UserId, request.Code);
        }
    }
}
