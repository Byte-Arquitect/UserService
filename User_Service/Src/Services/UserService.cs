using Grpc.Core;
using User_Service.Src.Models;
using User_Service.Src.Protos;
using User_Service.Src.Repositories.Interfaces;
using User_Service.Src.Services.Interfaces;

namespace User_Service.Src.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ApiGatewayService apiGatewayService;

        public UserService(IUserRepository userRepository, ApiGatewayService apiGatewayService)
        {
            this.userRepository = userRepository;
            this.apiGatewayService = apiGatewayService;
        }

        public Task<ResponseMyProgress> GetUserProgress(string id, ServerCallContext context)
        {
            var listofProgress = userRepository.GetUserProgress(id);

            var response = new List<GetProgressDto>();

            listofProgress.Result.ForEach(progress =>
            {
                response.Add(
                    new GetProgressDto
                    {
                        SubjectCode = progress.SubjectCode,
                        SubjectName = progress.SubjectName,
                    }
                );
            });

            return Task.FromResult(new ResponseMyProgress { GetProgress = { response } });
        }

        public async Task<ResponseSetMyProgress> SetUserProgress(
            RequestSetMyProgress request,
            ServerCallContext context
        )
        {
            // Obtener el contexto HTTP desde gRPC
            var httpContext = context.GetHttpContext();

            // Obtener el token desde los headers HTTP
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                throw new RpcException(
                    new Status(
                        StatusCode.Unauthenticated,
                        "Authorization token is missing or invalid"
                    )
                );
            }

            var token = authHeader.Substring("Bearer ".Length);

            var subjects = await apiGatewayService.CallApiGatewayAsync("/subject/", new { token });
            return new ResponseSetMyProgress { Message = subjects.ToString() };
        }

        public async Task<ResponseGetProfile> GetUserProfile(string id, ServerCallContext context)
        {
            var userId = int.Parse(id);
            var profile = await userRepository.GetByID(userId);

            return new ResponseGetProfile
            {
                Id = profile.Id.ToString(),
                Name = profile.Name,
                FirstLastName = profile.FirstLastName,
                SecondLastName = profile.SecondLastName,
                Rut = profile.RUT,
                Email = profile.Email,
                NameCareer = profile.CareerName,
                IdCareer = profile.CareerId.ToString(),
            };
        }
    }
}
