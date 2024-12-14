using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using User_Service.Src.Models;
using User_Service.Src.Protos;
using User_Service.Src.Services;
using User_Service.Src.Services.Interfaces;

namespace User_Service.Src.Services
{
    public class AuthController : AuthProto.AuthProtoBase
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthService> logger, AuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        public override async Task<ResponseRegister> Register(
            RegisterUserDto request,
            ServerCallContext context
        )
        {
            var response = await _authService.Register(request, context);
            return new ResponseRegister { User = response.User, Token = response.Token };
        }

        public override async Task<ResponsePassword> UpdatePassword(
            newPassword request,
            ServerCallContext context
        )
        {
            var response = await _authService.UpdatePassword(request, context);
            return new ResponsePassword { Response = response.Response };
        }
    }
}
