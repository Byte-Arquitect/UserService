using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using User_Service.Src.Dtos;
using User_Service.Src.DTOs.Auth;
using User_Service.Src.Exceptions;
using User_Service.Src.Models;
using User_Service.Src.Producers;
using User_Service.Src.Protos;
using User_Service.Src.Repositories.Interfaces;
using User_Service.Src.Services.Interfaces;

namespace User_Service.Src.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapperService _mapperService;

        private readonly RegisterEvent _registerEvent;
        private readonly UpdatePasswordEvent _updateEvent;

        public AuthService(
            ILogger<AuthService> logger,
            IUnitOfWork unitOfWork,
            IMapperService mapperService,
            RegisterEvent registerEvent,
            UpdatePasswordEvent updateEvent
        )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperService = mapperService;
            _registerEvent = registerEvent;
            _updateEvent = updateEvent;
        }

        public async Task<ResponseRegister> Register(
            RegisterUserDto request,
            ServerCallContext context
        )
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
                RepeatedPassword = request.RepeatedPassword,
            };

            var mappedUser = _mapperService.Map<RegisterStudentDto, User>(userOnRequest);

            mappedUser.RoleId = roleId;
            mappedUser.CareerId = careerId;
            mappedUser.IsEnabled = true;
            mappedUser.RoleName = "Student";
            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            var pass = BCrypt.Net.BCrypt.HashPassword(request.Password, salt);
            mappedUser.HashedPassword = pass;

            var createdUser = await _unitOfWork.UsersRepository.Insert(mappedUser);
            var Email = createdUser.Email;
            var UserUuid = mappedUser.Id.ToString();

            await _registerEvent.PublishRegisterEvent(Email, pass, UserUuid);
            // var responseLogin = "LLamada a la apiGateway del login con la contraseña "

            var loginUser = _mapperService.Map<User, RegisterResponseDto>(createdUser);
            string token = "token";
            var UserResponse = _mapperService.Map<RegisterResponseDto, UserRegisterResponse>(
                loginUser
            );
            var response = new ResponseRegister { User = UserResponse, Token = token };

            return response;
        }

        public async Task<ResponsePassword> UpdatePassword(
            newPassword request,
            ServerCallContext context
        )
        {
            string id = request.UserId;

            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);

            var password = request.Password;
            var HashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            var repeatedPassword = request.RepeatedPassword;
            var RepeatedHashedPassword = BCrypt.Net.BCrypt.HashPassword(repeatedPassword, salt);

            if (!BCrypt.Net.BCrypt.Verify(password, RepeatedHashedPassword))
            {
                var response2 = new ResponsePassword
                {
                    Response = "Error al comparar las contraseñas",
                };
                return response2;
            }

            //enviar evento a acces service
            await _updateEvent.PublishUpdateEvent(id, HashedPassword);

            var response = new ResponsePassword { Response = "Tamos Redy, contraseña cambiada" };

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
