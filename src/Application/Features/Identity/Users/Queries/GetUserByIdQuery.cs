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
    public class GetUserByIdQuery : IRequest<IResponseWrapper>
    {
        public string UserId { get; set; }
    }

    public class GetUserByIdQueryHanlder : IRequestHandler<GetUserByIdQuery, IResponseWrapper>
    {
        private readonly IUserService _userService;

        public GetUserByIdQueryHanlder(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetUserByIdAsync(request.UserId);
        }
    }
}
