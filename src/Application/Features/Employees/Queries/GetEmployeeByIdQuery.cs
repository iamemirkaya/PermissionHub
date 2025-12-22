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
    public class GetEmployeeByIdQuery : IRequest<IResponseWrapper>
    {
        public Guid EmployeeId { get; set; }
    }

    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public GetEmployeeByIdQueryHandler(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.EmployeeId);
            if (employeeInDb is not null)
            {
                var mappedEmployee = _mapper.Map<EmployeeResponse>(employeeInDb);
                return await ResponseWrapper<EmployeeResponse>.SuccessAsync(mappedEmployee);
            }
            return await ResponseWrapper.FailAsync("Employee does not exist.");
        }
    }
}
