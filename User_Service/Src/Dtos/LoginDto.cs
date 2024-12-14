using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User_Service.Src.Dtos
{
    public class LoginDto
    {
        public string Email {get;set;} = null!;
        public string Password {get;set;} = null!;
        public string UserUuid {get;set;} = null!;
    }
}