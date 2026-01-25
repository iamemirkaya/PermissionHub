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
    public class ResetPasswordCommand : IRequest<IResponseWrapper<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public string ConfirmPassword { get; set; }


    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, IResponseWrapper<string>>
    {
        private readonly IUserService _userService;

        public ResetPasswordCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {

            var requestDto = new ResetPasswordRequest
            {
                Email = request.Email,
                Password = request.Password,
                Token = request.Token,
                ConfirmPassword = request.ConfirmPassword
            };

            return await _userService.ResetPassword(requestDto);
        }
    }
}
