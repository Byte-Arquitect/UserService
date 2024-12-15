using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using User_Service.Src.Protos;

namespace User_Service.Src.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<ResponseRegister> Register(RegisterUserDto request, ServerCallContext context);
        public Task<ResponsePassword> UpdatePassword(
            newPassword request,
            ServerCallContext context
        );
    }
}
