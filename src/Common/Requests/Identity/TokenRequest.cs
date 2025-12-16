using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Requests.Identity
{
    public class TokenRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
