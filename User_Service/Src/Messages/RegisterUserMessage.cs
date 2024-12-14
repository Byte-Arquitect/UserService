using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User_Service.Src.Dtos;


namespace Shared.Messages
{
    public class RegisterUserMessage
    {
        public string Email {get;set;} = null!;
        public string Password {get;set;} = null!;
        public string UserUuid {get;set;} = null!;
    }
}