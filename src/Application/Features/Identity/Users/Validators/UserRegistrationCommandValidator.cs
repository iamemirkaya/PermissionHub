using Application.Features.Identity.Users.Commands;
using Application.Services.Identity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Validators
{
    public class UserRegistrationCommandValidator : AbstractValidator<UserRegistrationCommand>
    {
        public UserRegistrationCommandValidator(IUserService userService)
        {
            RuleFor(command => command.UserRegistration)
                .SetValidator(new UserRegistrationRequestValidator(userService));
        }
    }
}
