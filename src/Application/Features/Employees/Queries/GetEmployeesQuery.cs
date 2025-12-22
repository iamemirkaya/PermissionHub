using Application.Services.Employees;
using AutoMapper;
using Common.Responses.Employees;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Queries
{
    public class GetEmployeesQuery : IRequest<IResponseWrapper>
    {
    }

    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public GetEmployeesQueryHandler(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employeeList = await _employeeService.GetEmployeeListAsync();
            if (employeeList.Count > 0)
            {
                var mappedEmployeeList = _mapper.Map<List<EmployeeResponse>>(employeeList);
                return await ResponseWrapper<List<EmployeeResponse>>.SuccessAsync(mappedEmployeeList);
            }
            return await ResponseWrapper.FailAsync("No employees were found.");
        }
    }
}
