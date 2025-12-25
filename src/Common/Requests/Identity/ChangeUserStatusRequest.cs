using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Requests.Identity
{
    public class ChangeUserStatusRequest
    {
        public string UserId { get; set; }
        public bool Activate { get; set; }
    }
}
