using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesteSignalR.Models
{
    public class LoginResponse : Response
    {
        public string Token { get; set; }
        public string Expires { get; set; }
    }
}
