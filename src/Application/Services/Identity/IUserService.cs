using Common.Responses.Wrappers;
using Common.Requests.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Responses.Identity;
using Application.Features.Identity.Users.Commands;

namespace Application.Services.Identity
{
    public interface IUserService
    {
        Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request);

        Task<IResponseWrapper> GetUserByIdAsync(string userId);

        Task<IResponseWrapper> GetAllUsersAsync();

        Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request);

        Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request);

        Task<IResponseWrapper> GetRolesAsync(string userId);

        Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRolesRequest request);

        Task<IResponseWrapper<UserResponse>> GetUserByEmailAsync(string email);

       Task<IResponseWrapper<string>> ConfirmEmailAsync(string userId, string code);

        Task ForgotPassword(ForgotPasswordRequest model, string origin);
        Task<IResponseWrapper<string>> ResetPassword(ResetPasswordRequest model);
    }
}
