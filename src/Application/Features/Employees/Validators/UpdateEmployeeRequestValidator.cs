using Application.Services.Employees;
using Common.Requests.Employees;
using Domain;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Validators
{
    public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
    {
        public UpdateEmployeeRequestValidator(IEmployeeService employeeService)
        {
            RuleFor(request => request.Id)
                .MustAsync(async (id, ct) => await employeeService.GetEmployeeByIdAsync(id)
                    is Employee employeeInDb && employeeInDb.Id == id)
                .WithMessage("Employee does not exist.");

            RuleFor(request => request.FirstName)
                .NotEmpty().WithMessage("Employee firstname is required.")
                .MaximumLength(60);
            RuleFor(request => request.LastName)
                .NotEmpty().WithMessage("Employee lastname is required.")
                .MaximumLength(60);
            RuleFor(request => request.Email)
                .EmailAddress()
                .NotEmpty().WithMessage("Employee email is required.")
                .MaximumLength(100);
            RuleFor(request => request.Salary)
                .NotEmpty().WithMessage("Employee must have a salary.");
        }
    }
}
