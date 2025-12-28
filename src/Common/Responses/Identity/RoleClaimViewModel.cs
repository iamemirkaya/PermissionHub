using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Responses.Identity
{
    public class RoleClaimViewModel
    {
        public Guid RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public bool IsAssignedToRole { get; set; }
    }
}
