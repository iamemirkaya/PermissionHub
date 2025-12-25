using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Responses.Identity
{
    public class UserRoleViewModel
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsAssignedToUser { get; set; }
    }
}
