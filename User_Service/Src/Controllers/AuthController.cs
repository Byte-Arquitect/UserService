using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using User_Service.Src.Models;
using User_Service.Src.Protos;
using User_Service.Src.Services;

namespace User_Service.Src.Services
{
    public class AuthController : UserProto.UserProtoBase
    {
        private readonly ILogger<AuthService> _logger;
        private readonly AuthService _authService;

        public AuthController(ILogger<AuthService> logger, AuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        
        public override Task<ResponseRegister> Register(RegisterUserDto request, ServerCallContext context){
            
            // var response = _authService.Register(request,context);
            // return Task.FromResult(new ResponseRegister
            // {
            //     Message = "Register OK, usuario" + userRegister.ToString()
            // });

            return null;
        }

    }
}