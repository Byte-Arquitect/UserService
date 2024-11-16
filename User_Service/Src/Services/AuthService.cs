using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using User_Service.Src.Exceptions;
using User_Service.Src.Protos;
using User_Service.Src.Repositories.Interfaces;
using User_Service.Src.Services.Interfaces;
using User_Service.Src.Models;
using User_Service.Src.DTOs.Auth;
using User_Service.Src.Dtos;

namespace User_Service.Src.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapperService _mapperService;
        

        public AuthService(ILogger<AuthService> logger, IUnitOfWork unitOfWork, IMapperService mapperService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperService = mapperService;
        }

        public async Task<ResponseRegister> Register(RegisterUserDto request, ServerCallContext context)
        {
            var response1 = request.ToString();
            Console.WriteLine($"Received request for Register: {response1}");

            await ValidateEmailAndRUT(request.Email, request.Rut);

            // var role = await "ApiGateway/AuthService/GetRoleByName"
            int roleId = 1;
            if (roleId is 3)
                throw new InternalErrorException("Role not found");
            // var career = await "ApiGateway/AuthService/GetCareerByName"
            int careerId = request.CareerId;
            if (roleId is 3)
                throw new EntityNotFoundException($"Career with ID: {request.CareerId} not found");
            
            var userOnRequest = new RegisterStudentDto
            {
                Name = request.Name,
                FirstLastName = request.Firstlastname,
                SecondLastName = request.Secondlastname,
                RUT = request.Rut,
                Email = request.Email,
                CareerName = "ICCI",
                Password = request.Password,
                RepeatedPassword = request.RepeatedPassword
            };

            var mappedUser = _mapperService.Map<RegisterStudentDto, User>(userOnRequest);

            mappedUser.RoleId = roleId;
            mappedUser.CareerId = careerId;
            mappedUser.IsEnabled = true;
            mappedUser.RoleName = "Student";
            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            mappedUser.HashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, salt);

            var createdUser = await _unitOfWork.UsersRepository.Insert(mappedUser);

            // var responseLogin = "LLamada a la apiGateway del login con la contraseña "

            var loginUser = _mapperService.Map<User, RegisterResponseDto>(createdUser);
            string token = "token";
            var UserResponse = _mapperService.Map<RegisterResponseDto, UserRegisterResponse>(loginUser);
            var response = new ResponseRegister
            {
                User = UserResponse,
                Token = token
            };

            return response;
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