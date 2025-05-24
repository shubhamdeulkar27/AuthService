using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }

}
