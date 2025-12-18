using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AppConfigs
{
    public class AppConfiguration
    {
        public string Secret { get; set; }
        public int TokenExpiryInMinutes { get; set; }
    }
}
