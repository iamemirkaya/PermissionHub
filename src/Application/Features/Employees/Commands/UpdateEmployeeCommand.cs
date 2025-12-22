using Application.Services.Employees;
using AutoMapper;
using Common.Requests.Employees;
using Common.Responses.Employees;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Commands
{
    public class UpdateEmployeeCommand : IRequest<IResponseWrapper>
    {
        public UpdateEmployeeRequest UpdateEmployeeRequest { get; set; }
    }

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public UpdateEmployeeCommandHandler(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.UpdateEmployeeRequest.Id);

            if (employeeInDb is not null)
            {
                employeeInDb.FirstName = request.UpdateEmployeeRequest.FirstName;
                employeeInDb.LastName = request.UpdateEmployeeRequest.LastName;
                employeeInDb.Salary = request.UpdateEmployeeRequest.Salary;
                employeeInDb.Email = request.UpdateEmployeeRequest.Email;

                var updatedEmployee = await _employeeService.UpdateEmployeeAsync(employeeInDb);

                var mappedEmployee = _mapper.Map<EmployeeResponse>(updatedEmployee);
                return await ResponseWrapper<EmployeeResponse>.SuccessAsync(mappedEmployee,
                    "Employee updated successfully.");
            }
            return await ResponseWrapper<EmployeeResponse>.FailAsync("Employee does not exist.");
        }
    }
}
