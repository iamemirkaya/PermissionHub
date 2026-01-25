using Application.DTOs.Email;
using Application.Features.Identity.Users.Commands;
using Application.Services.Identity;
using Application.Services.MailService;
using AutoMapper;
using Azure;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper, ICurrentUserService currentUserService, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _emailService = emailService;
        }

        public async Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request)
        {
            var userInDb = await _userManager.FindByIdAsync(request.UserId);
            if (userInDb is not null)
            {
                userInDb.IsActive = request.Activate;
                var identityResult = await _userManager.UpdateAsync(userInDb);

                if (identityResult.Succeeded)
                {
                    return await ResponseWrapper<string>
                        .SuccessAsync(request.Activate ? "User actived successfully."
                            : "User de-activated successfully");
                }
                return await ResponseWrapper.FailAsync("User actived failed");
            }
            return await ResponseWrapper.FailAsync("User does not exist.");
        }

        public async Task<IResponseWrapper<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return await ResponseWrapper<string>.FailAsync("User not found.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return await ResponseWrapper<string>.SuccessAsync("Email confirmed successfully.");
            }
            else
            {
                return await ResponseWrapper<string>.FailAsync($"An error occured while confirming {user.Email}.");
            }
        }

        public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {

            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null) return;

            var code = await _userManager.GeneratePasswordResetTokenAsync(account);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var route = "reset-password";
            var uriString = $"{origin}/{route}";
            var endpointUri = new Uri(uriString);

            var passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "token", code);
            passwordResetUrl = QueryHelpers.AddQueryString(passwordResetUrl, "email", model.Email);

            var emailRequest = new EmailRequest()
            {
                Body = $"Please <a href='{passwordResetUrl}'>click here</a> to reset your password.",
                To = model.Email,
                Subject = "Reset Password",
            };

            await _emailService.SendAsync(emailRequest);
        }

        public async Task<IResponseWrapper> GetAllUsersAsync()
        {
            var usersInDb = await _userManager
                .Users
                .ToListAsync();

            if (usersInDb.Count > 0)
            {
                var mappedUsers = _mapper.Map<List<UserResponse>>(usersInDb);
                return await ResponseWrapper<List<UserResponse>>.SuccessAsync(mappedUsers);
            }
            return await ResponseWrapper.FailAsync("No Users were found.");
        }

        public async Task<IResponseWrapper> GetRolesAsync(string userId)
        {
            var userRolesVM = new List<UserRoleViewModel>();
            var userIdDb = await _userManager.FindByIdAsync(userId);
            if (userIdDb is not null)
            {
                var allRoles = await _roleManager.Roles.ToListAsync();
                foreach (var role in allRoles)
                {
                    var userRoleVM = new UserRoleViewModel
                    {
                        RoleName = role.Name,
                        RoleDescription = role.Description
                    };

                    if (await _userManager.IsInRoleAsync(userIdDb, role.Name))
                    {
                        userRoleVM.IsAssignedToUser = true;
                    }
                    else
                    {
                        userRoleVM.IsAssignedToUser = false;
                    }

                    userRolesVM.Add(userRoleVM);
                }

                return await ResponseWrapper<List<UserRoleViewModel>>.SuccessAsync(userRolesVM);
            }
            return await ResponseWrapper.FailAsync("User does not exist.");
        }

        public async Task<IResponseWrapper<UserResponse>> GetUserByEmailAsync(string email)
        {
            var userInDb = await _userManager.FindByEmailAsync(email);
            if (userInDb is not null)
            {
                var mappedUser = _mapper.Map<UserResponse>(userInDb);
                return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);
            }
            return await ResponseWrapper<UserResponse>.FailAsync("User does not exist");
        }

        public async Task<IResponseWrapper> GetUserByIdAsync(string userId)
        {
            var userInDb = await _userManager.FindByIdAsync(userId);
            if (userInDb is not null)
            {
                var mappedUser = _mapper.Map<UserResponse>(userInDb);
                return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);
            }
            return await ResponseWrapper.FailAsync("User does not exist."); throw new NotImplementedException();
        }

        public async Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request)
        {
            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);

            if (userWithSameEmail is not null)
            {
                return await ResponseWrapper.FailAsync("Email already taken.");
            }

            var userWithSameUsername = await _userManager.FindByNameAsync(request.UserName);

            if (userWithSameUsername is not null)
            {
                return await ResponseWrapper.FailAsync("Username already taken.");
            }

            var newUser = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                IsActive = request.ActivateUser,
                EmailConfirmed = request.AutoComfirmEmail,
            };

            var password = new PasswordHasher<ApplicationUser>();
            newUser.PasswordHash = password.HashPassword(newUser, request.Password);

            var identityResult = await _userManager.CreateAsync(newUser);

            if (identityResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);

                if (!request.AutoComfirmEmail)
                {
                    var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    verificationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(verificationToken));

                    var verificationUrl = $"https://localhost:3000/confirm-email?userId={newUser.Id}&code={verificationToken}";

                    var emailRequest = new EmailRequest
                    {
                        To = newUser.Email,
                        Subject = "PermissionHub Hesabınızı Onaylayın",
                        Body = $"<h3>Hoşgeldiniz {newUser.FirstName}!</h3><p>Hesabınızı onaylamak için lütfen <a href='{verificationUrl}'>buraya tıklayınız</a>.</p>"
                    };

                    await _emailService.SendAsync(emailRequest);

                    var mailResponse = await _emailService.SendAsync(emailRequest);

                    if (!mailResponse.IsSuccessful)
                    {
                        return await ResponseWrapper.FailAsync($"Kullanıcı oluştu ama mail gönderilemedi. Hata: {string.Join(',', mailResponse.Messages)}");
                    }

                    return await ResponseWrapper<string>.SuccessAsync($"User registered successfully. Please check your email ({newUser.Email}) to confirm your account.");
                }

                return await ResponseWrapper<string>.SuccessAsync("User registered successfully.");
            }
            return await ResponseWrapper.FailAsync("User registered failed.");
        }

        public async Task<IResponseWrapper<string>> ResetPassword(ResetPasswordRequest model)
        {

            var account = await _userManager.FindByEmailAsync(model.Email);

            if (account == null)
            {
                return await ResponseWrapper<string>.FailAsync($"No Accounts Registered with {model.Email}.");
            }

            try
            {
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

                var result = await _userManager.ResetPasswordAsync(account, decodedToken, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(account);

                    return await ResponseWrapper<string>.SuccessAsync(model.Email, "Password Reset Successful.");
                }
                else
                {
                    var errorMessages = result.Errors.Select(e => e.Description).ToList();
                    var fullErrorMessage = string.Join(" ", errorMessages);

                    return await ResponseWrapper<string>.FailAsync(fullErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return await ResponseWrapper<string>.FailAsync($"An error occurred while resetting password: {ex.Message}");
            }
        }

        public async Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request)
        {
            var userInDb = await _userManager.FindByIdAsync(request.UserId);
            if (userInDb is not null)
            {
                userInDb.FirstName = request.FirstName;
                userInDb.LastName = request.LastName;
                userInDb.PhoneNumber = request.PhoneNumber;

                var identityResult = await _userManager.UpdateAsync(userInDb);
                if (identityResult.Succeeded)
                {
                    return await ResponseWrapper<string>.SuccessAsync("User details successfully updated.");
                }
                return await ResponseWrapper.FailAsync("User details Fail updated.");
            }
            return await ResponseWrapper.FailAsync("User does not exist.");
        }

        public async Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRolesRequest request)
        {
            var userInDb = await _userManager.FindByIdAsync(request.UserId);
            if (userInDb is not null)
            {
                if (userInDb.Email == AppCredentials.Email)
                {
                    return await ResponseWrapper.FailAsync("User Roles update not permitted.");
                }
                var currentAssignedRoles = await _userManager.GetRolesAsync(userInDb);
                var rolesToBeAssigned = request.Roles
                    .Where(role => role.IsAssignedToUser == true)
                    .ToList();

                var currentLoggedInUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
                if (currentLoggedInUser is null)
                {
                    return await ResponseWrapper.FailAsync("User does not exist.");
                }

                if (await _userManager.IsInRoleAsync(currentLoggedInUser, AppRoles.Admin))
                {
                    var identityResult1 = await _userManager.RemoveFromRolesAsync(userInDb, currentAssignedRoles);
                    if (identityResult1.Succeeded)
                    {
                        var identityResult2 = await _userManager
                            .AddToRolesAsync(userInDb, rolesToBeAssigned.Select(role => role.RoleName));
                        if (identityResult2.Succeeded)
                        {
                            return await ResponseWrapper<string>.SuccessAsync("User Roles Updated Successfully.");
                        }
                        return await ResponseWrapper.FailAsync("An error occurred while adding new roles.");
                    }
                    return await ResponseWrapper.FailAsync("An error occurred while removing existing roles.");
                }
                return await ResponseWrapper.FailAsync("User Roles update not permitted.");
            }
            return await ResponseWrapper.FailAsync("User does not exist.");
        }
    }


}
