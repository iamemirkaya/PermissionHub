using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public string Description { get; set; }
        public string Group { get; set; }
    }
}
