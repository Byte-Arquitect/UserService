using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using Newtonsoft.Json;
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
            int roleId = 1;
            if (roleId is 3)
                throw new InternalErrorException("Role not found");
            string careerId = request.CareerId;
            if (careerId is "3")
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
            mappedUser.CareerId = careerId.ToString();
            mappedUser.IsEnabled = true;
            mappedUser.RoleName = "Student";
            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            var pass = BCrypt.Net.BCrypt.HashPassword(request.Password, salt);

            var createdUser = await _unitOfWork.UsersRepository.Insert(mappedUser);
            var Email = createdUser.Email;
            var UserUuid = mappedUser.Id.ToString();

            await _registerEvent.PublishRegisterEvent(Email, pass, UserUuid);
            var loginUser = _mapperService.Map<User, RegisterResponseDto>(createdUser);

            var UserResponse = _mapperService.Map<RegisterResponseDto, UserRegisterResponse>(
                loginUser
            );
            // var responseContent = await loginApiGateway(request.Email, request.Password);
            // var jsonDoc = JsonDocument.Parse(responseContent);
            // string token = jsonDoc.RootElement.GetProperty("token").GetString();
            var response = new ResponseRegister { User = UserResponse, Token = "token" };

            return response;
        }

        private async Task<string> loginApiGateway(string email, string password)
        {
            var responseContent = "";
            using (var httpClient = new HttpClient())
            {
                var loginData = new { Email = email, Password = password };

                var jsonContent = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(loginData),
                    Encoding.UTF8,
                    "application/json"
                );

                // Realizar la solicitud POST
                var responseApiGateway = await httpClient.PostAsync(
                    "http://localhost:5111/login",
                    jsonContent
                );

                if (!responseApiGateway.IsSuccessStatusCode)
                {
                    throw new RpcException(
                        new Status(
                            StatusCode.Internal,
                            $"Error calling external API: {responseApiGateway.ReasonPhrase}"
                        )
                    );
                }

                responseContent = await responseApiGateway.Content.ReadAsStringAsync();
            }
            return responseContent;
        }

        public async Task<ResponsePassword> UpdatePassword(
            newPassword request,
            ServerCallContext context
        )
        {
            // Obtener los metadatos
            string id = await GetIdByToken(context);

            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);

            var password = request.Password;
            var HashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            var repeatedPassword = request.RepeatedPassword;
            var RepeatedHashedPassword = BCrypt.Net.BCrypt.HashPassword(repeatedPassword, salt);

            if (!BCrypt.Net.BCrypt.Verify(password, RepeatedHashedPassword))
            {
                var response2 = new ResponsePassword
                {
                    Response = $"Error al comparar las contraseñas",
                };
                return response2;
            }

            //enviar evento a acces service
            await _updateEvent.PublishUpdateEvent(id, HashedPassword);

            var response = new ResponsePassword { Response = $"Contraseña cambiada" };

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

        public async Task<string> GetIdByToken(ServerCallContext context)
        {
            string token = await GetToken(context);
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var id = jsonToken?.Claims.FirstOrDefault(c => c.Type == "UserUuid")?.Value;
            return id;
        }

        public async Task<string> GetToken(ServerCallContext context)
        {
            var headers = context.RequestHeaders;
            var authorizationHeader = headers.GetValue("authorization");
            var token = authorizationHeader.StartsWith("Bearer ")
                ? authorizationHeader.Substring("Bearer ".Length)
                : authorizationHeader;
            return token;
        }
    }
}
