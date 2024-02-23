using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHubApi.Controllers.AdminServices.Login.Model
{
    public class LoginInput
    {
        public string account { get; set; }
        public string psw { get; set; }
    }
}
