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
    public class RevokeTokenCommandRequest : IRequest<IResponseWrapper>
    {
    }

    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommandRequest, IResponseWrapper>
    {
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;

        public RevokeTokenCommandHandler(ITokenService tokenService, ICurrentUserService currentUserService)
        {
            _tokenService = tokenService;
            _currentUserService = currentUserService;
        }

        public async Task<IResponseWrapper> Handle(RevokeTokenCommandRequest request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                return await ResponseWrapper.FailAsync("Oturum açmış kullanıcı bulunamadı.");
            }

            return await _tokenService.RevokeRefreshTokenAsync(userId);
        }
    }
}
