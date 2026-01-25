using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Requests.Identity
{
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }
        public string Provider { get; set; } = "GOOGLE";
    }
}
