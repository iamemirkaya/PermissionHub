using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Identity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Validators
{
    public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
    {
        public UserRegistrationRequestValidator(IUserService userService)
        {
            RuleFor(request => request.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256)
                .MustAsync(async (email, ct) => await userService.GetUserByEmailAsync(email)
                    is not UserResponse existingUser)
                .WithMessage("Email is taken.");

            RuleFor(request => request.FirstName)
                .NotEmpty()
                .MaximumLength(60);
            RuleFor(request => request.LastName)
                .NotEmpty()
                .MaximumLength(60);
            RuleFor(request => request.Email)
                .NotEmpty()
                .MaximumLength(256);
            RuleFor(request => request.UserName)
                .NotEmpty()
                .MaximumLength(256);
            RuleFor(request => request.Password)
                .NotEmpty();
            RuleFor(request => request.ComfirmPassword)
                .Must((req, confirmed) => req.Password == confirmed)
                .WithMessage("Passwords do not match.");
        }
    }
}
