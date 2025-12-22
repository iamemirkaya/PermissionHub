using Application.Services.Employees;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Commands
{
    public class DeleteEmployeeCommand : IRequest<IResponseWrapper>
    {
        public Guid EmployeeId { get; set; }
    }

    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;

        public DeleteEmployeeCommandHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IResponseWrapper> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.EmployeeId);
            if (employeeInDb is not null)
            {
                var employeeId = await _employeeService.DeleteEmployeeAsync(employeeInDb);
                return await ResponseWrapper<Guid>.SuccessAsync(employeeId, "Employee entry deleted successfully.");
            }
            else
            {
                return await ResponseWrapper.FailAsync("Employee does not exist.");
            }
        }
    }
}
