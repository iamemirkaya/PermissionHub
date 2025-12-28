using Common.Requests.Identity;
using Common.Responses.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Identity
{
    public interface IRoleService
    {
        Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest request);

        Task<IResponseWrapper> GetRolesAsync();

        Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest request);

        Task<IResponseWrapper> GetRoleByIdAsync(Guid roleId);

        Task<IResponseWrapper> DeleteRoleAsync(Guid roleId);

        Task<IResponseWrapper> GetPermissionsAsync(Guid roleId);

        Task<IResponseWrapper> UpdateRolePermissionsAsync(UpdateRolePermissionsRequest request);

    }
}
