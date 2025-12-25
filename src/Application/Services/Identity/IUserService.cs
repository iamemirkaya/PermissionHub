using Common.Responses.Wrappers;
using Common.Requests.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Identity
{
    public interface IUserService
    {
        Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request);

        Task<IResponseWrapper> GetUserByIdAsync(string userId);

        Task<IResponseWrapper> GetAllUsersAsync();

        Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request);

        Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request);

        Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request);

        Task<IResponseWrapper> GetRolesAsync(string userId);
    }
}
