using AutoMapper;
using Common.Requests.Employees;
using Common.Responses.Employees;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateEmployeeRequest, Employee>();
            CreateMap<Employee, EmployeeResponse>();
        }
    }
}
