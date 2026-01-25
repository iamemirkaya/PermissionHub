using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Commands
{
    public class GoogleLoginCommandRequest : IRequest<GoogleLoginCommandResponse>
    {
        public string? Id { get; set; }
        public string IdToken { get; set; }
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhotoUrl { get; set; }
        public string Provider { get; set; }
    }

    public class GoogleLoginCommandResponse
    {
        public TokenResponse Token { get; set; }
    }

    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandRequest, GoogleLoginCommandResponse>
    {
        private readonly ITokenService _tokenService;

        public GoogleLoginCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request, CancellationToken cancellationToken)
        {
            var googleLoginRequest = new GoogleLoginRequest
            {
                IdToken = request.IdToken,
                Provider = "GOOGLE"
            };

            var responseWrapper = await _tokenService.GoogleLoginAsync(googleLoginRequest);

            if (responseWrapper.IsSuccessful)
            {
                return new GoogleLoginCommandResponse
                {
                    Token = responseWrapper.ResponseData
                };
            }
            var errorMessage = responseWrapper.Messages?.FirstOrDefault() ?? "Google girişi başarısız oldu.";
            throw new Exception(errorMessage);
        }
    }
}
