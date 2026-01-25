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
    public class ForgotPasswordCommand : IRequest<IResponseWrapper<string>>
    {
        public string Email { get; set; }
        public string Origin { get; set; } 
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, IResponseWrapper<string>>
    {
        private readonly IUserService _userService;

        public ForgotPasswordCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var forgotPasswordRequest = new ForgotPasswordRequest
            {
                Email = request.Email
            };

            await _userService.ForgotPassword(forgotPasswordRequest, request.Origin);

            return await ResponseWrapper<string>.SuccessAsync("Password Reset Mail Sent.");
        }
    }
}
