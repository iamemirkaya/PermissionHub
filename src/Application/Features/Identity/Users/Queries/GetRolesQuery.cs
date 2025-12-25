using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Queries
{
    public class GetRolesQuery : IRequest<IResponseWrapper>
    {
        public string UserId { get; set; }
    }

    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IResponseWrapper>
    {
        private readonly IUserService _userService;

        public GetRolesQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetRolesAsync(request.UserId);
        }
    }
}
