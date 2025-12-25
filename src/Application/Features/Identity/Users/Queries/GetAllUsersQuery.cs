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
    public class GetAllUsersQuery : IRequest<IResponseWrapper>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IResponseWrapper>
    {
        private readonly IUserService _userService;

        public GetAllUsersQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetAllUsersAsync();
        }
    }
}
