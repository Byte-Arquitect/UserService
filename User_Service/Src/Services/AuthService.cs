using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using User_Service.Src.Exceptions;
using User_Service.Src.Protos;
using User_Service.Src.Repositories.Interfaces;
using User_Service.Src.Services.Interfaces;

namespace User_Service.Src.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
         private readonly IUnitOfWork _unitOfWork;
        

        public AuthService(ILogger<AuthService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseRegister> Register(RegisterUserDto request, ServerCallContext context)
        {
            await ValidateEmailAndRUT(request.Email, request.Rut);
            return new ResponseRegister();
        }

        private async Task ValidateEmailAndRUT(string email, string rut)
        {
            var user = await _unitOfWork.UsersRepository.GetByEmail(email);
            if (user is not null)
                throw new DuplicateUserException("Email already in use");

            user = await _unitOfWork.UsersRepository.GetByRut(rut);
            if (user is not null)
                throw new DuplicateUserException("RUT already in use");
        }
        
    }
}