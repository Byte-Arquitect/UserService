using System.Text.Json;
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
        private readonly AuthService authService;

        private readonly IUserProgressRepository userProgressRepository;

        public UserService(
            IUserRepository userRepository,
            ApiGatewayService apiGatewayService,
            AuthService authService,
            IUserProgressRepository userProgressRepository
        )
        {
            this.userRepository = userRepository;
            this.apiGatewayService = apiGatewayService;
            this.authService = authService;
            this.userProgressRepository = userProgressRepository;
        }

        public Task<ResponseMyProgress> GetUserProgress(ServerCallContext context)
        {
            var id = authService.GetIdByToken(context).Result;
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
            var httpContext = context.GetHttpContext();
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

            string id = await authService.GetIdByToken(context);

            if (await userRepository.GetByID(int.Parse(id)) == null)
            {
                return new ResponseSetMyProgress { Message = "Usuario no encontrado" };
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync("http://localhost:5111/subject/");

                if (!response.IsSuccessStatusCode)
                {
                    throw new RpcException(
                        new Status(
                            StatusCode.Internal,
                            $"Error calling external API: {response.ReasonPhrase}"
                        )
                    );
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var subjectsResponse = JsonSerializer.Deserialize<SubjectsResponse>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                var subjects = subjectsResponse?.Subjects;

                var subjectsToAdd = request.SubjectsCodesToAdd;
                var subjectsToRemove = request.SubjectsCodesToDelete;
                var userProgress = userRepository.GetUserProgress(id);

                if (subjectsToAdd.Count > 0)
                {
                    for (int i = subjectsToAdd.Count - 1; i >= 0; i--)
                    {
                        var subject = subjectsToAdd[i];
                        if (!subjects.Any(s => s.Code == subject))
                        {
                            subjectsToAdd.RemoveAt(i);
                        }
                    }
                    for (int i = subjectsToAdd.Count - 1; i >= 0; i--)
                    {
                        var subject = subjectsToAdd[i];
                        if (userProgress.Result.Any(s => s.SubjectCode == subject))
                        {
                            subjectsToAdd.RemoveAt(i);
                        }
                    }
                    foreach (var subject in subjectsToAdd)
                    {
                        Console.WriteLine($"Insertando el ramo: {subject}");
                        _ = await userProgressRepository.Insert(
                            new UserProgress
                            {
                                UserId = int.Parse(id),
                                SubjectId = subjects.First(s => s.Code == subject).Id,
                                SubjectCode = subject,
                                SubjectName = subjects.First(s => s.Code == subject).Name,
                            }
                        );
                    }
                }
                if (subjectsToRemove.Count > 0)
                {
                    for (int i = subjectsToRemove.Count - 1; i >= 0; i--)
                    {
                        var subject = subjectsToRemove[i];
                        if (!subjects.Any(s => s.Code == subject))
                        {
                            subjectsToRemove.RemoveAt(i);
                        }
                    }
                    for (int i = subjectsToRemove.Count - 1; i >= 0; i--)
                    {
                        var subject = subjectsToRemove[i];
                        if (!userProgress.Result.Any(s => s.SubjectCode == subject))
                        {
                            subjectsToRemove.RemoveAt(i);
                        }
                    }
                    foreach (var subject in subjectsToRemove)
                    {
                        var subjectToRemove = userProgress.Result.First(s =>
                            s.SubjectCode == subject
                        );
                        await userProgressRepository.Delete(subjectToRemove);
                    }
                }
                return new ResponseSetMyProgress
                {
                    Message = $"Asignaturas eliminadas y agregadas con éxito",
                };
            }
        }

        public async Task<ResponseGetProfile> GetUserProfile(ServerCallContext context)
        {
            var id = await authService.GetIdByToken(context);
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

        public async Task<ResponseEditProfile> EditUserProfile(
            RequestEditProfile request,
            ServerCallContext context
        )
        {
            string id = authService.GetIdByToken(context).Result;

            if (userRepository.GetByID(id) == null)
            {
                return new ResponseEditProfile { Status = "5", Message = "Usuario no encontrado" };
            }

            var user = userRepository.GetByID(int.Parse(id)).Result;

            user.Name = request.Name != "" ? request.Name : user.Name;
            user.FirstLastName = request.FirstLastName != "" ? request.FirstLastName : user.FirstLastName;
            user.SecondLastName = request.SecondLastName != "" ? request.SecondLastName : user.SecondLastName;

            var updatedUser = await userRepository.Update(user);

            return new ResponseEditProfile { Status = "0", Message = "Usuario editado con exito" };
        }
    }
}
