using Common.Requests.Employees;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Validators
{
    public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
    {
        public CreateEmployeeRequestValidator()
        {
            RuleFor(request => request.FirstName)
                .NotEmpty().WithMessage("Employee firstname is required.")
                .MaximumLength(60);
            RuleFor(request => request.LastName)
                .NotEmpty().WithMessage("Employee lastname is required.")
                .MaximumLength(60);
            RuleFor(request => request.Email)
                .NotEmpty().WithMessage("Employee email is required.")
                .MaximumLength(100);
            RuleFor(request => request.Salary)
                .NotEmpty().WithMessage("Employee must have a salary.");
        }
    }
}
