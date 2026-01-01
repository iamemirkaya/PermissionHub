using Application.Features.Employees.Commands;
using Application.Services.Employees;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Validators
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator(IEmployeeService employeeService)
        {
            RuleFor(command => command.UpdateEmployeeRequest)
                .SetValidator(new UpdateEmployeeRequestValidator(employeeService));
        }
    }
}
